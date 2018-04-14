using OA.Core.UI;
using OA.Ultima.World.Entities.Items;

namespace OA.Ultima.UI.Controls
{
    class GumpPicBackpack : GumpPic
    {
        public Item BackpackItem { get; protected set; }

        public GumpPicBackpack(AControl parent, int x, int y, Item backpack)
            : base(parent, x, y, 0xC4F6, 0)
        {
            BackpackItem = backpack;
        }
    }
}
