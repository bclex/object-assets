using OA.Core;
using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.Core;
using OA.Ultima.Core.Network;
using OA.Ultima.Core.UI;
using OA.Ultima.Network.Client;
using OA.Ultima.Network.Server;
using OA.Ultima.Resources;
using OA.Ultima.UI.Controls;
using System;
using System.Collections.Generic;

namespace OA.Ultima.UI.WorldGumps
{
    class VendorSellGump : Gump
    {
        ExpandableScroll _background;
        IScrollBar _scrollBar;
        HtmlGumpling _totalCost;

        int _vendorSerial;
        VendorItemInfo[] _items;
        RenderedTextList _shopContents;

        MouseState _mouseState = MouseState.None;
        int _mouseDownOnIndex;
        double _mouseDownMS;
        Button _okButton;

        public VendorSellGump(VendorSellListPacket packet)
            : base(0, 0)
        {
            IsMoveable = true;
            // note: original gumplings start at index 0x0870.
            AddControl(_background = new ExpandableScroll(this, 0, 0, 360, false));
            AddControl(new HtmlGumpling(this, 0, 6, 300, 20, 0, 0, " <center><span color='#004' style='font-family:uni0;'>My Inventory"));

            _scrollBar = (IScrollBar)AddControl(new ScrollFlag(this));
            AddControl(_shopContents = new RenderedTextList(this, 22, 32, 250, 260, _scrollBar));
            BuildShopContents(packet);

            AddControl(_totalCost = new HtmlGumpling(this, 44, 334, 200, 30, 0, 0, string.Empty));
            UpdateEntryAndCost();

            AddControl(_okButton = new Button(this, 220, 333, 0x907, 0x908, ButtonTypes.Activate, 0, 0));
            _okButton.GumpOverID = 0x909;
            _okButton.MouseClickEvent += okButton_MouseClickEvent;
        }

        public override void Dispose()
        {
            _okButton.MouseClickEvent -= okButton_MouseClickEvent;
            base.Dispose();
        }

        private void okButton_MouseClickEvent(AControl control, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;
            var itemsToBuy = new List<Tuple<int, short>>();
            for (var i = 0; i < _items.Length; i++)
                if (_items[i].AmountToSell > 0)
                    itemsToBuy.Add(new Tuple<int, short>(_items[i].Serial, (short)_items[i].AmountToSell));
            if (itemsToBuy.Count == 0)
                return;
            var network = Service.Get<INetworkClient>();
            network.Send(new SellItemsPacket(_vendorSerial, itemsToBuy.ToArray()));
            this.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            _shopContents.Height = Height - 69;
            base.Update(totalMS, frameMS);
            if (_mouseState != MouseState.None)
            {
                _mouseDownMS += frameMS;
                if (_mouseDownMS >= 350d)
                {
                    _mouseDownMS -= 120d;
                    if (_mouseState == MouseState.MouseDownOnAdd) AddItem(_mouseDownOnIndex);
                    else RemoveItem(_mouseDownOnIndex);
                }
            }
        }

        private void BuildShopContents(VendorSellListPacket packet)
        {
            _vendorSerial = packet.VendorSerial;
            _items = new VendorItemInfo[packet.Items.Length];
            for (var i = 0; i < packet.Items.Length; i++)
            {
                VendorSellListPacket.VendorSellItem item = packet.Items[i];
                if (item.Amount > 0)
                {
                    var cliLocAsString = packet.Items[i].Name;
                    int clilocDescription;
                    string description;
                    if (!(int.TryParse(cliLocAsString, out clilocDescription)))
                        description = cliLocAsString;
                    else
                    {
                        // get the resource provider
                        var provider = Service.Get<IResourceProvider>();
                        description = Utility.CapitalizeAllWords(provider.GetString(clilocDescription));
                    }
                    var html = string.Format(_format, description, item.Price.ToString(), item.ItemID, item.Amount, i);
                    _shopContents.AddEntry(html);
                    _items[i] = new VendorItemInfo(item.ItemSerial, item.ItemID, item.Hue, description, item.Price, item.Amount);
                }
            }

            // list starts displaying first item.
            _scrollBar.Value = 0;
        }

        const string _format =
            "<right><a href='add={4}'><gumpimg src='0x9CF'/></a><div width='4'/><a href='remove={4}'><gumpimg src='0x9CE'/></a></right>" +
            "<left><itemimg src='{2}' width='52' height='44'/></left><left><span color='#400'>{0}<br/>{1}gp, {3} to sell.</span></left>";

        public override void OnHtmlInputEvent(string href, MouseEvent e)
        {
            var hrefs = href.Split('=');
            bool isAdd;
            int index;
            if (hrefs[0] == "add") isAdd = true;
            else if (hrefs[0] == "remove") isAdd = false;
            else
            {
                Utils.Error($"Bad HREF in VendorBuyGump: {href}");
                return;
            }
            // parse item index
            if (!(int.TryParse(hrefs[1], out index)))
            {
                Utils.Error($"Unknown vendor item index in VendorBuyGump: {href}");
                return;
            }
            if (e == MouseEvent.Down)
            {
                if (isAdd) AddItem(index);
                else RemoveItem(index);
                _mouseState = isAdd ? MouseState.MouseDownOnAdd : MouseState.MouseDownOnRemove;
                _mouseDownMS = 0;
                _mouseDownOnIndex = index;
            }
            else if (e == MouseEvent.Up)
                _mouseState = MouseState.None;
            UpdateEntryAndCost(index);
            if (isAdd)
            {
                if (_items[index].AmountToSell < _items[index].AmountTotal)
                    _items[index].AmountToSell++;
            }
            else
            {
                if (_items[index].AmountToSell > 0)
                    _items[index].AmountToSell--;
            }
            UpdateEntryAndCost();
        }

        private void AddItem(int index)
        {
            if (_items[index].AmountToSell < _items[index].AmountTotal)
                _items[index].AmountToSell++;
            UpdateEntryAndCost(index);
        }

        private void RemoveItem(int index)
        {
            if (_items[index].AmountToSell > 0)
                _items[index].AmountToSell--;
            UpdateEntryAndCost(index);
        }

        private void UpdateEntryAndCost(int index = -1)
        {
            if (index >= 0)
            {
                _shopContents.UpdateEntry(index, string.Format(_format,
                   _items[index].Description,
                   _items[index].Price.ToString(),
                   _items[index].ItemID,
                   _items[index].AmountTotal - _items[index].AmountToSell, index));
            }
            var totalCost = 0;
            if (_items != null)
                for (var i = 0; i < _items.Length; i++)
                    totalCost += _items[i].AmountToSell * _items[i].Price;
            _totalCost.Text = string.Format("<span style='font-family:uni0;' color='#008'>Total: </span><span color='#400'>{0}gp</span>", totalCost);
        }

        private class VendorItemInfo
        {
            public readonly Serial Serial;
            public readonly ushort ItemID;
            public readonly ushort Hue;
            public readonly string Description;
            public readonly int Price;
            public readonly int AmountTotal;
            public int AmountToSell;

            public VendorItemInfo(Serial serial, ushort itemID, ushort hue, string description, int price, int amount)
            {
                Serial = serial;
                ItemID = itemID;
                Hue = hue;
                Description = description;
                Price = price;
                AmountTotal = amount;
                AmountToSell = 0;
            }
        }

        enum MouseState
        {
            None,
            MouseDownOnAdd,
            MouseDownOnRemove
        }
    }
}
