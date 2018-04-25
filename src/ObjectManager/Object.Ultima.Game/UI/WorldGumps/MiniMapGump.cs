using OA.Core;
using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Resources;
using OA.Ultima.World;
using System;
using UnityEngine;

namespace OA.Ultima.UI.WorldGumps
{
    class MiniMapGump : Gump
    {
        const float ReticleBlinkMS = 250f;

        float _timeMS;
        bool _useLargeMap;
        WorldModel _world;
        Texture2DInfo _gumpTexture;
        Texture2DInfo _playerIndicator;

        public static bool MiniMap_LargeFormat { get; set; }

        public static void Toggle()
        {
            var ui = Service.Get<UserInterfaceService>();
            if (ui.GetControl<MiniMapGump>() == null)
                ui.AddControl(new MiniMapGump(), 566, 25);
            else
            {
                if (MiniMapGump.MiniMap_LargeFormat == false)
                    MiniMapGump.MiniMap_LargeFormat = true;
                else
                {
                    ui.RemoveControl<MiniMapGump>();
                    MiniMapGump.MiniMap_LargeFormat = false;
                }
            }
        }

        public MiniMapGump()
            : base(0, 0)
        {
            _world = Service.Get<WorldModel>();
            _useLargeMap = MiniMap_LargeFormat;
            IsMoveable = true;
            MakeThisADragger();
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("minimap");
            base.OnInitialize();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (_gumpTexture == null || _useLargeMap != MiniMap_LargeFormat)
            {
                _useLargeMap = MiniMap_LargeFormat;
                if (_gumpTexture != null)
                    _gumpTexture = null;
                var provider = Service.Get<IResourceProvider>();
                _gumpTexture = provider.GetUITexture(_useLargeMap ? 5011 : 5010, true);
                Size = new Vector2Int(_gumpTexture.width, _gumpTexture.height);
            }
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            var player = WorldModel.Entities.GetPlayerEntity();
            var x = (float)Math.Round((player.Position.X % 256) + player.Position.X_offset) / 256f;
            var y = (float)Math.Round((player.Position.Y % 256) + player.Position.Y_offset) / 256f;
            var playerPosition = new Vector3(x - y, x + y, 0f);
            var minimapU = (_gumpTexture.width / 256f) / 2f;
            var minimapV = (_gumpTexture.height / 256f) / 2f;

            VertexPositionNormalTextureHue[] v = 
            {
                new VertexPositionNormalTextureHue(new Vector3(position.x, position.y, 0), playerPosition + new Vector3(-minimapU, -minimapV, 0), new Vector3(0, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.x + Width, position.y, 0), playerPosition + new Vector3(minimapU, -minimapV, 0), new Vector3(1, 0, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.x, position.y + Height, 0), playerPosition + new Vector3(-minimapU, minimapV, 0), new Vector3(0, 1, 0)),
                new VertexPositionNormalTextureHue(new Vector3(position.x + Width, position.y + Height, 0), playerPosition + new Vector3(minimapU, minimapV, 0), new Vector3(1, 1, 0))
            };

            spriteBatch.DrawSprite(_gumpTexture, v, Techniques.MiniMap);

            _timeMS += (float)frameMS;
            if (_timeMS >= ReticleBlinkMS)
            {
                if (_playerIndicator == null)
                {
                    _playerIndicator = new Texture2DInfo(spriteBatch.GraphicsDevice, 1, 1);
                    _playerIndicator.SetData(new uint[1] { 0xFFFFFFFF });
                }
                spriteBatch.Draw2D(_playerIndicator, new Vector3(position.x + Width / 2, position.y + Height / 2 - 8, 0), Vector3.zero);
            }
            if (_timeMS >= ReticleBlinkMS * 2)
                _timeMS -= ReticleBlinkMS * 2;
        }

        protected override void OnMouseDoubleClick(int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
                MiniMap_LargeFormat = !MiniMap_LargeFormat;
        }
    }
}
