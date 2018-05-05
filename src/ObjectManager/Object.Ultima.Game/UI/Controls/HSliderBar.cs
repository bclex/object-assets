using OA.Core;
using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Resources;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    enum HSliderBarStyle
    {
        MetalWidgetRecessedBar,
        BlueWidgetNoBar
    }

    class HSliderBar : AControl
    {
        Texture2DInfo[] _gumpSliderBackground;
        Texture2DInfo _gumpWidget;

        // we use m_newValue to (a) get delta, (b) so Value only changes once per frame.
        int _newValue, _value;
        public int Value
        {
            get { return _value; }
            set
            {
                _value = _newValue = value;
                if (IsInitialized)
                    RecalculateSliderX();
            }
        }

        private void RecalculateSliderX()
        {
            _sliderX = (int)((float)(BarWidth - _gumpWidget.Width) * ((float)(Value - MinValue) / (float)(MaxValue - MinValue)));
        }

        public int MinValue;
        public int MaxValue;
        public int BarWidth;

        private int _sliderX;
        private HSliderBarStyle Style;

        HSliderBar(AControl parent)
            : base(parent)
        {
            HandlesMouseInput = true;
            _pairedSliders = new List<HSliderBar>();
        }

        public HSliderBar(AControl parent, int x, int y, int width, int minValue, int maxValue, int value, HSliderBarStyle style)
            : this(parent)
        {
            BuildGumpling(x, y, width, minValue, maxValue, value, style);
        }

        void BuildGumpling(int x, int y, int width, int minValue, int maxValue, int value, HSliderBarStyle style)
        {
            Position = new Vector2Int(x, y);
            MinValue = minValue;
            MaxValue = maxValue;
            BarWidth = width;
            Value = value;
            Style = style;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (_gumpWidget == null)
            {
                var provider = Service.Get<IResourceProvider>();
                switch (Style)
                {
                    default:
                    case HSliderBarStyle.MetalWidgetRecessedBar:
                        _gumpSliderBackground = new Texture2DInfo[3];
                        _gumpSliderBackground[0] = provider.GetUITexture(213);
                        _gumpSliderBackground[1] = provider.GetUITexture(214);
                        _gumpSliderBackground[2] = provider.GetUITexture(215);
                        _gumpWidget = provider.GetUITexture(216);
                        break;
                    case HSliderBarStyle.BlueWidgetNoBar:
                        _gumpWidget = provider.GetUITexture(0x845);
                        break;
                }
                Size = new Vector2Int(BarWidth, _gumpWidget.Height);
                RecalculateSliderX();
            }
            modifyPairedValues(_newValue - Value);
            _value = _newValue;
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            if (_gumpSliderBackground != null)
            {
                spriteBatch.Draw2D(_gumpSliderBackground[0], new Vector3(position.x, position.y, 0), Vector3.zero);
                spriteBatch.Draw2DTiled(_gumpSliderBackground[1], new RectInt(position.x + _gumpSliderBackground[0].Width, position.y, BarWidth - _gumpSliderBackground[2].Width - _gumpSliderBackground[0].Width, _gumpSliderBackground[1].Height), Vector3.zero);
                spriteBatch.Draw2D(_gumpSliderBackground[2], new Vector3(position.x + BarWidth - _gumpSliderBackground[2].Width, position.y, 0), Vector3.zero);
            }
            spriteBatch.Draw2D(_gumpWidget, new Vector3(position.x + _sliderX, position.y, 0), Vector3.zero);
            base.Draw(spriteBatch, position, frameMS);
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            if (new RectInt(_sliderX, 0, _gumpWidget.Width, _gumpWidget.Height).Contains(new Vector2Int(x, y)))
                return true;
            return false;
        }

        bool _clicked;
        Vector2Int _clickPosition;

        protected override void OnMouseDown(int x, int y, MouseButton button)
        {
            _clicked = true;
            _clickPosition = new Vector2Int(x, y);
        }

        protected override void OnMouseUp(int x, int y, MouseButton button)
        {
            _clicked = false;
        }

        protected override void OnMouseOver(int x, int y)
        {
            if (_clicked)
            {
                _sliderX = _sliderX + (x - _clickPosition.x);
                if (_sliderX < 0)
                    _sliderX = 0;
                if (_sliderX > BarWidth - _gumpWidget.Width)
                    _sliderX = BarWidth - _gumpWidget.Width;
                _clickPosition = new Vector2Int(x, y);
                if (_clickPosition.x < _gumpWidget.Width / 2)
                    _clickPosition.x = _gumpWidget.Width / 2;
                if (_clickPosition.x > BarWidth - _gumpWidget.Width / 2)
                    _clickPosition.x = BarWidth - _gumpWidget.Width / 2;
                _newValue = (int)(((float)_sliderX / (float)(BarWidth - _gumpWidget.Width)) * (float)((MaxValue - MinValue))) + MinValue;
            }
        }

        readonly List<HSliderBar> _pairedSliders;
        public void PairSlider(HSliderBar s)
        {
            _pairedSliders.Add(s);
        }

        void modifyPairedValues(int delta)
        {
            if (_pairedSliders.Count == 0)
                return;
            var updateSinceLastCycle = true;
            var d = (delta > 0) ? -1 : 1;
            var points = Math.Abs(delta);
            var sliderIndex = Value % _pairedSliders.Count;
            while (points > 0)
            {
                if (d > 0)
                {
                    if (_pairedSliders[sliderIndex].Value < _pairedSliders[sliderIndex].MaxValue)
                    {
                        updateSinceLastCycle = true;
                        _pairedSliders[sliderIndex].Value += d;
                        points--;
                    }
                }
                else
                {
                    if (_pairedSliders[sliderIndex].Value > _pairedSliders[sliderIndex].MinValue)
                    {
                        updateSinceLastCycle = true;
                        _pairedSliders[sliderIndex].Value += d;
                        points--;
                    }
                }
                sliderIndex++;
                if (sliderIndex == _pairedSliders.Count)
                {
                    if (!updateSinceLastCycle)
                        return;
                    updateSinceLastCycle = false;
                    sliderIndex = 0;
                }
            }
        }
    }
}
