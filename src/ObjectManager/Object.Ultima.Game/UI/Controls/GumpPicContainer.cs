using OA.Core.UI;
using OA.Ultima.World.Entities.Items.Containers;

namespace OA.Ultima.UI.Controls
{
    class GumpPicContainer : GumpPic
    {
        readonly ContainerItem _containerItem;
        public ContainerItem Item { get { return _containerItem; } }

        public GumpPicContainer(AControl parent, int x, int y, int gumpID, int hue, ContainerItem containerItem)
            : base(parent, x, y, gumpID, hue)
        {
            _containerItem = containerItem;
        }
    }
}
