﻿using OA.Ultima.Core.Network.Packets;
using OA.Ultima.World.Entities;

namespace OA.Ultima.Network.Client
{
    public class TargetObjectPacket : SendPacket
    {
        public TargetObjectPacket(AEntity entity, int cursorID, byte cursorType)
            : base(0x6C, "Target Object", 19)
        {
            Stream.Write((byte)0x00); // BYTE[1] type: 0x00 = Select Object; 0x01 = Select X, Y, Z
            Stream.Write(cursorID); // BYTE[4] cursorID 
            Stream.Write(cursorType); // BYTE[1] Cursor Type
            Stream.Write((int)entity.Serial); // BYTE[4] Clicked On ID. Not used in this packet.
            Stream.Write((short)entity.X); // BYTE[2] click xLoc
            Stream.Write((short)entity.Y); // BYTE[2] click yLoc
            Stream.Write((byte)0x00); // BYTE unknown (0x00)
            Stream.Write((byte)entity.Z); // BYTE click zLoc
            Stream.Write((short)0); // BYTE[2] model # (if a static tile, 0 if a map/landscape tile)
        }
    }
}
