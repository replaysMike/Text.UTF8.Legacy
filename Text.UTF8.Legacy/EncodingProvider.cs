using System;

namespace Text.UTF8.Legacy
{
    public abstract class EncodingProvider
    {
        public EncodingProvider() { }
        public abstract Encoding GetEncoding(string name);
        public abstract Encoding GetEncoding(int codepage);

        // GetEncoding should return either valid encoding or null. shouldn't throw any exception except on null name
        public virtual Encoding GetEncoding(string name, EncoderFallback encoderFallback, DecoderFallback decoderFallback)
        {
            Encoding enc = GetEncoding(name);
            if (enc != null)
            {
                enc = (Encoding)GetEncoding(name).Clone();
                enc.EncoderFallback = encoderFallback;
                enc.DecoderFallback = decoderFallback;
            }

            return enc;
        }

        public virtual Encoding GetEncoding(int codepage, EncoderFallback encoderFallback, DecoderFallback decoderFallback)
        {
            Encoding enc = GetEncoding(codepage);
            if (enc != null)
            {
                enc = (Encoding)GetEncoding(codepage).Clone();
                enc.EncoderFallback = encoderFallback;
                enc.DecoderFallback = decoderFallback;
            }

            return enc;
        }

        internal static void AddProvider(EncodingProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            lock (_internalSyncObject)
            {
                if (_providers == null)
                {
                    _providers = new EncodingProvider[1] { provider };
                    return;
                }

                if (Array.IndexOf(_providers, provider) >= 0)
                {
                    return;
                }

                var providers = new EncodingProvider[_providers.Length + 1];
                Array.Copy(_providers, providers, _providers.Length);
                providers[providers.Length - 1] = provider;
                _providers = providers;
            }
        }

        internal static Encoding GetEncodingFromProvider(int codepage)
        {
            if (_providers == null)
                return null;

            EncodingProvider[] providers = _providers;
            foreach (EncodingProvider provider in providers)
            {
                Encoding enc = provider.GetEncoding(codepage);
                if (enc != null)
                    return enc;
            }

            return null;
        }

        internal static Encoding GetEncodingFromProvider(string encodingName)
        {
            if (_providers == null)
                return null;

            EncodingProvider[] providers = _providers;
            foreach (EncodingProvider provider in providers)
            {
                Encoding enc = provider.GetEncoding(encodingName);
                if (enc != null)
                    return enc;
            }

            return null;
        }

        internal static Encoding GetEncodingFromProvider(int codepage, EncoderFallback enc, DecoderFallback dec)
        {
            if (_providers == null)
                return null;

            EncodingProvider[] providers = _providers;
            foreach (EncodingProvider provider in providers)
            {
                Encoding encing = provider.GetEncoding(codepage, enc, dec);
                if (encing != null)
                    return encing;
            }

            return null;
        }

        internal static Encoding GetEncodingFromProvider(string encodingName, EncoderFallback enc, DecoderFallback dec)
        {
            if (_providers == null)
                return null;

            EncodingProvider[] providers = _providers;
            foreach (EncodingProvider provider in providers)
            {
                Encoding encoding = provider.GetEncoding(encodingName, enc, dec);
                if (encoding != null)
                    return encoding;
            }

            return null;
        }

        private static object _internalSyncObject = new object();
        private static volatile EncodingProvider[] _providers;
    }
}
