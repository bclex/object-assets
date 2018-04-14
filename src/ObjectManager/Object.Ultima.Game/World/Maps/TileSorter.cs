using OA.Ultima.World.Entities;
using OA.Ultima.World.Entities.Effects;
using OA.Ultima.World.Entities.Items;
using OA.Ultima.World.Entities.Mobiles;
using System.Collections.Generic;

namespace OA.Ultima.World.Maps
{
    static class TileSorter
    {
        public static void Sort(List<AEntity> items)
        {
            for (var i = 0; i < items.Count - 1; i++)
            {
                var j = i + 1;
                while (j > 0)
                {
                    var result = Compare(items[j - 1], items[j]);
                    if (result > 0)
                    {
                        var temp = items[j - 1];
                        items[j - 1] = items[j];
                        items[j] = temp;
                    }
                    j--;
                }
            }
        }

        public static int Compare(AEntity x, AEntity y)
        {
            GetSortValues(x, out int xZ, out int xType, out int xThreshold, out int xTiebreaker);
            GetSortValues(y, out int yZ, out int yType, out int yThreshold, out int yTiebreaker);
            xZ += xThreshold;
            yZ += yThreshold;
            var comparison = xZ - yZ;
            if (comparison == 0)
                comparison = xType - yType;
            if (comparison == 0)
                comparison = xThreshold - yThreshold;
            if (comparison == 0)
                comparison = xTiebreaker - yTiebreaker;
            return comparison;
        }

        public static void GetSortValues(AEntity e, out int z, out int type, out int threshold, out int tiebreaker)
        {
            if (e is AEffect)
            {
                var effect = e as AEffect;
                z = effect.Z;
                type = 4;
                threshold = 2;
                tiebreaker = 0;
            }
            else if (e is DeferredEntity)
            {
                var mobile = (DeferredEntity)e;
                z = mobile.Z;
                type = 2;
                threshold = 1;
                tiebreaker = 0;
            }
            else if (e is Mobile)
            {
                var mobile = (Mobile)e;
                z = mobile.Z;
                type = mobile.IsSitting ? 0 : 3;
                threshold = 2;
                tiebreaker = mobile.IsClientEntity ? 0x40000000 : (int)mobile.Serial;
            }
            else if (e is Ground)
            {
                var tile = (Ground)e;
                z = tile.GetView().SortZ;
                type = 0;
                threshold = 0;
                tiebreaker = 0;
            }
            else if (e is StaticItem)
            {
                var item = (StaticItem)e;
                z = item.Z;
                type = 1;
                threshold = (item.ItemData.Height > 0 ? 1 : 0) + (item.ItemData.IsBackground ? 0 : 1);
                tiebreaker = item.SortInfluence;
            }
            else if (e is Item)
            {
                var item = (Item)e;
                z = item.Z;
                type = ((item.ItemID & 0x3fff) == 0x2006) ? 4 : 2; // corpses show on top of mobiles and items.
                threshold = (item.ItemData.Height > 0 ? 1 : 0) + (item.ItemData.IsBackground ? 0 : 1);
                tiebreaker = item.Serial;
            }
            else
            {
                z = 0;
                threshold = 0;
                type = 0;
                tiebreaker = 0;
            }
        }
    }
}
