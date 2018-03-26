using OA.Ultima.Core.Network;

namespace OA.Ultima.Network.Server.GeneralInfo
{
    /// <summary>
    /// Subcommand 0x18: The count of map diffs that were received.
    /// As of 6.0.0.0, this is only used to inform the client of the number of active maps.
    /// </summary>
    public class MapDiffInfo : IGeneralInfo
    {
        public readonly int MapCount;
        public readonly int[] MapPatches;
        public readonly int[] StaticPatches;

        public MapDiffInfo(PacketReader reader)
        {
            MapCount = reader.ReadInt32();
            MapPatches = new int[MapCount];
            StaticPatches = new int[MapCount];
            for (var i = 0; i < MapCount; i++)
            {
                StaticPatches[i] = reader.ReadInt32();
                MapPatches[i] = reader.ReadInt32();
            }
        }
    }
}
