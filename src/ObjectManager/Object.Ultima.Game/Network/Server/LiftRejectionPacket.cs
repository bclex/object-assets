using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class LiftRejectionPacket : RecvPacket
    {
        private static string[] _reasons = {
              "Cannot lift the item.",
              "Out of range.",
              "Out of sight.",
              "Belongs to another.",
              "Already holding something.",
              "???"
            };

        readonly byte _errorCode;

        public byte ErrorCode
        {
            get { return _errorCode; }
        }

        public string ErrorMessage
        {
            get { return _reasons[_errorCode]; }
        }

        public LiftRejectionPacket(PacketReader reader)
            : base(0x27, "Request Move Item Request")
        {
            _errorCode = reader.ReadByte();
        }
    }
}
