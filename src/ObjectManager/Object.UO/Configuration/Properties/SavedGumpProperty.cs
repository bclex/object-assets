using System;
using System.Collections.Generic;

namespace OA.Ultima.Configuration.Properties
{
    public struct SavedGumpProperty
    {
        public string GumpType;
        public Dictionary<string, object> GumpData;

        /// <summary>
        /// A description of a gump that has been saved.
        /// </summary>
        /// <param name="gumpType">The gump's type (no namespace)</param>
        /// <param name="gumpData"></param>
        public SavedGumpProperty(Type gumpType, Dictionary<string, object> gumpData)
        {
            GumpType = gumpType.ToString();
            GumpData = gumpData;
        }
    }
}
