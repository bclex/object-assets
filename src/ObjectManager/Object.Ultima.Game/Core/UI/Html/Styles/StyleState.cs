using OA.Ultima.Core.UI;
using OA.Ultima.Resources;
using UnityEngine;

namespace OA.Core.UI.Html.Styles
{
    public class StyleState
    {
        private StyleValue _isUnderlined = StyleValue.Default;
        public bool IsUnderlined
        {
            set
            {
                if (value) _isUnderlined = StyleValue.True;
                else _isUnderlined = StyleValue.False;
            }
            get
            {
                if (_isUnderlined == StyleValue.False) return false;
                else if (_isUnderlined == StyleValue.True) return true;
                else // _isUnderlined == TagValue.Default
                {
                    if (HREF != null) return true;
                    else return false;
                }
            }
        }

        public bool IsBold;
        public bool IsItalic;
        public bool IsOutlined;

        public bool DrawOutline
        {
            get { return IsOutlined && !Font.HasBuiltInOutline; }
        }

        public IFont Font; // default value set in manager ctor.
        public Color Color = Color.white;
        public int ColorHue = 2;
        public int ActiveColorHue = 12;
        public int HoverColorHue = 24;

        public string HREF;
        public bool IsHREF
        {
            get { return HREF != null; }
        }

        public int ExtraWidth
        {
            get
            {
                var extraWidth = 0;
                if (IsItalic)
                    extraWidth = Font.Height / 2;
                if (DrawOutline)
                    extraWidth += 2;
                return extraWidth;
            }
        }

        public StyleState(IResourceProvider provider)
        {
            Font = provider.GetUnicodeFont((int)Fonts.Default);
        }

        public StyleState(StyleState parent)
        {
            _isUnderlined = parent._isUnderlined;
            IsBold = parent.IsBold;
            IsItalic = parent.IsItalic;
            IsOutlined = parent.IsOutlined;
            Font = parent.Font;
            Color = parent.Color;
            ColorHue = parent.ColorHue;
            ActiveColorHue = parent.ActiveColorHue;
            HoverColorHue = parent.HoverColorHue;
            HREF = parent.HREF;
        }
    }
}
