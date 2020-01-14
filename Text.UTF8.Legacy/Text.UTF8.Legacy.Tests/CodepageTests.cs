using NUnit.Framework;
using System.Text;
using Text.UTF8.Legacy.Tests.Extensions;

namespace Text.UTF8.Legacy.Tests
{
    [TestFixture]
    public class CodepageTests
    {
        /// <summary>
        /// Test that we can load the windows-1252 codepage, which is no longer available in .Net Core unless using
        /// the System.Text.Encoding.CodePages package.
        /// </summary>
        [Test]
        public void Should_LoadWindowsCodepage1252()
        {
            var str = "A test string using Windows Codepage 1252";
            var codepage1252Encoder = CodePagesEncodingProvider.Instance.GetEncoding(1252);
            var hex = "41207465737420737472696E67207573696E672057696E646F777320436F6465706167652031323532";
            var expected = hex.HexToBytes();
            var bytes = codepage1252Encoder.GetBytes(str);
            var bytesHex = bytes.ToHexString();
            Assert.AreEqual(expected, bytes);
        }
    }
}
