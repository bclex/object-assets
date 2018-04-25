using OA.Core;
using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Resources;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    public enum ButtonTypes
    {
        Default = 0,
        SwitchPage = 0,
        Activate = 1,
    }

    public class Button : AControl
    {
        const int Gump_Up = 0, Gump_Down = 1, Gump_Over = 2;

        Texture2DInfo[] _gumpTextures = { null, null, null };
        int[] _gumpID = { 0, 0, 0 }; // 0 == up, 1 == down, 2 == additional over state, not sent by the server but can be added for clientside gumps.
        RenderedText _texture;

        public int GumpUpID
        {
            set
            {
                _gumpID[Gump_Up] = value;
                _gumpTextures[Gump_Up] = null;
            }
        }

        public int GumpDownID
        {
            set
            {
                _gumpID[Gump_Down] = value;
                _gumpTextures[Gump_Down] = null;
            }
        }

        public int GumpOverID
        {
            set
            {
                _gumpID[Gump_Over] = value;
                _gumpTextures[Gump_Over] = null;
            }
        }

        public ButtonTypes ButtonType = ButtonTypes.Default;
        public int ButtonParameter;
        public int ButtonID;
        public string Caption = string.Empty;

        public bool MouseDownOnThis => _clicked;

        Button(AControl parent)
            : base(parent)
        {
            HandlesMouseInput = true;
        }

        public Button(AControl parent, string[] arguements)
            : this(parent)
        {
            var x = int.Parse(arguements[1]);
            var y = int.Parse(arguements[2]);
            var gumpID1 = int.Parse(arguements[3]);
            var gumpID2 = int.Parse(arguements[4]);
            var buttonType = int.Parse(arguements[5]);
            var param = int.Parse(arguements[6]);
            var buttonID = 0;
            if (arguements.Length > 7)
                buttonID = int.Parse(arguements[7]);
            BuildGumpling(x, y, gumpID1, gumpID2, (ButtonTypes)buttonType, param, buttonID);
        }

        public Button(AControl parent, int x, int y, int gumpID1, int gumpID2, ButtonTypes buttonType, int param, int buttonID)
            : this(parent)
        {
            BuildGumpling(x, y, gumpID1, gumpID2, buttonType, param, buttonID);
        }

        void BuildGumpling(int x, int y, int gumpID1, int gumpID2, ButtonTypes buttonType, int param, int buttonID)
        {
            Position = new Vector2Int(x, y);
            GumpUpID = gumpID1;
            GumpDownID = gumpID2;
            ButtonType = buttonType;
            ButtonParameter = param;
            ButtonID = buttonID;
            _texture = new RenderedText(string.Empty, 100, true);
        }

        public override void Update(double totalMS, double frameMS)
        {
            for (int i = Gump_Up; i <= Gump_Over; i++)
                if (_gumpID[i] != 0 && _gumpTextures[i] == null)
                {
                    var provider = Service.Get<IResourceProvider>();
                    _gumpTextures[i] = provider.GetUITexture(_gumpID[i]);
                }
            if (Width == 0 && Height == 0 && _gumpTextures[Gump_Up] != null)
                Size = new Vector2Int(_gumpTextures[Gump_Up].width, _gumpTextures[Gump_Up].height);
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            var texture = GetTextureFromMouseState();
            if (Caption != string.Empty)
                _texture.Text = Caption;
            spriteBatch.Draw2D(texture, new RectInt(position.x, position.y, Width, Height), Vector3.zero);
            if (Caption != string.Empty)
            {
                var yoffset = MouseDownOnThis ? 1 : 0;
                _texture.Draw(spriteBatch, new Vector2Int(
                    position.x + (Width - _texture.Width) / 2,
                    position.y + yoffset + (Height - _texture.Height) / 2));
            }
            base.Draw(spriteBatch, position, frameMS);
        }

        Texture2DInfo GetTextureFromMouseState()
        {
            if (MouseDownOnThis && _gumpTextures[Gump_Down] != null)
                return _gumpTextures[Gump_Down];
            if (UserInterface.MouseOverControl == this && _gumpTextures[Gump_Over] != null)
                return _gumpTextures[Gump_Over];
            return _gumpTextures[Gump_Up];
        }

        int GetGumpIDFromMouseState()
        {
            if (MouseDownOnThis && _gumpTextures[Gump_Down] != null)
                return _gumpID[Gump_Down];
            if (UserInterface.MouseOverControl == this && _gumpTextures[Gump_Over] != null)
                return _gumpID[Gump_Over];
            return _gumpID[Gump_Up];
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            var gumpID = GetGumpIDFromMouseState();
            var provider = Service.Get<IResourceProvider>();
            return provider.IsPointInUITexture(gumpID, x, y);
        }

        bool _clicked;

        protected override void OnMouseDown(int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
                _clicked = true;
        }

        protected override void OnMouseUp(int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
                _clicked = false;
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
                switch (ButtonType)
                {
                    case ButtonTypes.SwitchPage:
                        // switch page
                        ChangePage(ButtonParameter);
                        break;
                    case ButtonTypes.Activate:
                        // send response
                        OnButtonClick(ButtonID);
                        break;
                }
        }
    }
}