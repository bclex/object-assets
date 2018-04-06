namespace OA.Ultima.Network.Server
{
    public struct EquipmentEntry
    {
        public Serial Serial;
        public ushort GumpId;
        public byte Layer;
        public ushort Hue;

        public EquipmentEntry(Serial serial, ushort gumpId, byte layer, ushort hue)
        {
            Serial = serial;
            GumpId = gumpId;
            Layer = layer;
            Hue = hue;
        }
    }
}
