using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class DisconnectNotificationPacket : SendPacket
    {
        public DisconnectNotificationPacket(Serial followed, Serial follower)
            : base(0x15, "Disconnect Notification", 9)
        {
            Stream.Write(followed);
            Stream.Write(follower);
        }
    }
}
