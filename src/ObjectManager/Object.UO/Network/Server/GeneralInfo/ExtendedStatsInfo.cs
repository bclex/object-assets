using OA.Core;
using OA.Ultima.Core.Network;
using OA.Ultima.Data;

namespace OA.Ultima.Network.Server.GeneralInfo
{
    /// <summary>
    /// Subcommand 0x19: the serial of the mobile which the extended stats must be applied to.
    /// </summary>
    class ExtendedStatsInfo : IGeneralInfo
    {
        public readonly Serial Serial;
        public readonly StatLocks Locks;

        public ExtendedStatsInfo(PacketReader reader)
        {
            var clientFlag = reader.ReadByte(); // (0x2 for 2D client, 0x5 for KR client) 
            Serial = reader.ReadInt32();
            var unknown0 = reader.ReadByte(); // (always 0) 
            var lockFlags = reader.ReadByte();
            // Lock flags = bitflags 00SSDDII 
            //     00 = up
            //     01 = down
            //     10 = locked
            // FF = update mobile status animation ( KR only )
            if (lockFlags != 0xFF)
            {
                var strengthLock = (lockFlags >> 4) & 0x03;
                var dexterityLock = (lockFlags >> 2) & 0x03;
                var inteligenceLock = (lockFlags) & 0x03;
                Locks = new StatLocks(strengthLock, dexterityLock, inteligenceLock);
            }
            if (clientFlag == 5)
            {
                Utils.Warning("ClientFlags == 5 in GeneralInfoPacket ExtendedStats 0x19. This is not a KR client.");
                // If(Lock flags = 0xFF) //Update mobile status animation
                //  BYTE[1] Status // Unveryfied if lock flags == FF the locks will be handled here
                //  BYTE[1] unknown (0x00) 
                //  BYTE[1] Animation 
                //  BYTE[1] unknown (0x00) 
                //  BYTE[1] Frame 
                // else
                //  BYTE[1] unknown (0x00) 
                //  BYTE[4] unknown (0x00000000)
                // endif
            }
        }
    }
}
