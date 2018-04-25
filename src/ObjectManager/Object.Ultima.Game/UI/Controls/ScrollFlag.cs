using OA.Core;
using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Core.UI;
using OA.Ultima.Resources;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    /// <summary>
    /// A base scrollbar with methods that control min, max, and value
    /// </summary>
    class ScrollFlag : AControl, IScrollBar
    {
        Texture2DInfo _gumpSlider;

        int _sliderExtentTop, _sliderExtentHeight;
        float _sliderPosition;
        float _value;
        int _max, _min;

        bool _btnSliderClicked;
        Vector2Int _clickPosition;

        // ============================================================================================================
        // Public properties
        // ============================================================================================================
        public int Value
        {
            get { return (int)_value; }
            set
            {
                _value = value;
                if (_value < MinValue)
                    _value = MinValue;
                if (_value > MaxValue)
                    _value = MaxValue;
            }
        }

        public int MinValue
        {
            get { return _min; }
            set
            {
                _min = value;
                if (_value < _min)
                    _value = _min;
            }
        }

        public int MaxValue
        {
            get { return _max; }
            set
            {
                if (value < 0)
                    value = 0;
                _max = value;
                if (_value > _max)
                    _value = _max;
            }
        }

        // ============================================================================================================
        // Ctor, Initialize, Update, and Draw
        // ============================================================================================================
        public ScrollFlag(AControl parent)
            : base(parent)
        {
            HandlesMouseInput = true;
        }

        public ScrollFlag(AControl parent, int x, int y, int height, int minValue, int maxValue, int value)
            : this(parent)
        {
            Position = new Vector2Int(x, y);
            _sliderExtentTop = y;
            _sliderExtentHeight = height;
            MinValue = minValue;
            MaxValue = maxValue;
            Value = value;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var provider = Service.Get<IResourceProvider>();
            _gumpSlider = provider.GetUITexture(0x0828);
            Size = new Vector2Int(_gumpSlider.width, _gumpSlider.height);
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
            if (MaxValue <= MinValue || MinValue >= MaxValue)
                Value = MaxValue = MinValue;
            _sliderPosition = CalculateSliderYPosition();
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            // draw slider
            if (MaxValue == MinValue) { } // do nothing.
            else spriteBatch.Draw2D(_gumpSlider, new Vector3(position.x - 5, position.y + _sliderPosition, 0), Vector3.zero);
            base.Draw(spriteBatch, position, frameMS);
        }

        private float CalculateSliderYPosition()
        {
            if (!IsInitialized)
                return 0f;
            if (MaxValue - MinValue == 0)
                return 0f;
            return CalculateScrollableArea() * ((_value - MinValue) / (MaxValue - MinValue));
        }

        private float CalculateScrollableArea()
        {
            if (!IsInitialized)
                return 0f;
            return Height - _gumpSlider.height;
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            x -= 5;
            var slider = new RectInt(0, (int)_sliderPosition, _gumpSlider.width, _gumpSlider.height);
            return slider.Contains(new Vector2Int(x, y));
        }

        protected override void OnMouseDown(int x, int y, MouseButton button)
        {
            if (IsPointWithinControl(x, y))
            {
                // clicked on the slider
                _btnSliderClicked = true;
                _clickPosition = new Vector2Int(x, y);
            }
        }

        protected override void OnMouseUp(int x, int y, MouseButton button)
        {
            _btnSliderClicked = false;
        }

        protected override void OnMouseOver(int x, int y)
        {
            if (_btnSliderClicked)
            {
                if (y != _clickPosition.y)
                {
                    var sliderY = _sliderPosition + (y - _clickPosition.y);
                    if (sliderY < 0)
                        sliderY = 0;
                    var scrollableArea = CalculateScrollableArea();
                    if (sliderY > scrollableArea)
                        sliderY = scrollableArea;
                    _clickPosition = new Vector2Int(x, y);
                    _value = ((sliderY / scrollableArea) * (float)((MaxValue - MinValue))) + MinValue;
                    _sliderPosition = sliderY;
                }
            }
        }

        public bool PointWithinControl(int x, int y)
        {
            return IsPointWithinControl(x, y);
        }
    }
}
