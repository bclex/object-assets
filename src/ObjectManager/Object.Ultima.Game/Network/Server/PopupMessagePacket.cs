using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class PopupMessagePacket : RecvPacket
    {
        public static string[] Messages = {
                "Incorrect password",
                "This character does not exist any more!",
                "This character already exists.",
                "Could not attach to game server.",
                "Could not attach to game server.",
                "A character is already logged in.",
                "Synchronization Error.",
                "You have been idle for to long.",
                "Could not attach to game server.",
                "Character transfer in progress."
            };

        readonly byte _id;

        public string Message
        {
            get { return Messages[_id]; }
        }

        public PopupMessagePacket(PacketReader reader)
            : base(0x53, "Popup Message")
        {
            _id = reader.ReadByte();
        }
    }
}
