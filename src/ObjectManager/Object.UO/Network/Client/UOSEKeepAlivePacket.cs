using OA.Ultima.Core;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    /// <summary>
    /// The legacy client sends this packet every four seconds. Not certain what use it has, but
    /// it serves to keep the connection with the server alive.
    /// </summary>
    public class UOSEKeepAlivePacket : SendPacket
    {
        public UOSEKeepAlivePacket()
            : base(0xBF, "UOSE Keep Alive")
        {
            var value = (byte)Utility.RandomValue(0x20, 0x80);
            Stream.Write((ushort)0x0024);
            Stream.Write((byte)value);
        }
    }
}
