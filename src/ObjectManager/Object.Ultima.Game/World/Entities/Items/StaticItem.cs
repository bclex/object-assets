namespace OA.Ultima.World.Entities.Items
{
    public class StaticItem : Item
    {
        public int SortInfluence;

        public StaticItem(int itemID, int hue,  int sortInfluence, Map map)
            : base(Serial.Null, map)
        {
            ItemID = itemID;
            Hue = hue;
            SortInfluence = sortInfluence;
        }
    }
}
