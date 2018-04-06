namespace OA.Ultima.Core.Network.Packets
{
    public abstract class RecvPacket : IRecvPacket
    {
        readonly int _id;
        readonly string _name;

        public int Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        public RecvPacket(int id, string name)
        {
            _id = id;
            _name = name;
        }
    }
}
