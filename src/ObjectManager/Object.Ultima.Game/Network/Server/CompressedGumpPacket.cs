using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Compression;
using OA.Ultima.Core.Network.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace OA.Ultima.Network.Server
{
    public class CompressedGumpPacket : RecvPacket
    {
        public readonly int GumpSerial;
        public readonly int GumpTypeID;
        public readonly int X;
        public readonly int Y;
        public readonly string GumpData;
        public readonly string[] TextLines;

        public bool HasData
        {
            get { return GumpData != null; }
        }

        public CompressedGumpPacket(PacketReader reader)
            : base(0xDD, "Compressed Gump")
        {
            GumpSerial = reader.ReadInt32();
            GumpTypeID = reader.ReadInt32();
            X = reader.ReadInt32();
            Y = reader.ReadInt32();
            var compressedLength = reader.ReadInt32() - 4;
            var decompressedLength = reader.ReadInt32();
            var compressedData = reader.ReadBytes(compressedLength);
            var decompressedData = new byte[decompressedLength];
            if (ZlibCompression.Unpack(decompressedData, ref decompressedLength, compressedData, compressedLength) != ZLibError.Okay)
                // Problem decompressing, go ahead and quit.
                return;
            GumpData = Encoding.ASCII.GetString(decompressedData);
            var numTextLines = reader.ReadInt32();
            var compressedTextLength = reader.ReadInt32() - 4;
            var decompressedTextLength = reader.ReadInt32();
            var decompressedText = new byte[decompressedTextLength];
            if (numTextLines > 0 && decompressedTextLength > 0)
            {
                var compressedTextData = reader.ReadBytes(compressedTextLength);
                ZlibCompression.Unpack(decompressedText, ref decompressedTextLength, compressedTextData, compressedTextLength);
                var index = 0;
                var lines = new List<string>();
                for (var i = 0; i < numTextLines; i++)
                {
                    var length = decompressedText[index] * 256 + decompressedText[index + 1];
                    index += 2;
                    var b = new byte[length * 2];
                    Array.Copy(decompressedText, index, b, 0, length * 2);
                    index += length * 2;
                    lines.Add(Encoding.BigEndianUnicode.GetString(b));
                }
                TextLines = lines.ToArray();
            }
            else TextLines = new string[0];
        }
    }
}
