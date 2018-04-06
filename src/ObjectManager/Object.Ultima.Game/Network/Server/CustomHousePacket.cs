using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;
using OA.Ultima.World.Data;

namespace OA.Ultima.Network.Server
{
    public class CustomHousePacket : RecvPacket
    {
        readonly Serial _houseSerial;
        public Serial HouseSerial => _houseSerial;

        readonly int _revisionHash;
        public int RevisionHash => _revisionHash;

        readonly int _numPlanes;
        public int PlaneCount => _numPlanes;

        readonly CustomHousePlane[] _planes;
        public CustomHousePlane[] Planes => _planes;

        public CustomHousePacket(PacketReader reader)
            : base(0xD8, "Custom House Packet")
        {
            var CompressionType = reader.ReadByte();
            if (CompressionType != 3)
            {
                _houseSerial = Serial.Null;
                return;
            }
            reader.ReadByte(); // unknown, always 0?
            _houseSerial = reader.ReadInt32();
            _revisionHash = reader.ReadInt32();
            // this is for compression type 3 only
            var bufferLength = reader.ReadInt16();
            var trueBufferLength = reader.ReadInt16();
            _numPlanes = reader.ReadByte();
            // end compression type 3
            _planes = new CustomHousePlane[_numPlanes];
            for (var i = 0; i < _numPlanes; i++)
                _planes[i] = new CustomHousePlane(reader);
        }
    }
}
