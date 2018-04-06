using OA.Configuration;
using OA.Ultima.Configuration.Properties;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Ultima.Configuration
{
    public class GumpSettings : ASettingsSection
    {
        /// <summary>
        /// The list of last positions where a given gump type was located.
        /// </summary>
        public Dictionary<string, Vector2Int> LastPositions { get; set; }

        /// <summary>
        /// A list of saved gumps, and data describing the same. These are reloaded when the world is started.
        /// </summary>
        public List<SavedGumpProperty> SavedGumps { get; set; }

        public GumpSettings()
        {
            LastPositions = new Dictionary<string, Vector2Int>();
            SavedGumps = new List<SavedGumpProperty>();
        }

        public Vector2Int GetLastPosition(string gumpID, Vector2Int defaultPosition)
        {
            Vector2Int value;
            if (LastPositions.TryGetValue(gumpID, out value)) return value;
            else return defaultPosition;
        }

        public void SetLastPosition(string gumpID, Vector2Int position)
        {
            if (LastPositions.ContainsKey(gumpID)) LastPositions[gumpID] = position;
            else LastPositions.Add(gumpID, position);
        }
    }
}
