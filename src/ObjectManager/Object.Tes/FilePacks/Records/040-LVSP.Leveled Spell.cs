using OA.Core;
using System;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class LVSPRecord : Record
    {
        public struct LVLOField
        {
            public short Level;
            public byte[] Unknown1;
            public int ItemFormId;
            public short Count;
            public byte[] Unknown2;

            public LVLOField(UnityBinaryReader r, uint dataSize)
            {
                Level = r.ReadLEInt16();
                Unknown1 = r.ReadBytes(2);
                ItemFormId = r.ReadLEInt32();
                Count = r.ReadLEInt16();
                Unknown2 = r.ReadBytes(2);
            }
        }

        public override string ToString() => $"LVSP: {EDID.Value}";
        public STRVField EDID { get; set; } // ID
        public BYTEField LVLD; // Chance None?
        public BYTEField LVLF; // Flags
        public List<LVLOField> LVLOs = new List<LVLOField>(); // Number of items in list

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize) => throw new NotImplementedException();
        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "LVLD": LVLD = new BYTEField(r, dataSize); return true;
                case "LVLF": LVLF = new BYTEField(r, dataSize); return true;
                case "LVLO": LVLOs.Add(new LVLOField(r, dataSize)); return true;
                default: return false;
            }
        }
    }
}