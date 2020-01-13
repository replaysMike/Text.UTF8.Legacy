using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Text.UTF8.Legacy
{
    public abstract class DecoderFallback
    {
        private static DecoderFallback _replacementFallback; // Default fallback, uses no best fit & "?"
        private static DecoderFallback _exceptionFallback;

        public static DecoderFallback ReplacementFallback =>
            _replacementFallback ?? Interlocked.CompareExchange(ref _replacementFallback, new DecoderReplacementFallback(), null) ?? _replacementFallback;        


        public static DecoderFallback ExceptionFallback =>
            _exceptionFallback ?? Interlocked.CompareExchange<DecoderFallback>(ref _exceptionFallback, new DecoderExceptionFallback(), null) ?? _exceptionFallback;

        // Fallback
        //
        // Return the appropriate unicode string alternative to the character that need to fall back.
        // Most implementations will be:
        //      return new MyCustomDecoderFallbackBuffer(this);

        public abstract DecoderFallbackBuffer CreateFallbackBuffer();

        // Maximum number of characters that this instance of this fallback could return

        public abstract int MaxCharCount { get; }
    }


    public abstract class DecoderFallbackBuffer
    {
        // Most implementations will probably need an implementation-specific constructor

        // internal methods that cannot be overridden that let us do our fallback thing
        // These wrap the internal methods so that we can check for people doing stuff that's incorrect

        public abstract bool Fallback(byte[] bytesUnknown, int index);

        // Get next character

        public abstract char GetNextChar();

        // Back up a character

        public abstract bool MovePrevious();

        // How many chars left in this fallback?

        public abstract int Remaining { get; }

        // Clear the buffer

        public virtual void Reset()
        {
            while (GetNextChar() != (char)0) ;
        }

        // Internal items to help us figure out what we're doing as far as error messages, etc.
        // These help us with our performance and messages internally
        internal unsafe byte* _byteStart;
        internal unsafe char* _charEnd;

        // Internal Reset
        internal unsafe void InternalReset()
        {
            _byteStart = null;
            Reset();
        }

        // Set the above values
        // This can't be part of the constructor because DecoderFallbacks would have to know how to implement these.
        internal unsafe void InternalInitialize(byte* byteStart, char* charEnd)
        {
            _byteStart = byteStart;
            _charEnd = charEnd;
        }

        // Fallback the current byte by sticking it into the remaining char buffer.
        // This can only be called by our encodings (other have to use the public fallback methods), so
        // we can use our DecoderNLS here too (except we don't).
        // Returns true if we are successful, false if we can't fallback the character (no buffer space)
        // So caller needs to throw buffer space if return false.
        // Right now this has both bytes and bytes[], since we might have extra bytes, hence the
        // array, and we might need the index, hence the byte*
        // Don't touch ref chars unless we succeed
        internal unsafe virtual bool InternalFallback(byte[] bytes, byte* pBytes, ref char* chars)
        {
            Debug.Assert(_byteStart != null, "[DecoderFallback.InternalFallback]Used InternalFallback without calling InternalInitialize");

            // See if there's a fallback character and we have an output buffer then copy our string.
            if (Fallback(bytes, (int)(pBytes - _byteStart - bytes.Length)))
            {
                // Copy the chars to our output
                char ch;
                char* charTemp = chars;
                bool bHighSurrogate = false;
                while ((ch = GetNextChar()) != 0)
                {
                    // Make sure no mixed up surrogates
                    if (char.IsSurrogate(ch))
                    {
                        if (char.IsHighSurrogate(ch))
                        {
                            // High Surrogate
                            if (bHighSurrogate)
                                throw new ArgumentException(SR.Argument_InvalidCharSequenceNoIndex);
                            bHighSurrogate = true;
                        }
                        else
                        {
                            // Low surrogate
                            if (bHighSurrogate == false)
                                throw new ArgumentException(SR.Argument_InvalidCharSequenceNoIndex);
                            bHighSurrogate = false;
                        }
                    }

                    if (charTemp >= _charEnd)
                    {
                        // No buffer space
                        return false;
                    }

                    *(charTemp++) = ch;
                }

                // Need to make sure that bHighSurrogate isn't true
                if (bHighSurrogate)
                    throw new ArgumentException(SR.Argument_InvalidCharSequenceNoIndex);

                // Now we aren't going to be false, so its OK to update chars
                chars = charTemp;
            }

            return true;
        }

        // This version just counts the fallback and doesn't actually copy anything.
        internal unsafe virtual int InternalFallback(byte[] bytes, byte* pBytes)
        // Right now this has both bytes and bytes[], since we might have extra bytes, hence the
        // array, and we might need the index, hence the byte*
        {
            Debug.Assert(_byteStart != null, "[DecoderFallback.InternalFallback]Used InternalFallback without calling InternalInitialize");

            // See if there's a fallback character and we have an output buffer then copy our string.
            if (Fallback(bytes, (int)(pBytes - _byteStart - bytes.Length)))
            {
                int count = 0;

                char ch;
                bool bHighSurrogate = false;
                while ((ch = GetNextChar()) != 0)
                {
                    // Make sure no mixed up surrogates
                    if (char.IsSurrogate(ch))
                    {
                        if (char.IsHighSurrogate(ch))
                        {
                            // High Surrogate
                            if (bHighSurrogate)
                                throw new ArgumentException(SR.Argument_InvalidCharSequenceNoIndex);
                            bHighSurrogate = true;
                        }
                        else
                        {
                            // Low surrogate
                            if (bHighSurrogate == false)
                                throw new ArgumentException(SR.Argument_InvalidCharSequenceNoIndex);
                            bHighSurrogate = false;
                        }
                    }

                    count++;
                }

                // Need to make sure that bHighSurrogate isn't true
                if (bHighSurrogate)
                    throw new ArgumentException(SR.Argument_InvalidCharSequenceNoIndex);

                return count;
            }

            // If no fallback return 0
            return 0;
        }

        // private helper methods
        internal void ThrowLastBytesRecursive(byte[] bytesUnknown)
        {
            // Create a string representation of our bytes.
            var strBytes = new System.Text.StringBuilder(bytesUnknown.Length * 3);
            int i;
            for (i = 0; i < bytesUnknown.Length && i < 20; i++)
            {
                if (strBytes.Length > 0)
                    strBytes.Append(' ');
                strBytes.AppendFormat(CultureInfo.InvariantCulture, "\\x{0:X2}", bytesUnknown[i]);
            }
            // In case the string's really long
            if (i == 20)
                strBytes.Append(" ...");

            // Throw it, using our complete bytes
            throw new ArgumentException(
                SR.Format(SR.Argument_RecursiveFallbackBytes,
                    strBytes.ToString()), nameof(bytesUnknown));
        }
    }
}
