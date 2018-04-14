using OA.Core;
using OA.Core.Input;
using OA.Ultima.Core.Graphics;
using OA.Ultima.UI.Controls;
using OA.Ultima.World;
using UnityEngine;

namespace OA.Ultima.UI.WorldGumps
{
    public class DebugGump : Gump
    {
        readonly WorldModel _world;
        HtmlGumpling _debug;

        public DebugGump()
            : base(0, 0)
        {
            _world = Service.Get<WorldModel>();
            IsMoveable = true;
            AddControl(new ResizePic(this, 0, 0, 0x2436, 256 + 16, 256 + 16));
            AddControl(_debug = new HtmlGumpling(this, 0, 0, 256, 256, 0, 0, string.Empty));
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
            var input = Service.Get<IInputService>();
            var lmb = input.IsMouseButtonDown(MouseButton.Left);
            var rmb = input.IsMouseButtonDown(MouseButton.Right);
            _debug.Text = $"{(lmb ? "LMB" : string.Empty)}:{(rmb ? "RMB" : string.Empty)}";
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            base.Draw(spriteBatch, position, frameMS);

            spriteBatch.Draw2D(((WorldView)_world.GetView()).MiniMap.Texture, new Vector3(position.X + 8, position.Y + 8, 0), Vector3.Zero);
        }
    }
}