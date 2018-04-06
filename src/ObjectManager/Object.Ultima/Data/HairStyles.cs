using OA.Core;
using OA.Ultima.Resources;

namespace OA.Ultima.Data
{
    class HairStyles
    {
        static readonly int[] _maleStyles = { 3000340, 3000341, 3000342, 3000343, 3000344, 3000345, 3000346, 3000347, 3000348, 3000349 };
        static string[] _male;
        public static string[] MaleHairNames
        {
            get
            {
                if (_male == null)
                {
                    // get the resource provider
                    var provider = Service.Get<IResourceProvider>();
                    _male = new string[_maleStyles.Length];
                    for (var i = 0; i < _maleStyles.Length; i++)
                    {
                        _male[i] = provider.GetString(_maleStyles[i]);
                        if (_male[i] == "Pigtails")
                            _male[i] = "2 Tails";
                    }
                }
                return _male;
            }
        }
        static readonly int[] _maleIDs = { 0, 8251, 8252, 8253, 8260, 8261, 8266, 8263, 8264, 8265 };
        public static int[] MaleIDs
        {
            get { return _maleIDs; }
        }
        static readonly int[] _maleIDsForCreation = { 0, 1875, 1876, 1879, 1877, 1871, 1874, 1873, 1880, 1870 };
        public static int MaleGumpIDForCharacterCreationFromItemID(int id)
        {
            int gumpID = 0;
            for (var i = 0; i < _maleIDsForCreation.Length; i++)
                if (_maleIDs[i] == id)
                    gumpID = _maleIDsForCreation[i];
            return gumpID;
        }

        static readonly int[] _facialStyles = { 3000340, 3000351, 3000352, 3000353, 3000354, 1011060, 1011061, 3000357 };
        static string[] _facial;
        public static string[] FacialHair
        {
            get
            {
                if (_facial == null)
                {
                    // get the resource provider
                    var provider = Service.Get<IResourceProvider>();
                    _facial = new string[_facialStyles.Length];
                    for (var i = 0; i < _facialStyles.Length; i++)
                        _facial[i] = provider.GetString(_facialStyles[i]);
                }
                return _facial;
            }
        }
        static readonly int[] _facialIDs = { 0, 8256, 8254, 8255, 8257, 8267, 8268, 8269 };
        public static int[] FacialHairIDs
        {
            get { return _facialIDs; }
        }
        static readonly int[] m_facialGumpIDsForCreation = { 0, 1881, 1883, 1885, 1884, 1886, 1882, 1887 };
        public static int FacialHairGumpIDForCharacterCreationFromItemID(int id)
        {
            int gumpID = 0;
            for (var i = 0; i < m_facialGumpIDsForCreation.Length; i++)
                if (_facialIDs[i] == id)
                    gumpID = m_facialGumpIDsForCreation[i];
            return gumpID;
        }

        static readonly int[] _femaleStyles = { 3000340, 3000341, 3000342, 3000343, 3000344, 3000345, 3000346, 3000347, 3000349, 3000350 };
        static string[] _female;
        public static string[] FemaleHairNames
        {
            get
            {
                if (_female == null)
                {
                    // get the resource provider
                    var provider = Service.Get<IResourceProvider>();
                    _female = new string[_femaleStyles.Length];
                    for (var i = 0; i < _femaleStyles.Length; i++)
                        _female[i] = provider.GetString(_femaleStyles[i]);
                }
                return _female;
            }
        }
        static readonly int[] _femaleIDs = { 0, 8251, 8252, 8253, 8260, 8261, 8266, 8263, 8265, 8262 };
        public static int[] FemaleIDs
        {
            get { return _femaleIDs; }
        }
        static readonly int[] _femaleIDsForCreation = { 0, 1847, 1842, 1845, 1843, 1844, 1840, 1839, 1836, 1841 };
        public static int FemaleGumpIDForCharacterCreationFromItemID(int id)
        {
            int gumpID = 0;
            for (var i = 0; i < _femaleIDsForCreation.Length; i++)
                if (_femaleIDs[i] == id)
                    gumpID = _femaleIDsForCreation[i];
            return gumpID;
        }
    }
}