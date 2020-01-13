using NUnit.Framework;
using Text.UTF8.Legacy.Tests.Extensions;

namespace Text.UTF8.Legacy.Tests
{
    [TestFixture]
    public class UTF8Tests
    {
        [Test]
        public void Should_EncodeIncorrectUnicode_ToUtf8()
        {
            // the result of the Unicode encoded string decoded to UTF8 should match both .Net Standard and .Net Framework
            var expected = "�\u0004[\u0004�\u0001�\v,�\u001cn]�$«�� )�:�YH̗I5�V���Nl7α��i�g_�ZQW%\u001d�Dy\u001eЕ\u0013w�v+\u0012*��\u000f*��\u0019r��}���8��w��&�\r���\f����?���&�t�M��[�`kzhz9\u0015�\u0012I�\u001ey_`�\u0011\tF��A�Af~��q��%P�����\u0003�x�(g���e\u001fM�32\u0014��";
            // here we are providing a test sample in Hex that we know to contain invalid UTF8 encoding, but we know parses in .Net Framework
            var hex = "BC045B0488019F0B2CE61C6E5DFC24C2ABE09BDA2029CC3AE9AD5948CC9749359756B1A2D94E6C37CEB189D269AA675FF75A5157251D8544791ED09513779B762B122A89E10F2A98E91972D7CA7DF9F98038DFDB779FED269A0DE3F8FA0C828993B23F85B5A826B474E84DFECD5B87606B7A687A3915C31249CE1E795F609A11094686DF41E99041667E9DD271A0E22550FDD0C3CEF0039678F328679B8590651F4DBE3332148DBA";
            var bytes = hex.HexToBytes();
            var utf8Encoded = Text.UTF8.Legacy.Encoding.UTF8.GetString(bytes);
            Assert.AreEqual(expected, utf8Encoded);
        }

        [Test]
        public void Should_EncodeCorrectUnicode_ToUtf8()
        {
            // the result of the Unicode encoded string decoded to UTF8
            var expected = "B\0a\0s\0i\0c\0 \0u\0n\0i\0c\0o\0d\0e\0 \0t\0e\0s\0t\0 \0s\0t\0r\0i\0n\0g\0";
            // here we are providing a test sample in Hex that we know to contain invalid UTF8 encoding, but we know parses in .Net Framework
            var bytes = System.Text.Encoding.Unicode.GetBytes("Basic unicode test string");
            var utf8Encoded = Text.UTF8.Legacy.Encoding.UTF8.GetString(bytes);
            Assert.AreEqual(expected, utf8Encoded);
        }

#if NETCOREAPP
        [Test]
        public void ShouldNot_SystemText_EncodeIncorrectUnicode_ToUtf8()
        {
            // the result of the Unicode encoded string decoded to UTF8 should NOT in match .Net Core 3 because the UTF8 encoding bug was fixed
            var expected = "�\u0004[\u0004�\u0001�\v,�\u001cn]�$«�� )�:�YH̗I5�V���Nl7α��i�g_�ZQW%\u001d�Dy\u001eЕ\u0013w�v+\u0012*��\u000f*��\u0019r��}���8��w��&�\r���\f����?���&�t�M��[�`kzhz9\u0015�\u0012I�\u001ey_`�\u0011\tF��A�Af~��q��%P�����\u0003�x�(g���e\u001fM�32\u0014��";
            // here we are providing a test sample in Hex that we know to contain invalid UTF8 encoding, but we know parses in .Net Framework
            var hex = "BC045B0488019F0B2CE61C6E5DFC24C2ABE09BDA2029CC3AE9AD5948CC9749359756B1A2D94E6C37CEB189D269AA675FF75A5157251D8544791ED09513779B762B122A89E10F2A98E91972D7CA7DF9F98038DFDB779FED269A0DE3F8FA0C828993B23F85B5A826B474E84DFECD5B87606B7A687A3915C31249CE1E795F609A11094686DF41E99041667E9DD271A0E22550FDD0C3CEF0039678F328679B8590651F4DBE3332148DBA";
            var bytes = hex.HexToBytes();
            var utf8Encoded = System.Text.Encoding.UTF8.GetString(bytes);
            Assert.AreNotEqual(expected, utf8Encoded);
        }

        [Test]
        public void Should_SystemText_EncodeCorrectUnicode_ToUtf8()
        {
            // the result of the Unicode encoded string decoded to UTF8
            var expected = "B\0a\0s\0i\0c\0 \0u\0n\0i\0c\0o\0d\0e\0 \0t\0e\0s\0t\0 \0s\0t\0r\0i\0n\0g\0";
            // here we are providing a test sample in Hex that we know to contain invalid UTF8 encoding, but we know parses in .Net Framework
            var bytes = System.Text.Encoding.Unicode.GetBytes("Basic unicode test string");
            var utf8Encoded = Text.UTF8.Legacy.Encoding.UTF8.GetString(bytes);
            Assert.AreEqual(expected, utf8Encoded);
        }
#endif

#if NETFRAMEWORK
        [Test]
        public void Should_SystemText_EncodeIncorrectUnicode_ToUtf8()
        {
            // the result of the Unicode encoded string decoded to UTF8 should match in .Net Framework
            var expected = "�\u0004[\u0004�\u0001�\v,�\u001cn]�$«�� )�:�YH̗I5�V���Nl7α��i�g_�ZQW%\u001d�Dy\u001eЕ\u0013w�v+\u0012*��\u000f*��\u0019r��}���8��w��&�\r���\f����?���&�t�M��[�`kzhz9\u0015�\u0012I�\u001ey_`�\u0011\tF��A�Af~��q��%P�����\u0003�x�(g���e\u001fM�32\u0014��";
            // here we are providing a test sample in Hex that we know to contain invalid UTF8 encoding, but we know parses in .Net Framework
            var hex = "BC045B0488019F0B2CE61C6E5DFC24C2ABE09BDA2029CC3AE9AD5948CC9749359756B1A2D94E6C37CEB189D269AA675FF75A5157251D8544791ED09513779B762B122A89E10F2A98E91972D7CA7DF9F98038DFDB779FED269A0DE3F8FA0C828993B23F85B5A826B474E84DFECD5B87606B7A687A3915C31249CE1E795F609A11094686DF41E99041667E9DD271A0E22550FDD0C3CEF0039678F328679B8590651F4DBE3332148DBA";
            var bytes = hex.HexToBytes();
            var utf8Encoded = System.Text.Encoding.UTF8.GetString(bytes);
            Assert.AreEqual(expected, utf8Encoded);
        }

        [Test]
        public void Should_SystemText_EncodeCorrectUnicode_ToUtf8()
        {
            // the result of the Unicode encoded string decoded to UTF8
            var expected = "B\0a\0s\0i\0c\0 \0u\0n\0i\0c\0o\0d\0e\0 \0t\0e\0s\0t\0 \0s\0t\0r\0i\0n\0g\0";
            // here we are providing a test sample in Hex that we know to contain invalid UTF8 encoding, but we know parses in .Net Framework
            var bytes = System.Text.Encoding.Unicode.GetBytes("Basic unicode test string");
            var utf8Encoded = Text.UTF8.Legacy.Encoding.UTF8.GetString(bytes);
            Assert.AreEqual(expected, utf8Encoded);
        }
#endif
    }
}
