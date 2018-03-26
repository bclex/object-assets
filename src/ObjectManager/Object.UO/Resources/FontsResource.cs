using OA.Ultima.Core.UI.Fonts;
using OA.Ultima.IO;
using OA.Ultima.Resources.Fonts;
using System;
using System.IO;

namespace OA.Ultima.Resources
{
    public class FontsResource
    {
        bool _initialized;
        object _graphicsDevice;

        public const int UniFontCount = 3;
        readonly AFont[] _unicodeFonts = new AFont[UniFontCount];

        public const int AsciiFontCount = 10;
        readonly AFont[] _asciiFonts = new AFont[AsciiFontCount];

        internal AFont GetUniFont(int index)
        {
            if (index < 0 || index >= UniFontCount)
                return _unicodeFonts[0];
            return _unicodeFonts[index];
        }

        internal AFont GetAsciiFont(int index)
        {
            if (index < 0 || index >= AsciiFontCount)
                return _asciiFonts[9];
            return _asciiFonts[index];
        }
       
        public FontsResource(object graphics)
        {
            _graphicsDevice = graphics;
            Initialize();
        }

        void Initialize()
        {
            if (!_initialized)
            {
                _initialized = true;
                //_graphicsDevice.DeviceReset -= GraphicsDeviceReset;
                //_graphicsDevice.DeviceReset += GraphicsDeviceReset;
                LoadFonts();
            }
        }

        void GraphicsDeviceReset(object sender, EventArgs e)
        {
            LoadFonts();
        }

        void LoadFonts()
        {
            // load Ascii fonts
            using (var reader = new BinaryReader(new FileStream(FileManager.GetFilePath("fonts.mul"), FileMode.Open, FileAccess.Read)))
                for (var i = 0; i < AsciiFontCount; i++)
                {
                    _asciiFonts[i] = new FontAscii();
                    _asciiFonts[i].Initialize(reader);
                    _asciiFonts[i].HasBuiltInOutline = true;
                }
            // load Unicode fonts
            var maxHeight = 0; // because all unifonts are designed to be used together, they must all share a single maxheight value.
            for (var i = 0; i < UniFontCount; i++)
            {
                var path = FileManager.GetFilePath("unifont" + (i == 0 ? "" : i.ToString()) + ".mul");
                if (path != null)
                {
                    _unicodeFonts[i] = new FontUnicode();
                    _unicodeFonts[i].Initialize(new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)));
                    if (_unicodeFonts[i].Height > maxHeight)
                        maxHeight = _unicodeFonts[i].Height;
                }
            }
            for (var i = 0; i < UniFontCount; i++)
            {
                if (_unicodeFonts[i] == null)
                    continue;
                _unicodeFonts[i].Height = maxHeight;
            }
        }
    }
}
