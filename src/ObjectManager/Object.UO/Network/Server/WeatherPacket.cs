using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class WeatherPacket : RecvPacket
    {
        readonly byte _weatherType;
        readonly byte _effectId;
        readonly byte _temperature;

        public byte WeatherType
        {
            get { return _weatherType; }
        }

        public byte NumberOfWeatherEffectsOnScreen
        {
            get { return _effectId; }        
        }

        public byte Temperature 
        {
            get { return _temperature; }
        }

        public WeatherPacket(PacketReader reader)
            : base(0x65, "Set Weather")
        {
            _weatherType = reader.ReadByte();
            _effectId = reader.ReadByte();
            _temperature = reader.ReadByte();
        }
    }
}
