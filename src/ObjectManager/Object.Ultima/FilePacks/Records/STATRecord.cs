namespace OA.Ultima.FilePacks.Records
{
    public class STATRecord : Record
    {
        public readonly short ItemId;

        public STATRecord(short itemId)
        {
            ItemId = itemId;
            //var itemData = TileData.ItemData[i];
        }

        public string Name
        {
            get { return $"sta{ItemId}"; }
        }
    }
}