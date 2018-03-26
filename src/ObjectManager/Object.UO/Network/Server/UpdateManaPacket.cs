﻿using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class UpdateManaPacket : RecvPacket
    {
        readonly Serial _serial;
        readonly short _current;
        readonly short _max;

        public Serial Serial
        {
            get { return _serial; } 
        }

        public short Current 
        {
            get { return _current; } 
        }

        public short Max
        {
            get { return _max; }
        }
        
        public UpdateManaPacket(PacketReader reader)
            : base(0xA2, "Update Mana")
        {
            _serial = reader.ReadInt32();
            _max = reader.ReadInt16();
            _current = reader.ReadInt16();
        }
    }
}
