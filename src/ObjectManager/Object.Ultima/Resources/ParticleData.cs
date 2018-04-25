using OA.Core;
using OA.Ultima.Core;

namespace OA.Ultima.Resources
{
    public class ParticleData
    {
        static ParticleData[] _data;

        static ParticleData()
        {
            _data = new ParticleData[] {
                new ParticleData("Explosion", 0x36B0, 0),           // 14000 explosion 1
                new ParticleData("Explosion", 0x36BD, 0),           // 14013 explosion 2
                new ParticleData("Explosion",0x36CB, 0),            // 14027 explosion 3
                new ParticleData("Large Fireball",0x36D4, 0),       // 14036 large fireball
                new ParticleData("Small Fireball",0x36E4, 0),       // 14052 small fireball
                new ParticleData("Fire Snake",0x36F4, 0),           // 14068 fire snake
                new ParticleData("Explosion Ball",0x36FE, 0),       // 14078explosion ball
                new ParticleData("Fire Column",0x3709, 0),          // 14089 fire column
                                                                    // 14106 - display only the ending of fire column - is this actually used?
                new ParticleData("Smoke",0x3728, 0),                // 14120 smoke
                new ParticleData("Fizzle",0x3735, 0),               // 14133 fizzle
                new ParticleData("Sparkle Blue",0x373A, 0),         // 14138 sparkle blue
                new ParticleData("Sparkle Red",0x374A, 0),          // 14154 sparkle red
                new ParticleData("Sparkle Yellow",0x375A, 0),       // 14170 sparkle yellow blue
                new ParticleData("Sparkle Surround",0x376A, 0),     // 14186 sparkle surround
                new ParticleData("Sparkle Planar",0x3779, 0),       // 14201 sparkle planar
                new ParticleData("Death Vortex",0x3789, 0),         // 14217 death vortex (whirlpool on ground?)
                new ParticleData("Magic Arrow",0x379E, 0),          // glowing arrow
                new ParticleData("Small Bolt",0x379F, 0),           // small bolt
                new ParticleData("Field of Blades (Summon)",0x37A0, 0),     // field of blades (summon?)
                new ParticleData("Glow",0x37B9, 0),                 // glow
                new ParticleData("Death Vortex",0x37CC, 0),         // death vortex
                new ParticleData("Field of Blades (Folding)",0x37EB, 0),    // field of blades (folding up)
                new ParticleData("Field of Blades (Unfolding)",0x37F7, 0),  // field of blades (unfolding)
                new ParticleData("Energy",0x3818, 0),               // energy
                new ParticleData("Poison Wall (SW)",0x3914, 0),     // field of poison (facing SW)
                new ParticleData("Poison Wall (SE)",0x3920, 0),     // field of poison (facing SE)
                new ParticleData("Energy Wall (SW)",0x3946, 0),     // field of energy (facing SW)
                new ParticleData("Energy Wall (SE)",0x3956, 0),     // field of energy (facing SE)
                new ParticleData("Paralysis Wall (SW)",0x3967, 0),  // field of paralysis (facing SW, open and close?)
                new ParticleData("Paralysis Wall (SE)",0x3979, 0),  // field of paralysis (Facing SE, open and close?)
                new ParticleData("Fire Wall (SW)",0x398C, 0),       // field of fire (facing SW)
                new ParticleData("Fire Wall (SE)",0x3996, 0),       // field of fire (facing SE)
                new ParticleData("<null>",0x39A0, 0)                // Used to determine the frame length of the preceding effect.
            };
            DetermineParticleLengths();
            if (_defaultData == null)
                _defaultData = _data[0];
        }

        private static void DetermineParticleLengths()
        {
            for (var i = 0; i < _data.Length - 1; i++)
                _data[i].FrameLength = _data[i + 1].ItemID - _data[i].ItemID;
        }

        static ParticleData _defaultData;
        public static ParticleData DefaultEffect
        {
            get { return _defaultData; }
            set { _defaultData = value; }
        }

        public static ParticleData RandomExplosion
        {
            get
            {
                switch (Utility.RandomValue(0, 2))
                {
                    case 0: return Get(0x36B0);
                    case 1: return Get(0x36BD);
                    case 2: return Get(0x36CB);
                    default: return Get(0x36B0);
                }
            }
        }

        public static ParticleData Get(int itemID)
        {
            if (itemID < _data[0].ItemID)
                return null;
            if (itemID >= _data[_data.Length - 1].ItemID)
                return null;
            ParticleData pData = null;
            for (var i = 1; i < _data.Length; i++)
                if (itemID < _data[i].ItemID)
                {
                    pData = _data[i - 1];
                    if (itemID != pData.ItemID)
                        Utils.Error($"Mismatch? Requested particle {itemID}, returning particle {pData.ItemID}.");
                    return _data[i - 1];
                }
            Utils.Error($"Unknown particle effect with ItemID={itemID}");
            return null;
        }

        readonly int _itemID;
        int _frameLength;
        int _speed;
        readonly string _name;

        public int ItemID { get { return _itemID; } }
        public int FrameLength { get { return _frameLength; } set { _frameLength = value; } }
        public int SpeedOffset { get { return _speed; } set { _speed = value; } }
        public string Name { get { return _name; } }

        public ParticleData(string name, int itemID, int speed)
        {
            _name = name;
            _itemID = itemID;
            _speed = speed;
        }
    }
}
