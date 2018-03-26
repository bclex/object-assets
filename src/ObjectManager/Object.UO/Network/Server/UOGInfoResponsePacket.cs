using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class UOGInfoResponsePacket : RecvPacket
    {
        readonly string _name;
        readonly int _age;
        readonly int _clientCount;
        readonly int _itemCount;
        readonly int _mobileCount;
        readonly string _memory;

        public int Age
        {
            get { return _age; }
        }

        public int ClientCount
        {
            get { return _clientCount; }
        }

        public int ItemCount
        {
            get { return _itemCount; }
        }

        public int MobileCount
        {
            get { return _mobileCount; }
        }

        public string ServerName
        {
            get { return _name; }
        }

        public string Memory
        {
            get { return _memory; }
        }

        public UOGInfoResponsePacket(PacketReader reader)
            : base(0x52, "UOG Information Response")
        {
            var response = reader.ReadString();
            var parts = response.Split(',');
            for (var j = 0; j < parts.Length; j++)
            {
                var keyValue = parts[j].Split('=');
                if (keyValue.Length == 2)
                {
                    var key = keyValue[0].Trim();
                    var value = keyValue[1].Trim();
                    switch (key)
                    {
                        case "Name": _name = value; break;
                        case "Age": _age = int.Parse(value); break;
                        case "Clients": _clientCount = int.Parse(value) - 1; break;
                        case "Items": _itemCount = int.Parse(value); break;
                        case "Chars": _mobileCount = int.Parse(value); break;
                        case "Mem": _memory = value; break;
                    }
                }
            }
        }
    }
}
