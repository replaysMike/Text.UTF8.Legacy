using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

namespace Text.UTF8.Legacy
{
    internal class InternalEncoderBestFitFallback : EncoderFallback
    {
        // Our variables
        internal Encoding _encoding = null;
        internal char[] _arrayBestFit = null;

        internal InternalEncoderBestFitFallback(Encoding encoding)
        {
            // Need to load our replacement characters table.
            _encoding = encoding;
        }

        public override EncoderFallbackBuffer CreateFallbackBuffer()
        {
            return new InternalEncoderBestFitFallbackBuffer(this);
        }

        // Maximum number of characters that this instance of this fallback could return
        public override int MaxCharCount
        {
            get
            {
                return 1;
            }
        }

        public override bool Equals(object value)
        {
            var that = value as InternalEncoderBestFitFallback;
            if (that != null)
            {
                return (_encoding.CodePage == that._encoding.CodePage);
            }
            return (false);
        }

        public override int GetHashCode()
        {
            return _encoding.CodePage;
        }
    }

    internal sealed class InternalEncoderBestFitFallbackBuffer : EncoderFallbackBuffer
    {
        // Our variables
        private char _cBestFit = '\0';
        private InternalEncoderBestFitFallback _oFallback;
        private int _iCount = -1;
        private int _iSize;

        // Private object for locking instead of locking on a public type for SQL reliability work.
        private static object _internalSyncObject;
        private static object InternalSyncObject
        {
            get
            {
                if (_internalSyncObject == null)
                {
                    var o = new object();
                    Interlocked.CompareExchange<object>(ref _internalSyncObject, o, null);
                }
                return _internalSyncObject;
            }
        }

        // Constructor
        public InternalEncoderBestFitFallbackBuffer(InternalEncoderBestFitFallback fallback)
        {
            _oFallback = fallback;

            if (_oFallback._arrayBestFit == null)
            {
                // Lock so we don't confuse ourselves.
                lock (InternalSyncObject)
                {
                    // Double check before we do it again.
                    if (_oFallback._arrayBestFit == null)
                        _oFallback._arrayBestFit = fallback._encoding.GetBestFitUnicodeToBytesData();
                }
            }
        }

        // Fallback methods
        public override bool Fallback(char charUnknown, int index)
        {
            // If we had a buffer already we're being recursive, throw, it's probably at the suspect
            // character in our array.
            // Shouldn't be able to get here for all of our code pages, table would have to be messed up.
            Debug.Assert(_iCount < 1, "[InternalEncoderBestFitFallbackBuffer.Fallback(non surrogate)] Fallback char " + ((int)_cBestFit).ToString("X4", CultureInfo.InvariantCulture) + " caused recursive fallback");

            _iCount = _iSize = 1;
            _cBestFit = TryBestFit(charUnknown);
            if (_cBestFit == '\0')
                _cBestFit = '?';

            return true;
        }

        public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
        {
            // Double check input surrogate pair
            if (!char.IsHighSurrogate(charUnknownHigh))
                throw new ArgumentOutOfRangeException(nameof(charUnknownHigh),
                    SR.Format(SR.ArgumentOutOfRange_Range,
                    0xD800, 0xDBFF));

            if (!char.IsLowSurrogate(charUnknownLow))
                throw new ArgumentOutOfRangeException(nameof(charUnknownLow),
                    SR.Format(SR.ArgumentOutOfRange_Range,
                    0xDC00, 0xDFFF));

            // If we had a buffer already we're being recursive, throw, it's probably at the suspect
            // character in our array.  0 is processing last character, < 0 is not falling back
            // Shouldn't be able to get here, table would have to be messed up.
            Debug.Assert(_iCount < 1, "[InternalEncoderBestFitFallbackBuffer.Fallback(surrogate)] Fallback char " + ((int)_cBestFit).ToString("X4", CultureInfo.InvariantCulture) + " caused recursive fallback");

            // Go ahead and get our fallback, surrogates don't have best fit
            _cBestFit = '?';
            _iCount = _iSize = 2;

            return true;
        }

        // Default version is overridden in EncoderReplacementFallback.cs
        public override char GetNextChar()
        {
            // We want it to get < 0 because == 0 means that the current/last character is a fallback
            // and we need to detect recursion.  We could have a flag but we already have this counter.
            _iCount--;

            // Do we have anything left? 0 is now last fallback char, negative is nothing left
            if (_iCount < 0)
                return '\0';

            // Need to get it out of the buffer.
            // Make sure it didn't wrap from the fast count-- path
            if (_iCount == int.MaxValue)
            {
                _iCount = -1;
                return '\0';
            }

            // Return the best fit character
            return _cBestFit;
        }

        public override bool MovePrevious()
        {
            // Exception fallback doesn't have anywhere to back up to.
            if (_iCount >= 0)
                _iCount++;

            // Return true if we could do it.
            return (_iCount >= 0 && _iCount <= _iSize);
        }


        // How many characters left to output?
        public override int Remaining
        {
            get
            {
                return (_iCount > 0) ? _iCount : 0;
            }
        }

        // Clear the buffer
        public override unsafe void Reset()
        {
            _iCount = -1;
            _charStart = null;
            _bFallingBack = false;
        }

        // private helper methods
        private char TryBestFit(char cUnknown)
        {
            // Need to figure out our best fit character, low is beginning of array, high is 1 AFTER end of array
            int lowBound = 0;
            int highBound = _oFallback._arrayBestFit.Length;
            int index;

            // Binary search the array
            int iDiff;
            while ((iDiff = (highBound - lowBound)) > 6)
            {
                // Look in the middle, which is complicated by the fact that we have 2 #s for each pair,
                // so we don't want index to be odd because we want to be on word boundaries.
                // Also note that index can never == highBound (because diff is rounded down)
                index = ((iDiff / 2) + lowBound) & 0xFFFE;

                char cTest = _oFallback._arrayBestFit[index];
                if (cTest == cUnknown)
                {
                    // We found it
                    Debug.Assert(index + 1 < _oFallback._arrayBestFit.Length,
                        "[InternalEncoderBestFitFallbackBuffer.TryBestFit]Expected replacement character at end of array");
                    return _oFallback._arrayBestFit[index + 1];
                }
                else if (cTest < cUnknown)
                {
                    // We weren't high enough
                    lowBound = index;
                }
                else
                {
                    // We weren't low enough
                    highBound = index;
                }
            }

            for (index = lowBound; index < highBound; index += 2)
            {
                if (_oFallback._arrayBestFit[index] == cUnknown)
                {
                    // We found it
                    Debug.Assert(index + 1 < _oFallback._arrayBestFit.Length,
                        "[InternalEncoderBestFitFallbackBuffer.TryBestFit]Expected replacement character at end of array");
                    return _oFallback._arrayBestFit[index + 1];
                }
            }

            // Char wasn't in our table
            return '\0';
        }
    }
}

