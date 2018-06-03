using OA.Core;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class PACKRecord : Record
    {
        public struct PKDTField
        {
            public ushort Flags;
            public byte Type;

            public PKDTField(UnityBinaryReader r, int dataSize)
            {
                Flags = r.ReadLEUInt16();
                Type = r.ReadByte();
                r.SkipBytes(dataSize - 3); // Unused
            }
        }

        public struct PLDTField
        {
            public int Type;
            public uint Target;
            public int Radius;

            public PLDTField(UnityBinaryReader r, int dataSize)
            {
                Type = r.ReadLEInt32();
                Target = r.ReadLEUInt32();
                Radius = r.ReadLEInt32();
            }
        }

        public struct PSDTField
        {
            public byte Month;
            public byte DayOfWeek;
            public byte Date;
            public sbyte Time;
            public int Duration;

            public PSDTField(UnityBinaryReader r, int dataSize)
            {
                Month = r.ReadByte();
                DayOfWeek = r.ReadByte();
                Date = r.ReadByte();
                Time = (sbyte)r.ReadByte();
                Duration = r.ReadLEInt32();
            }
        }

        public struct PTDTField
        {
            public int Type;
            public uint Target;
            public int Count;

            public PTDTField(UnityBinaryReader r, int dataSize)
            {
                Type = r.ReadLEInt32();
                Target = r.ReadLEUInt32();
                Count = r.ReadLEInt32();
            }
        }

        public override string ToString() => $"PACK: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public PKDTField PKDT; // General
        public PLDTField PLDT; // Location
        public PSDTField PSDT; // Schedule
        public PTDTField PTDT; // Target
        public List<SCPTRecord.CTDAField> CTDAs = new List<SCPTRecord.CTDAField>(); // Conditions

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "PKDT": PKDT = new PKDTField(r, dataSize); return true;
                case "PLDT": PLDT = new PLDTField(r, dataSize); return true;
                case "PSDT": PSDT = new PSDTField(r, dataSize); return true;
                case "PTDT": PTDT = new PTDTField(r, dataSize); return true;
                case "CTDA":
                case "CTDT": CTDAs.Add(new SCPTRecord.CTDAField(r, dataSize, format)); return true;
                default: return false;
            }
        }
    }
}