using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;
using System.Collections.Generic;

namespace OA.Ultima.Network.Server
{
    public class ObjectPropertyListPacket : RecvPacket
    {
        readonly Serial _serial;
        readonly int _hash;
        readonly List<int> _clilocs;
        readonly List<string> _arguments;

        public Serial Serial
        {
            get { return _serial; }
        }
        
        public int Hash
        {
            get { return _hash; }
        }
        
        public List<int> CliLocs
        {
            get { return _clilocs; }
        }
        
        public List<string> Arguements
        {
            get { return _arguments; }
        }
        
        public ObjectPropertyListPacket(PacketReader reader)
            : base(0xD6, "Object Property List")
        {
            reader.ReadInt16(); // Always 0x0001
            _serial = reader.ReadInt32();
            reader.ReadInt16(); // Always 0x0000
            _hash = reader.ReadInt32();
            _clilocs = new List<int>();
            _arguments = new List<string>();
            // Loop of all the item/creature's properties to display in the order to display them. The name is always the first entry.
            var clilocId = reader.ReadInt32();
            while (clilocId != 0)
            {
                _clilocs.Add(clilocId);
                var textLength = reader.ReadUInt16();
                var args = string.Empty;
                if (textLength > 0)
                    args = reader.ReadUnicodeStringReverse(textLength / 2);
                _arguments.Add(args);
                clilocId = reader.ReadInt32();
            }
        }
    }
}
