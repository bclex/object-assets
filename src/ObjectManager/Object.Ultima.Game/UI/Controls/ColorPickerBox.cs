using OA.Core;
using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Resources;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    class ColorPickerBox : AControl
    {
        protected Texture2DInfo _huesTexture;
        protected Texture2DInfo _selectedIndicator;
        protected RectInt _openArea;

        protected int _hueWidth, _hueHeight;
        protected int[] _hues;
        protected int _index;

        protected ColorPickerBox _childColorPicker;
        private Texture2DInfo _inactive;

        public int Index { get; set; }

        public bool IsChild;
        public ColorPickerBox ParentColorPicker;

        public int HueValue
        {
            get { return _hues[Index]; }
            set
            {
                for (var i = 0; i < _hues.Length; i++)
                    if (value == _hues[i])
                    {
                        Index = i;
                        break;
                    }
            }
        }

        ColorPickerBox(AControl parent)
            : base(parent)
        {
            HandlesMouseInput = true;
        }

        public ColorPickerBox(AControl parent, RectInt area, int swatchWidth, int swatchHeight, int[] hues)
            : this(parent)
        {
            BuildGumpling(area, swatchWidth, swatchHeight, hues);
        }

        public ColorPickerBox(AControl parent, RectInt closedArea, RectInt openArea, int swatchWidth, int swatchHeight, int[] hues, int index)
            : this(parent)
        {
            _openArea = openArea;
            _index = index;
            BuildGumpling(closedArea, swatchWidth, swatchHeight, hues);
        }

        void BuildGumpling(RectInt area, int swatchWidth, int swatchHeight, int[] hues)
        {
            _hueWidth = swatchWidth;
            _hueHeight = swatchHeight;
            Position = new Vector2Int(area.x, area.y);
            Size = new Vector2Int(area.width, area.height);
            _hues = hues;
            Index = _index;
        }

        protected override void OnInitialize()
        {
            if (_huesTexture == null)
            {
                if (IsChild) // is a child
                {
                    _huesTexture = HueData.CreateHueSwatch(_hueWidth, _hueHeight, _hues);
                    var provider = Service.Get<IResourceProvider>();
                    _selectedIndicator = provider.GetUITexture(6000);
                }
                else
                {
                    _huesTexture = HueData.CreateHueSwatch(1, 1, new int[1] { _hues[Index] });
                }
            }
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            IResourceProvider provider = Service.Get<IResourceProvider>();
            _inactive = provider.GetUITexture(210);

            spriteBatch.Draw2D(_inactive, new Vector3(position.x, position.y, 0), Vector3.zero);
            spriteBatch.Draw2D(_huesTexture, new RectInt(position.x+3, position.y+3, Width-2, Height-1), Vector3.zero);

            if (IsChild && IsMouseOver)
            {
                spriteBatch.Draw2D(_selectedIndicator, new Vector3(
                    (int)(position.x + (float)(Width / _hueWidth) * ((Index % _hueWidth) + 0.5f) - _selectedIndicator.width / 2),
                    (int)(position.y + (float)(Height / _hueHeight) * ((Index / _hueWidth) + 0.5f) - _selectedIndicator.height / 2),
                    0), Vector3.zero);
            }
            base.Draw(spriteBatch, position, frameMS);
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            if (IsChild) // is a child
            {
                ParentColorPicker.Index = this.Index;
                ParentColorPicker.CloseChildPicker();
            }
            else
            {
                if (_childColorPicker == null)
                {
                    _childColorPicker = new ColorPickerBox(this.Parent, _openArea, _hueWidth, _hueHeight, _hues);
                    _childColorPicker.IsChild = true;
                    _childColorPicker.ParentColorPicker = this;
                    Parent.AddControl(_childColorPicker, this.Page);
                }
                else
                {
                    _childColorPicker.Dispose();
                    _childColorPicker = null;
                }
            }
        }

        protected override void OnMouseOver(int x, int y)
        {
            if (IsChild)
            {
                var clickRow = x / (Width / _hueWidth);
                var clickColumn = y / (Height / _hueHeight);
                ParentColorPicker.Index = Index = clickRow + clickColumn * _hueWidth;
            }
        }

        protected override void OnMouseOut(int x, int y)
        {
        }

        protected void CloseChildPicker()
        {
            if (_childColorPicker != null)
            {
                _childColorPicker.Dispose();
                _childColorPicker = null;
                _huesTexture = HueData.CreateHueSwatch(1, 1, new int[1] { _hues[Index] });
            }
        }
    }
}
