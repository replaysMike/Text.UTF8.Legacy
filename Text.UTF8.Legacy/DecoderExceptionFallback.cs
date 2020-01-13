using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Text.UTF8.Legacy
{
    public sealed class DecoderExceptionFallback : DecoderFallback
    {
        // Construction
        public DecoderExceptionFallback()
        {
        }

        public override DecoderFallbackBuffer CreateFallbackBuffer()
        {
            return new DecoderExceptionFallbackBuffer();
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
            var that = value as DecoderExceptionFallback;
            if (that != null)
            {
                return (true);
            }
            return (false);
        }

        public override int GetHashCode()
        {
            return 879;
        }
    }


    public sealed class DecoderExceptionFallbackBuffer : DecoderFallbackBuffer
    {
        public override bool Fallback(byte[] bytesUnknown, int index)
        {
            Throw(bytesUnknown, index);
            return true;
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

        private void Throw(byte[] bytesUnknown, int index)
        {
            // Create a string representation of our bytes.            
            var strBytes = new System.Text.StringBuilder(bytesUnknown.Length * 3);

            int i;
            for (i = 0; i < bytesUnknown.Length && i < 20; i++)
            {
                strBytes.Append('[');
                strBytes.Append(bytesUnknown[i].ToString("X2", CultureInfo.InvariantCulture));
                strBytes.Append(']');
            }

            // In case the string's really long
            if (i == 20)
                strBytes.Append(" ...");

            // Known index
            throw new DecoderFallbackException($"SR.Argument_InvalidCodePageBytesIndex,{strBytes}, {index}), {bytesUnknown}, {index}");
        }
    }

    // Exception for decoding unknown byte sequences.
    [Serializable]
    [System.Runtime.CompilerServices.TypeForwardedFrom("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089")]
    public sealed class DecoderFallbackException : ArgumentException
    {
        private byte[] _bytesUnknown = null;
        private int _index = 0;

        public DecoderFallbackException()
            : base()
        {
            HResult = -1;
        }

        public DecoderFallbackException(string message)
            : base(message)
        {
            HResult = -2;
        }

        public DecoderFallbackException(string message, Exception innerException)
            : base(message, innerException)
        {
            HResult = -3;
        }

        public DecoderFallbackException(string message, byte[] bytesUnknown, int index)
            : base(message)
        {
            _bytesUnknown = bytesUnknown;
            _index = index;
        }

        private DecoderFallbackException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        public byte[] BytesUnknown
        {
            get
            {
                return (_bytesUnknown);
            }
        }

        public int Index
        {
            get
            {
                return _index;
            }
        }
    }
}
