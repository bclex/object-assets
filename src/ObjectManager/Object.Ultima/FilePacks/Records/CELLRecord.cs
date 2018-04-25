using OA.Core;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Ultima.FilePacks.Records
{
    public class CELLRecord : Record, ICellRecord
    {
        public uint flags;
        public int gridX;
        public int gridY;

        public class RefObjDataGroup
        {
            public string Name
            {
                get { return "Name"; }
            }
        }

        public string Name
        {
            get { return "Name"; }
        }

        public bool IsInterior
        {
            get { return false; }
        }

        public Vector2i GridCoords
        {
            get { return new Vector2i(gridX, gridY); }
        }

        public Color? AmbientLight => null;

        public List<RefObjDataGroup> refObjDataGroups = new List<RefObjDataGroup>();
    }
}