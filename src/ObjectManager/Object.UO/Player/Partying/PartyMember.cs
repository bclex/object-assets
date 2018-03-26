using OA.Ultima.World;
using OA.Ultima.World.Entities.Mobiles;

namespace OA.Ultima.Player.Partying
{
    public class PartyMember
    {
        public readonly Serial Serial;
        string _cachedName;
        public Mobile Mobile => WorldModel.Entities.GetObject<Mobile>(Serial, false);

        public string Name
        {
            get
            {
                var mobile = Mobile;
                if (Mobile != null)
                    _cachedName = Mobile.Name;
                return _cachedName;
            }
        }

        public PartyMember(Serial serial)
        {
            Serial = serial;
            _cachedName = Name;
        }
    }
}
