using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;
using System;

namespace OA.Ultima.Network.Server
{
    // RunUO (Packets.cs:3402)
    public enum LoginRejectionReasons : byte
    {
        InvalidAccountPassword = 0x00,
        AccountInUse = 0x01,
        AccountBlocked = 0x02,
        BadPassword = 0x03,
        IdleExceeded = 0xFE,
        BadCommuncation = 0xFF
    }

    public class LoginRejectionPacket : RecvPacket
    {
        private static Tuple<int, string>[] m_Reasons = {
            new Tuple<int, string>(0x00, "Incorrect username and/or password."), 
            new Tuple<int, string>(0x01, "Someone is already using this account."),
            new Tuple<int, string>(0x02, "Your account has been blocked."),
            new Tuple<int, string>(0x03, "Your account credentials are invalid."),
            new Tuple<int, string>(0xFE, "Login idle period exceeded."),
            new Tuple<int, string>(0xFF, "Communication problem.")
        };
        readonly byte _id;

        public string ReasonText
        {
            get
            {
                for (var i = 0; i < m_Reasons.Length; i++)
                    if (m_Reasons[i].Item1 == _id)
                        return m_Reasons[i].Item2;
                return (m_Reasons[m_Reasons.Length - 1].Item2);
            }
        }

        public LoginRejectionReasons Reason
        {
            get { return (LoginRejectionReasons)_id; }
        }

        public LoginRejectionPacket(PacketReader reader)
            : base(0x82, "Login Rejection")
        {
            _id = reader.ReadByte();
        }
    }
}
