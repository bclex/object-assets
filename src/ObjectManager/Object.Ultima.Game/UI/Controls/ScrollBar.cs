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
    class ScrollBar : AControl, IScrollBar
    {
        // ============================================================================================================
        // Private variables
        // ============================================================================================================
        Texture2D[] _gumpUpButton;
        Texture2D[] _gumpDownButton;
        Texture2D[] _gumpBackground;
        Texture2D _gumpSlider;

        float _sliderPosition;
        float _value;
        int _max, _min;

        bool _btnUpClicked;
        bool _btnDownClicked;
        bool _btnSliderClicked;
        Vector2Int _clickPosition;

        float _timeUntilNextClick;
        const float _timeBetweenClicks = 500f;

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

        public Vector2Int ClickPosition { get => _clickPosition; set => _clickPosition = value; }

        // ============================================================================================================
        // Ctors, Initialize, Update, and Draw
        // ============================================================================================================
        public ScrollBar(AControl parent)
            : base(parent)
        {
            HandlesMouseInput = true;
        }

        public ScrollBar(AControl parent, int x, int y, int height, int minValue, int maxValue, int value)
            : this(parent)
        {
            Position = new Vector2Int(x, y);
            MinValue = minValue;
            MaxValue = maxValue;
            Height = height;
            Value = value;
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            var provider = Service.Get<IResourceProvider>();
            _gumpUpButton = new Texture2D[2];
            _gumpUpButton[0] = provider.GetUITexture(251);
            _gumpUpButton[1] = provider.GetUITexture(250);
            _gumpDownButton = new Texture2D[2];
            _gumpDownButton[0] = provider.GetUITexture(253);
            _gumpDownButton[1] = provider.GetUITexture(252);
            _gumpBackground = new Texture2D[3];
            _gumpBackground[0] = provider.GetUITexture(257);
            _gumpBackground[1] = provider.GetUITexture(256);
            _gumpBackground[2] = provider.GetUITexture(255);
            _gumpSlider = provider.GetUITexture(254);
            Size = new Vector2Int(_gumpBackground[0].width, Height);
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
            if (MaxValue <= MinValue || MinValue >= MaxValue)
                Value = MaxValue = MinValue;
            _sliderPosition = CalculateSliderYPosition();
            if (_btnUpClicked || _btnDownClicked)
            {
                if (_timeUntilNextClick <= 0f)
                {
                    _timeUntilNextClick += _timeBetweenClicks;
                    if (_btnUpClicked) Value -= 1;
                    if (_btnDownClicked) Value += 1;
                }
                _timeUntilNextClick -= (float)totalMS;
            }
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            if (Height <= 0)
                return;
            // draw scrollbar background
            var middleHeight = Height - _gumpUpButton[0].height - _gumpDownButton[0].height - _gumpBackground[0].height - _gumpBackground[2].height;
            if (middleHeight > 0)
            {
                spriteBatch.Draw2D(_gumpBackground[0], new Vector3(position.x, position.y + _gumpUpButton[0].height, 0), Vector3.zero);
                spriteBatch.Draw2DTiled(_gumpBackground[1], new RectInt(position.x, position.y + _gumpUpButton[0].height + _gumpBackground[0].height, _gumpBackground[0].width, middleHeight), Vector3.zero);
                spriteBatch.Draw2D(_gumpBackground[2], new Vector3(position.x, position.y + Height - _gumpDownButton[0].height - _gumpBackground[2].height, 0), Vector3.zero);
            }
            else
            {
                middleHeight = Height - _gumpUpButton[0].height - _gumpDownButton[0].height;
                spriteBatch.Draw2DTiled(_gumpBackground[1], new RectInt(position.x, position.y + _gumpUpButton[0].height, _gumpBackground[0].width, middleHeight), Vector3.zero);
            }
            // draw up button
            spriteBatch.Draw2D(_btnUpClicked ? _gumpUpButton[1] : _gumpUpButton[0], new Vector3(position.x, position.y, 0), Vector3.zero);
            // draw down button
            spriteBatch.Draw2D(_btnDownClicked ? _gumpDownButton[1] : _gumpDownButton[0], new Vector3(position.x, position.y + Height - _gumpDownButton[0].height, 0), Vector3.zero);
            // draw slider
            if (MaxValue > MinValue && middleHeight > 0)
                spriteBatch.Draw2D(_gumpSlider, new Vector3(position.x + (_gumpBackground[0].width - _gumpSlider.width) / 2, position.y + _gumpUpButton[0].height + _sliderPosition, 0), Vector3.zero);
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
            return Height - _gumpUpButton[0].height - _gumpDownButton[0].height - _gumpSlider.height;
        }

        protected override void OnMouseDown(int x, int y, MouseButton button)
        {
            _timeUntilNextClick = 0f;
            if (new RectInt(0, Height - _gumpDownButton[0].height, _gumpDownButton[0].width, _gumpDownButton[0].height).Contains(new Vector2Int(x, y)))
            {
                // clicked on the down button
                _btnDownClicked = true;
            }
            else if (new RectInt(0, 0, _gumpUpButton[0].width, _gumpUpButton[0].height).Contains(new Vector2Int(x, y)))
            {
                // clicked on the up button
                _btnUpClicked = true;
            }
            else if (new RectInt((_gumpBackground[0].width - _gumpSlider.width) / 2, _gumpUpButton[0].height + (int)_sliderPosition, _gumpSlider.width, _gumpSlider.height).Contains(new Vector2Int(x, y)))
            {
                // clicked on the slider
                _btnSliderClicked = true;
                ClickPosition = new Vector2Int(x, y);
            }
            else
            {
                // clicked on the bar. This should scroll up a full slider's height worth of entries.
                // not coded yet, obviously.
            }
        }

        protected override void OnMouseUp(int x, int y, MouseButton button)
        {
            _btnUpClicked = false;
            _btnDownClicked = false;
            _btnSliderClicked = false;
        }

        protected override void OnMouseOver(int x, int y)
        {
            if (_btnSliderClicked)
            {
                if (y != ClickPosition.y)
                {
                    var sliderY = _sliderPosition + (y - ClickPosition.y);
                    if (sliderY < 0)
                        sliderY = 0;
                    var scrollableArea = CalculateScrollableArea();
                    if (sliderY > scrollableArea)
                        sliderY = scrollableArea;
                    ClickPosition = new Vector2Int(x, y);
                    if (sliderY == 0 && ClickPosition.y < _gumpUpButton[0].height + _gumpSlider.height / 2)
                        ClickPosition.y = _gumpUpButton[0].height + _gumpSlider.height / 2;
                    if (sliderY == (scrollableArea) && ClickPosition.y > Height - _gumpDownButton[0].height - _gumpSlider.height / 2)
                        ClickPosition.y = Height - _gumpDownButton[0].height - _gumpSlider.height / 2;
                    _value = ((sliderY / scrollableArea) * (float)((MaxValue - MinValue))) + MinValue;
                    _sliderPosition = sliderY;
                }
            }
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            var bounds = new RectInt(0, 0, Width, Height);
            if (bounds.Contains(new Vector2Int(x, y)))
                return true;
            return false;
        }

        public bool PointWithinControl(int x, int y)
        {
            return IsPointWithinControl(x, y);
        }
    }
}
