using System.Text;

namespace OA.Core.Windows
{
    static class CultureHandler
    {
        static Encoding _encoding;

        public static void InvalidateEncoder()
        {
            _encoding = null;
        }

        public static char TranslateChar(char inputChar)
        {
            if (_encoding == null)
                _encoding = GetCurrentEncoding();
            var chars = _encoding.GetChars(new byte[] { (byte)inputChar });
            return chars[0];
        }

        private static Encoding GetCurrentEncoding()
        {
            var encoding = Encoding.GetEncoding((int)NativeMethods.GetCurrentCodePage());
            Utils.Debug($"Keyboard: Using encoding {encoding.EncodingName} (Code page {encoding.CodePage}).");
            return encoding;
        }
    }
}
