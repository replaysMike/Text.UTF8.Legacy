using System;
using System.Diagnostics;
using System.Threading;

namespace Text.UTF8.Legacy
{
    public abstract class EncoderFallback
    {
        private static EncoderFallback _replacementFallback; // Default fallback, uses no best fit & "?"
        private static EncoderFallback _exceptionFallback;

        // Get each of our generic fallbacks.

        public static EncoderFallback ReplacementFallback
        {
            get
            {
                if (_replacementFallback == null)
                    Interlocked.CompareExchange<EncoderFallback>(ref _replacementFallback, new EncoderReplacementFallback(), null);

                return _replacementFallback;
            }
        }


        public static EncoderFallback ExceptionFallback
        {
            get
            {
                if (_exceptionFallback == null)
                    Interlocked.CompareExchange<EncoderFallback>(ref _exceptionFallback, new EncoderExceptionFallback(), null);

                return _exceptionFallback;
            }
        }

        // Fallback
        //
        // Return the appropriate unicode string alternative to the character that need to fall back.
        // Most implementations will be:
        //      return new MyCustomEncoderFallbackBuffer(this);

        public abstract EncoderFallbackBuffer CreateFallbackBuffer();

        // Maximum number of characters that this instance of this fallback could return

        public abstract int MaxCharCount { get; }
    }


    public abstract class EncoderFallbackBuffer
    {
        // Most implementations will probably need an implementation-specific constructor

        // Public methods that cannot be overridden that let us do our fallback thing
        // These wrap the internal methods so that we can check for people doing stuff that is incorrect

        public abstract bool Fallback(char charUnknown, int index);

        public abstract bool Fallback(char charUnknownHigh, char charUnknownLow, int index);

        // Get next character

        public abstract char GetNextChar();

        // Back up a character

        public abstract bool MovePrevious();

        // How many chars left in this fallback?

        public abstract int Remaining { get; }

        // Not sure if this should be public or not.
        // Clear the buffer

        public virtual void Reset()
        {
            while (GetNextChar() != (char)0) ;
        }

        // Internal items to help us figure out what we're doing as far as error messages, etc.
        // These help us with our performance and messages internally
        internal unsafe char* _charStart;
        internal unsafe char* _charEnd;
        internal EncoderNLS _encoder;
        internal bool _setEncoder;
        internal bool _bUsedEncoder;
        internal bool _bFallingBack = false;
        internal int _iRecursionCount = 0;
        private const int MaxRecursion = 250;

        // Internal Reset
        // For example, what if someone fails a conversion and wants to reset one of our fallback buffers?
        internal unsafe void InternalReset()
        {
            _charStart = null;
            _bFallingBack = false;
            _iRecursionCount = 0;
            Reset();
        }

        // Set the above values
        // This can't be part of the constructor because EncoderFallbacks would have to know how to implement these.
        internal unsafe void InternalInitialize(char* charStart, char* charEnd, EncoderNLS encoder, bool setEncoder)
        {
            _charStart = charStart;
            _charEnd = charEnd;
            _encoder = encoder;
            _setEncoder = setEncoder;
            _bUsedEncoder = false;
            _bFallingBack = false;
            _iRecursionCount = 0;
        }

        internal char InternalGetNextChar()
        {
            char ch = GetNextChar();
            _bFallingBack = (ch != 0);
            if (ch == 0) _iRecursionCount = 0;
            return ch;
        }

        // Fallback the current character using the remaining buffer and encoder if necessary
        // This can only be called by our encodings (other have to use the public fallback methods), so
        // we can use our EncoderNLS here too.
        // setEncoder is true if we're calling from a GetBytes method, false if we're calling from a GetByteCount
        //
        // Note that this could also change the contents of this.encoder, which is the same
        // object that the caller is using, so the caller could mess up the encoder for us
        // if they aren't careful.
        internal unsafe virtual bool InternalFallback(char ch, ref char* chars)
        {
            // Shouldn't have null charStart
            Debug.Assert(_charStart != null,
                "[EncoderFallback.InternalFallbackBuffer]Fallback buffer is not initialized");

            // Get our index, remember chars was preincremented to point at next char, so have to -1
            int index = (int)(chars - _charStart) - 1;

            // See if it was a high surrogate
            if (char.IsHighSurrogate(ch))
            {
                // See if there's a low surrogate to go with it
                if (chars >= _charEnd)
                {
                    // Nothing left in input buffer
                    // No input, return 0 if mustflush is false
                    if (_encoder != null && !_encoder.MustFlush)
                    {
                        // Done, nothing to fallback
                        if (_setEncoder)
                        {
                            _bUsedEncoder = true;
                            _encoder._charLeftOver = ch;
                        }
                        _bFallingBack = false;
                        return false;
                    }
                }
                else
                {
                    // Might have a low surrogate
                    char cNext = *chars;
                    if (char.IsLowSurrogate(cNext))
                    {
                        // If already falling back then fail
                        if (_bFallingBack && _iRecursionCount++ > MaxRecursion)
                            ThrowLastCharRecursive(char.ConvertToUtf32(ch, cNext));

                        // Next is a surrogate, add it as surrogate pair, and increment chars
                        chars++;
                        _bFallingBack = Fallback(ch, cNext, index);
                        return _bFallingBack;
                    }
                    // Next isn't a low surrogate, just fallback the high surrogate
                }
            }

            // If already falling back then fail
            if (_bFallingBack && _iRecursionCount++ > MaxRecursion)
                ThrowLastCharRecursive((int)ch);

            // Fall back our char
            _bFallingBack = Fallback(ch, index);

            return _bFallingBack;
        }

        // private helper methods
        internal void ThrowLastCharRecursive(int charRecursive)
        {
            // Throw it, using our complete character
            throw new ArgumentException(
                SR.Format(SR.Argument_RecursiveFallback,
                    charRecursive), "chars");
        }
    }
}
