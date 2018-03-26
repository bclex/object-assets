using System.Collections.Generic;

namespace OA.Ultima.World.Data
{
    class CustomHousing
    {
        static readonly Dictionary<Serial, CustomHouse> _customHouses = new Dictionary<Serial, CustomHouse>();

        public static bool IsHashCurrent(Serial serial, int hash)
        {
            if (_customHouses.ContainsKey(serial))
            {
                var h = _customHouses[serial];
                return (h.Hash == hash);
            }
            return false;
        }

        public static CustomHouse GetCustomHouseData(Serial serial) => _customHouses[serial];

        public static void UpdateCustomHouseData(Serial serial, int hash, int planecount, CustomHousePlane[] planes)
        {
            CustomHouse house;
            if (_customHouses.ContainsKey(serial))
                house = _customHouses[serial];
            else
            {
                house = new CustomHouse(serial);
                _customHouses.Add(serial, house);
            }
            house.Update(hash, planecount, planes);
        }
    }
}