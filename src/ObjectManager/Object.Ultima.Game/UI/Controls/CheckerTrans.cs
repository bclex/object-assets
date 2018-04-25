using OA.Core;
using OA.Core.UI;
using OA.Ultima.Core.Graphics;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    class CheckerTrans : AControl
    {
        static Texture2DInfo _checkeredTransTexture;
        public static Texture2DInfo CheckeredTransTexture
        {
            get
            {
                if (_checkeredTransTexture == null)
                {
                    var data = new ushort[32 * 32];
                    for (var h = 0; h < 32; h++)
                    {
                        var i = h % 2;
                        for (var w = 0; w < 32; w++)
                        {
                            if (i++ >= 1) { data[h * 32 + w] = 0x8000; i = 0; }
                            else data[h * 32 + w] = 0x0000;
                        }
                    }
                    var sb = Service.Get<SpriteBatchUI>();
                    _checkeredTransTexture = new Texture2DInfo(sb.GraphicsDevice, 32, 32, false, SurfaceFormat.Bgra5551);
                    _checkeredTransTexture.SetData<ushort>(data);
                }
                return _checkeredTransTexture;
            }
        }

        CheckerTrans(AControl parent)
            : base(parent)
        {
        }

        public CheckerTrans(AControl parent, string[] arguements)
            : this(parent)
        {
            var x = int.Parse(arguements[1]);
            var y = int.Parse(arguements[2]);
            var width = int.Parse(arguements[3]);
            var height = int.Parse(arguements[4]);
            BuildGumpling(x, y, width, height);
        }

        public CheckerTrans(AControl parent, int x, int y, int width, int height)
            : this(parent)
        {
            BuildGumpling(x, y, width, height);
        }

        void BuildGumpling(int x, int y, int width, int height)
        {
            Position = new Vector2Int(x, y);
            Size = new Vector2Int(width, height);
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            spriteBatch.Draw2DTiled(CheckeredTransTexture, new RectInt(position.x, position.y, Width, Height), Vector3.zero);
            base.Draw(spriteBatch, position, frameMS);
        }
    }
}
