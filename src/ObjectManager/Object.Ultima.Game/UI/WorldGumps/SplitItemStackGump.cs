using OA.Core;
using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.UI.Controls;
using OA.Ultima.World;
using OA.Ultima.World.Entities.Items;
using UnityEngine;

namespace OA.Ultima.UI.WorldGumps
{
    class SplitItemStackGump : Gump
    {
        public Item Item { get; private set; }

        readonly Vector2Int _pickupOffset;

        readonly HSliderBar _slider;
        TextEntry _amountEntry;
        Button _okButton;
        int _lastValue;

        public SplitItemStackGump(Item item, Vector2Int pickupOffset)
            : base(0, 0)
        {
            Item = item;
            _pickupOffset = pickupOffset;

            IsMoveable = true;

            // Background
            AddControl(new GumpPic(this, 0, 0, 0x085c, 0));
            // Slider
            _slider = (HSliderBar)AddControl(new HSliderBar(this, 30, 16, 104, 0, item.Amount, item.Amount, HSliderBarStyle.BlueWidgetNoBar));
            _lastValue = _slider.Value;
            // Ok button
            AddControl(_okButton = new Button(this, 102, 38, 0x085d, 0x085e, ButtonTypes.Default, 0, 0));
            _okButton.GumpOverID = 0x085f;
            _okButton.MouseClickEvent += ClickOkayButton;
            // Text entry field
            _amountEntry = (TextEntry)AddControl(new TextEntry(this, 30, 39, 60, 16, 0, 0, 5, item.Amount.ToString()));
            _amountEntry.LeadingHtmlTag = "<big>";
            _amountEntry.LegacyCarat = true;
            _amountEntry.Hue = 1001;
            _amountEntry.ReplaceDefaultTextOnFirstKeypress = true;
            _amountEntry.NumericOnly = true;
        }

        public override void Dispose()
        {
            _okButton.MouseClickEvent -= ClickOkayButton;
            base.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            // update children controls first
            base.Update(totalMS, frameMS);

            // update strategy: if slider != last value, then set text equal to slider value. else try parsing text.
            //                  if text is empty, value = minvalue.
            //                  if can't parse text, then set text equal to slider value.
            //                  if can parse text, and text != slider, then set slider = text.
            if (_slider.Value != _lastValue)
                _amountEntry.Text = _slider.Value.ToString();
            else
            {
                int textValue;
                if (_amountEntry.Text.Length == 0)
                    _slider.Value = _slider.MinValue;
                else if (!int.TryParse(_amountEntry.Text, out textValue))
                    _amountEntry.Text = _slider.Value.ToString();
                else
                {
                    if (textValue != _slider.Value)
                    {
                        if (textValue <= _slider.MaxValue)
                            _slider.Value = textValue;
                        else
                        {
                            _slider.Value = _slider.MaxValue;
                            _amountEntry.Text = _slider.Value.ToString();
                        }
                    }
                }
            }
            _lastValue = _slider.Value;
        }

        private void ClickOkayButton(AControl sender, int x, int y, MouseButton button)
        {
            if (_slider.Value > 0)
            {
                var world = Service.Get<WorldModel>();
                world.Interaction.PickupItem(Item, _pickupOffset, _slider.Value);
            }
            Dispose();
        }

        public override void OnKeyboardReturn(int textID, string text)
        {
            if (_slider.Value > 0)
            {
                var world = Service.Get<WorldModel>();
                world.Interaction.PickupItem(Item, _pickupOffset, _slider.Value);
            }
            Dispose();
        }
    }
}
