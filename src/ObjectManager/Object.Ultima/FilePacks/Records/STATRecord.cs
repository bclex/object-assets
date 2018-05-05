namespace OA.Ultima.FilePacks.Records
{
    public class STATRecord : Record
    {
        public readonly bool Land;
        public readonly short ItemId;

        public STATRecord(bool land, short itemId)
        {
            Land = land;
            ItemId = itemId;
        }

        public string Name => Land ? $"lnd{ItemId:000}" : $"sta{ItemId:000}";
    }
}