using OA.Core;
using OA.Core.UI;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Core.UI;
using OA.Ultima.Resources;
using OA.Ultima.World;
using UnityEngine;

namespace OA.Ultima.UI
{
    class UltimaCursor : ICursor
    {
        HuedTexture _cursorSprite;
        int _cursorSpriteArtIndex = -1;
        protected Tooltip _tooltip;

        public int CursorSpriteArtIndex
        {
            get { return _cursorSpriteArtIndex; }
            set
            {
                if (value != _cursorSpriteArtIndex)
                {
                    _cursorSpriteArtIndex = value;
                    var provider = Service.Get<IResourceProvider>();
                    var art = provider.GetItemTexture(_cursorSpriteArtIndex);
                    if (art == null)
                        _cursorSprite = null; // shouldn't we have a debug texture to show that we are missing this cursor art? !!!
                    else
                    {
                        var sourceRect = new RectInt(1, 1, art.width - 2, art.height - 2);
                        _cursorSprite = new HuedTexture(art, Vector2Int.zero, sourceRect, 0);
                    }
                }
            }
        }

        public Vector2Int CursorOffset { get; protected set; }

        public int CursorHue { get; protected set; }

        UserInterfaceService _userInterface;

        public UltimaCursor()
        {
            _userInterface = Service.Get<UserInterfaceService>();
        }

        public virtual void Dispose()
        {
            _userInterface = null;
        }

        public virtual void Update()
        {
        }

        protected virtual void BeforeDraw(SpriteBatchUI spriteBatch, Vector2Int position)
        {
            // Over the interface or not in world. Display a default cursor.
            var artworkIndex = 8305;
            if (WorldModel.IsInWorld && WorldModel.Entities.GetPlayerEntity().Flags.IsWarMode)
                artworkIndex -= 23; // if in warmode, show the red-hued cursor.
            CursorSpriteArtIndex = artworkIndex;
            CursorOffset = new Vector2Int(-1, 1);
        }

        public void Draw(SpriteBatchUI spriteBatch, Vector2Int position)
        {
            BeforeDraw(spriteBatch, position);
            if (_cursorSprite != null)
            {
                _cursorSprite.Hue = CursorHue;
                _cursorSprite.Offset = CursorOffset;
                _cursorSprite.Draw(spriteBatch, position);
            }
            DrawTooltip(spriteBatch, position);
        }

        protected virtual void DrawTooltip(SpriteBatchUI spritebatch, Vector2Int position)
        {
            if (_userInterface.IsMouseOverUI && _userInterface.MouseOverControl != null && _userInterface.MouseOverControl.HasTooltip)
            {
                if (_tooltip != null && _tooltip.Caption != _userInterface.MouseOverControl.Tooltip)
                {
                    _tooltip.Dispose();
                    _tooltip = null;
                }
                if (_tooltip == null)
                    _tooltip = new Tooltip(_userInterface.MouseOverControl.Tooltip);
                _tooltip.Draw(spritebatch, position.x, position.y + 24);
            }
            else if (_tooltip != null)
            {
                _tooltip.Dispose();
                _tooltip = null;
            }
        }
    }
}
