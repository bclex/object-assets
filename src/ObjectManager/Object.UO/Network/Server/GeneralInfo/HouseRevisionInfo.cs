using OA.Ultima.Core.Network;
using OA.Ultima.Data;

namespace OA.Ultima.Network.Server.GeneralInfo
{
    /// <summary>
    /// Subcommand 0x1D: The revision hash of a custom house.
    /// </summary>
    class HouseRevisionInfo : IGeneralInfo
    {
        public readonly HouseRevisionState Revision;

        public HouseRevisionInfo(PacketReader reader)
        {
            Serial s = reader.ReadInt32();
            var hash = reader.ReadInt32();
            Revision = new HouseRevisionState(s, hash);
        }
    }
}
