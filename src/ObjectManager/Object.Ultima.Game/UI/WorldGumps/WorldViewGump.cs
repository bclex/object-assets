using OA.Core;
using OA.Core.UI;
using OA.Ultima.Core.Graphics;
using OA.Ultima.UI.Controls;
using OA.Ultima.World;
using UnityEngine;

namespace OA.Ultima.UI.WorldGumps
{
    /// <summary>
    /// A bordered container that displays the world control.
    /// </summary>
    class WorldViewGump : Gump
    {
        WorldModel _model;

        WorldViewport _viewport;
        ResizePic _border;
        ChatControl _chatWindow;

        const int BorderWidth = 5, BorderHeight = 7;
        int _worldWidth, _worldHeight;

        public WorldViewGump()
            : base(0, 0)
        {
            HandlesMouseInput = false;
            IsUncloseableWithRMB = true;
            IsUncloseableWithEsc = true;
            IsMoveable = true;
            MetaData.Layer = UILayer.Under;

            _model = Service.Get<WorldModel>();

            _worldWidth = UltimaGameSettings.UserInterface.PlayWindowGumpResolution.Width;
            _worldHeight = UltimaGameSettings.UserInterface.PlayWindowGumpResolution.Height;

            Position = new Vector2Int(32, 32);
            OnResize();
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("worldview");
            base.OnInitialize();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (_worldWidth != UltimaGameSettings.UserInterface.PlayWindowGumpResolution.Width || _worldHeight != UltimaGameSettings.UserInterface.PlayWindowGumpResolution.Height)
            {
                _worldWidth = UltimaGameSettings.UserInterface.PlayWindowGumpResolution.Width;
                _worldHeight = UltimaGameSettings.UserInterface.PlayWindowGumpResolution.Height;
                OnResize();
            }

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            base.Draw(spriteBatch, position, frameMS);
        }

        protected override void OnMove()
        {
            // base.OnMove() would make sure that the gump remained at least half on screen, but we want more fine-grained control over movement.
            var sb = Service.Get<SpriteBatchUI>();
            var position = Position;
            if (position.x < -BorderWidth)
                position.x = -BorderWidth;
            if (position.y < -BorderHeight)
                position.y = -BorderHeight;
            if (position.x + Width - BorderWidth > sb.GraphicsDevice.Viewport.Width)
                position.x = sb.GraphicsDevice.Viewport.Width - (Width - BorderWidth);
            if (position.y + Height - BorderHeight > sb.GraphicsDevice.Viewport.Height)
                position.y = sb.GraphicsDevice.Viewport.Height - (Height - BorderHeight);
            Position = position;
        }

        private void OnResize()
        {
            if (Service.Has<ChatControl>())
                Service.Remove<ChatControl>();
            ClearControls();
            Size = new Vector2Int(_worldWidth + BorderWidth * 2, _worldHeight + BorderHeight * 2);
            AddControl(_border = new ResizePic(this, 0, 0, 0xa3c, Width, Height));
            AddControl(_viewport = new WorldViewport(this, BorderWidth, BorderHeight, _worldWidth, _worldHeight));
            AddControl(_chatWindow = new ChatControl(this, BorderWidth, BorderHeight, _worldWidth, _worldHeight));
            Service.Add<ChatControl>(_chatWindow);
        }
    }
}
