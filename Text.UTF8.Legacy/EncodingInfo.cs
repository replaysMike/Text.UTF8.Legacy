using System;
using System.Text;

namespace Text.UTF8.Legacy
{
    public sealed class EncodingInfo
    {
        private int _iCodePage;          // Code Page #
        private string _strEncodingName;    // Short name (web name)
        private string _strDisplayName;     // Full localized name

        internal EncodingInfo(int codePage, string name, string displayName)
        {
            _iCodePage = codePage;
            _strEncodingName = name;
            _strDisplayName = displayName;
        }


        public int CodePage
        {
            get
            {
                return _iCodePage;
            }
        }


        public string Name
        {
            get
            {
                return _strEncodingName;
            }
        }


        public string DisplayName
        {
            get
            {
                return _strDisplayName;
            }
        }


        public Encoding GetEncoding()
        {
            return Encoding.GetEncoding(_iCodePage);
        }

        public override bool Equals(object value)
        {
            var that = value as EncodingInfo;
            if (that != null)
            {
                return (CodePage == that.CodePage);
            }
            return (false);
        }

        public override int GetHashCode()
        {
            return CodePage;
        }
    }
}
