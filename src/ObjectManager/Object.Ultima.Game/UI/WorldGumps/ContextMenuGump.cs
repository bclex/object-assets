using OA.Core;
using OA.Core.Input;
using OA.Ultima.Core.Network;
using OA.Ultima.Core.UI;
using OA.Ultima.Data;
using OA.Ultima.Network.Client;
using OA.Ultima.Resources;
using OA.Ultima.UI.Controls;
using System.Text;

namespace OA.Ultima.UI.WorldGumps
{
    /// <summary>
    /// A context menu with a number of choices.
    /// </summary>
    class ContextMenuGump : Gump
    {
        readonly ContextMenuData _data;

        readonly ResizePic _background;
        readonly HtmlGumpling _menuItems;

        public ContextMenuGump(ContextMenuData data)
            : base(0, 0)
        {
            MetaData.IsModal = true;
            MetaData.ModalClickOutsideAreaClosesThisControl = true;

            _data = data;

            var provider = Service.Get<IResourceProvider>();
            var font = (AFont)provider.GetUnicodeFont(1);

            _background = (ResizePic)AddControl(new ResizePic(this, 0, 0, 0x0A3C, 50, font.Height * _data.Count + 20));

            var htmlContextItems = new StringBuilder();
            for (var i = 0; i < _data.Count; i++)
                htmlContextItems.Append(string.Format("<a href='{0}' color='#DDD' hovercolor='#FFF' activecolor='#BBB' style='text-decoration:none;'>{1}</a><br/>", _data[i].ResponseCode, _data[i].Caption));
            _menuItems = (HtmlGumpling)AddControl(new HtmlGumpling(this, 10, 10, 200, font.Height * _data.Count, 0, 0, htmlContextItems.ToString()));
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
            _background.Width = _menuItems.Width + 20;
        }

        protected override void OnMouseOut(int x, int y)
        {
            // Dispose();
        }

        public override void OnHtmlInputEvent(string href, MouseEvent e)
        {
            if (e != MouseEvent.Click)
                return;

            int contextMenuItemSelected;
            if (int.TryParse(href, out contextMenuItemSelected))
            {
                var network = Service.Get<INetworkClient>();
                network.Send(new ContextMenuResponsePacket(_data.Serial, (short)contextMenuItemSelected));
                this.Dispose();
            }
        }
    }
}
