using System;
using System.Runtime.Serialization;

namespace Text.UTF8.Legacy
{
    public sealed class EncoderExceptionFallback : EncoderFallback
    {
        // Construction
        public EncoderExceptionFallback()
        {
        }

        public override EncoderFallbackBuffer CreateFallbackBuffer()
        {
            return new EncoderExceptionFallbackBuffer();
        }

        // Maximum number of characters that this instance of this fallback could return
        public override int MaxCharCount
        {
            get
            {
                return 0;
            }
        }

        public override bool Equals(object value)
        {
            var that = value as EncoderExceptionFallback;
            if (that != null)
            {
                return (true);
            }
            return (false);
        }

        public override int GetHashCode()
        {
            return 654;
        }
    }


    public sealed class EncoderExceptionFallbackBuffer : EncoderFallbackBuffer
    {
        public EncoderExceptionFallbackBuffer() { }
        public override bool Fallback(char charUnknown, int index)
        {
            // Fall back our char
            throw new EncoderFallbackException(
                SR.Format(SR.Argument_InvalidCodePageConversionIndex, (int)charUnknown, index), charUnknown, index);
        }

        public override bool Fallback(char charUnknownHigh, char charUnknownLow, int index)
        {
            if (!char.IsHighSurrogate(charUnknownHigh))
            {
                throw new ArgumentOutOfRangeException(nameof(charUnknownHigh),
                    SR.Format(SR.ArgumentOutOfRange_Range, 0xD800, 0xDBFF));
            }
            if (!char.IsLowSurrogate(charUnknownLow))
            {
                throw new ArgumentOutOfRangeException(nameof(charUnknownLow),
                    SR.Format(SR.ArgumentOutOfRange_Range, 0xDC00, 0xDFFF));
            }

            int iTemp = char.ConvertToUtf32(charUnknownHigh, charUnknownLow);

            // Fall back our char
            throw new EncoderFallbackException(
                SR.Format(SR.Argument_InvalidCodePageConversionIndex, iTemp, index), charUnknownHigh, charUnknownLow, index);
        }

        public override char GetNextChar()
        {
            return (char)0;
        }

        public override bool MovePrevious()
        {
            // Exception fallback doesn't have anywhere to back up to.
            return false;
        }

        // Exceptions are always empty
        public override int Remaining
        {
            get
            {
                return 0;
            }
        }
    }

    [Serializable]
    [System.Runtime.CompilerServices.TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public sealed class EncoderFallbackException : ArgumentException
    {
        private char _charUnknown;
        private char _charUnknownHigh;
        private char _charUnknownLow;
        private int _index;

        public EncoderFallbackException()
            : base(SR.Arg_ArgumentException)
        {
            HResult = -1;
        }

        public EncoderFallbackException(string message)
            : base(message)
        {
            HResult = -2;
        }

        public EncoderFallbackException(string message, Exception innerException)
            : base(message, innerException)
        {
            HResult = -3;
        }

        internal EncoderFallbackException(
            string message, char charUnknown, int index) : base(message)
        {
            _charUnknown = charUnknown;
            _index = index;
        }

        internal EncoderFallbackException(
            string message, char charUnknownHigh, char charUnknownLow, int index) : base(message)
        {
            if (!char.IsHighSurrogate(charUnknownHigh))
            {
                throw new ArgumentOutOfRangeException(nameof(charUnknownHigh),
                    SR.Format(SR.ArgumentOutOfRange_Range, 0xD800, 0xDBFF));
            }
            if (!char.IsLowSurrogate(charUnknownLow))
            {
                throw new ArgumentOutOfRangeException(nameof(CharUnknownLow),
                    SR.Format(SR.ArgumentOutOfRange_Range, 0xDC00, 0xDFFF));
            }

            _charUnknownHigh = charUnknownHigh;
            _charUnknownLow = charUnknownLow;
            _index = index;
        }

        private EncoderFallbackException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        public char CharUnknown
        {
            get
            {
                return (_charUnknown);
            }
        }

        public char CharUnknownHigh
        {
            get
            {
                return (_charUnknownHigh);
            }
        }

        public char CharUnknownLow
        {
            get
            {
                return (_charUnknownLow);
            }
        }

        public int Index
        {
            get
            {
                return _index;
            }
        }

        // Return true if the unknown character is a surrogate pair.
        public bool IsUnknownSurrogate()
        {
            return (_charUnknownHigh != '\0');
        }
    }
}
