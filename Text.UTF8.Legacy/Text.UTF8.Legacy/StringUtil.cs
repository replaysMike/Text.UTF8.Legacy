using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Text.UTF8.Legacy
{
    public class StringUtil
    {
        internal static string FastAllocateString(int length)
        {
            var fastAllocate = typeof(string).GetMethods(BindingFlags.NonPublic | BindingFlags.Static)
                .First(x => x.Name == "FastAllocateString");

            return (string)fastAllocate.Invoke(null, new object[] { length });
        }

        internal static unsafe string CreateStringFromEncoding(
            byte* bytes, int byteLength, Encoding encoding)
        {
            Debug.Assert(bytes != null);
            Debug.Assert(byteLength >= 0);

            // Get our string length
            var stringLength = encoding.GetCharCount(bytes, byteLength);
            Debug.Assert(stringLength >= 0, "stringLength >= 0");

            // They gave us an empty string if they needed one
            // 0 bytelength might be possible if there's something in an encoder
            if (stringLength == 0)
                return string.Empty;

            var s = FastAllocateString(stringLength);
            //var stringPtr = Marshal.StringToBSTR(s);
            

            fixed (char* pTempChars = s)
            {
                //var firstChar = s[0];
                //var pTempChars = &firstChar;
                var doubleCheck = encoding.GetChars(bytes, byteLength, pTempChars, stringLength);
                var pass = stringLength == doubleCheck;
                //if (!pass)
                //    Debugger.Break();
                /*Debug.Assert(stringLength == doubleCheck,
                    "Expected encoding.GetChars to return same length as encoding.GetCharCount");*/
            }

            return s;
        }
    }
}
