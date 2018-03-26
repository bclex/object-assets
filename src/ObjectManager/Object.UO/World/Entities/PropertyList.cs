using System.Collections.Generic;
using System.Text;

namespace OA.Ultima.World.Entities
{
    public class PropertyList
    {
        public int Hash;
        readonly List<string> _propertyList = new List<string>();

        public bool HasProperties
        {
            get
            {
                if (_propertyList.Count == 0) return false;
                else return true;
            }
        }

        public string Properties
        {
            get
            {
                var b = new StringBuilder();
                for (var i = 0; i < _propertyList.Count; i++)
                {
                    b.Append(_propertyList[i]);
                    if (i < _propertyList.Count - 1)
                        b.Append('\n');
                }
                return b.ToString();
            }
        }

        public void Clear()
        {
            _propertyList.Clear();
        }

        public void AddProperty(string nProperty)
        {
            _propertyList.Add(nProperty);
        }
    }
}
