using OA.Ultima.Resources;
using OA.Ultima.World.EntityViews;
using OA.Ultima.World.Maps;

namespace OA.Ultima.World.Entities
{
    public class Ground : AEntity
    {
        // !!! Don't forget to update surrounding Z values - code is in UpdateSurroundingsIfNecessary(map)

        int _landDataID;
        public int LandDataID
        {
            get { return _landDataID; }
        }

        public LandData LandData;

        public bool IsIgnored
        {
            get
            {
                return
                    _landDataID == 2 ||
                    _landDataID == 0x1DB ||
                    (_landDataID >= 0x1AE && _landDataID <= 0x1B5);
            }
        }

        public Ground(int tileID, Map map)
            : base(Serial.Null, map)
        {
            _landDataID = tileID;
            LandData = TileData.LandData[_landDataID & 0x3FFF];
        }

        protected override AEntityView CreateView()
        {
            return new GroundView(this);
        }
    }
}
