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
using OA.Ultima.World.Entities;
using OA.Ultima.World.Entities.Items;
using OA.Ultima.World.Entities.Items.Containers;
using OA.Ultima.World.Entities.Mobiles;
using System;
using System.Collections.Generic;

namespace OA.Ultima.UI.WorldGumps
{
    class VendorBuyGump : Gump
    {
        ExpandableScroll _background;
        IScrollBar _scrollBar;
        HtmlGumpling _totalCost;
        Button _okButton;
        int _vendorSerial;
        VendorItemInfo[] _items;
        RenderedTextList _shopContents;

        MouseState _mouseState = MouseState.None;
        int _mouseDownOnIndex;
        double _mouseDownMS;

        public VendorBuyGump(AEntity vendorBackpack, VendorBuyListPacket packet)
            : base(0, 0)
        {
            // sanity checking: don't show buy gumps for empty containers.
            if (!(vendorBackpack is ContainerItem) || ((vendorBackpack as ContainerItem).Contents.Count <= 0) || (packet.Items.Count <= 0))
            {
                base.Dispose();
                return;
            }

            IsMoveable = true;
            // note: original gumplings start at index 0x0870.
            AddControl(_background = new ExpandableScroll(this, 0, 0, 360, false));
            AddControl(new HtmlGumpling(this, 0, 6, 300, 20, 0, 0, " <center><span color='#004' style='font-family:uni0;'>Shop Inventory"));

            _scrollBar = (IScrollBar)AddControl(new ScrollFlag(this));
            AddControl(_shopContents = new RenderedTextList(this, 22, 32, 250, 260, _scrollBar));
            BuildShopContents(vendorBackpack, packet);

            AddControl(_totalCost = new HtmlGumpling(this, 44, 334, 200, 30, 0, 0, string.Empty));
            UpdateEntryAndCost();

            AddControl(_okButton = new Button(this, 220, 333, 0x907, 0x908, ButtonTypes.Activate, 0, 0));
            _okButton.GumpOverID = 0x909;
            _okButton.MouseClickEvent += okButton_MouseClickEvent;

        }

        public override void Dispose()
        {
            if (_okButton != null)
                _okButton.MouseClickEvent -= okButton_MouseClickEvent;
            base.Dispose();
        }

        void okButton_MouseClickEvent(AControl control, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;

            var itemsToBuy = new List<Tuple<int, short>>();
            for (var i = 0; i < _items.Length; i++)
                if (_items[i].AmountToBuy > 0)
                    itemsToBuy.Add(new Tuple<int, short>(_items[i].Item.Serial, (short)_items[i].AmountToBuy));

            if (itemsToBuy.Count == 0)
                return;

            var network = Service.Get<INetworkClient>();
            network.Send(new BuyItemsPacket(_vendorSerial, itemsToBuy.ToArray()));
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

        void BuildShopContents(AEntity vendorBackpack, VendorBuyListPacket packet)
        {
            if (!(vendorBackpack is ContainerItem))
            {
                _shopContents.AddEntry("<span color='#800'>Err: vendorBackpack is not Container.");
                return;
            }

            var contents = (vendorBackpack as ContainerItem);
            var vendor = contents.Parent;
            if (vendor == null || !(vendor is Mobile))
            {
                _shopContents.AddEntry("<span color='#800'>Err: vendorBackpack item does not belong to a vendor Mobile.");
                return;
            }
            _vendorSerial = vendor.Serial;
            _items = new VendorItemInfo[packet.Items.Count];
            for (var i = 0; i < packet.Items.Count; i++)
            {
                var item = contents.Contents[packet.Items.Count - 1 - i];
                if (item.Amount > 0)
                {
                    var cliLocAsString = packet.Items[i].Description;
                    var price = packet.Items[i].Price;

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

                    var html = string.Format(_format, description, price.ToString(), item.DisplayItemID, item.Amount, i);
                    _shopContents.AddEntry(html);

                    _items[i] = new VendorItemInfo(item, description, price, item.Amount);
                }
            }

            // list starts displaying first item.
            _scrollBar.Value = 0;
        }

        const string _format =
            "<right><a href='add={4}'><gumpimg src='0x9CF'/></a><div width='4'/><a href='remove={4}'><gumpimg src='0x9CE'/></a></right>" +
            "<left><itemimg src='{2}'  width='52' height='44'/></left><left><span color='#400'>{0}<br/>{1}gp, {3} available.</span></left>";

        public override void OnHtmlInputEvent(string href, MouseEvent e)
        {
            var hrefs = href.Split('=');
            bool isAdd;
            int index;
            // parse add/remove
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
        }

        void AddItem(int index)
        {
            if (_items[index].AmountToBuy < _items[index].AmountTotal)
                _items[index].AmountToBuy++;
            UpdateEntryAndCost(index);
        }

        void RemoveItem(int index)
        {
            if (_items[index].AmountToBuy > 0)
                _items[index].AmountToBuy--;
            UpdateEntryAndCost(index);
        }

        void UpdateEntryAndCost(int index = -1)
        {
            if (index >= 0)
            {
                _shopContents.UpdateEntry(index, string.Format(_format,
                    _items[index].Description,
                    _items[index].Price.ToString(),
                    _items[index].Item.DisplayItemID,
                    _items[index].AmountTotal - _items[index].AmountToBuy, index));
            }
            var totalCost = 0;
            if (_items != null)
                for (int i = 0; i < _items.Length; i++)
                    totalCost += _items[i].AmountToBuy * _items[i].Price;
            _totalCost.Text = string.Format("<span style='font-family:uni0;' color='#008'>Total: </span><span color='#400'>{0}gp</span>", totalCost);
        }

        class VendorItemInfo
        {
            public readonly Item Item;
            public readonly string Description;
            public readonly int Price;
            public readonly int AmountTotal;
            public int AmountToBuy;

            public VendorItemInfo(Item item, string description, int price, int amount)
            {
                Item = item;
                Description = description;
                Price = price;
                AmountTotal = amount;
                AmountToBuy = 0;
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
