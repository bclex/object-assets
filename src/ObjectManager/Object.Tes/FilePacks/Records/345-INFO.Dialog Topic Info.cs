using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class INFORecord : Record
    {
        public struct DATAField
        {
            public int Unknown1;
            public int Disposition;
            public byte Rank; // (0-10)
            public byte Gender; // 0xFF = None, 0x00 = Male, 0x01 = Female
            public byte PCRank; // (0-10)
            public byte Unknown2;

            public DATAField(UnityBinaryReader r, uint dataSize)
            {
                Unknown1 = r.ReadLEInt32();
                Disposition = r.ReadLEInt32();
                Rank = r.ReadByte();
                Gender = r.ReadByte();
                PCRank = r.ReadByte();
                Unknown2 = r.ReadByte();
            }
        }

        public struct SCVRField
        {
            public enum INFOType : byte
            {
                Nothing = 0,
                Function = 1,
                Global = 2,
                Local = 3,
                Journal = 4,
                Item = 5,
                Dead = 6,
                NotID = 7,
                NotFaction = 8,
                NotClass = 9,
                NotRace = 10,
                NotCell = 11,
                NotLocal = 12,
            }

            public byte Index; // (0-5)
            public byte Type;
            public short Function; // (00-71)
                                   //'sX' = Global/Local/Not Local types	
                                   //'JX' = Journal type
                                   //'IX' = Item Type
                                   //'DX' = Dead Type
                                   //'XX' = Not ID Type
                                   //'FX' = Not Faction
                                   //'CX' = Not Class
                                   //'RX' = Not Race
                                   //'LX' = Not Cell
            public byte CompareOp;
            //'0' = '='
            //'1' = '!='
            //'2' = '>'
            //'3' = '>='
            //'4' = '<'
            //'5' = '<='
            public string Name; // Except for the function type, this is the ID for the global/local/etc. Is not nessecarily NULL terminated.The function type SCVR sub-record has

            public SCVRField(UnityBinaryReader r, uint dataSize)
            {
                Index = r.ReadByte();
                Type = r.ReadByte();
                Function = r.ReadLEInt16();
                CompareOp = r.ReadByte();
                Name = r.ReadASCIIString((int)dataSize - 5);
            }
        }

        public override string ToString() => $"INFO: {EDID.Value}";
        public STRVField EDID { get; set; } // Info name string (unique sequence of #'s), ID
        public STRVField PNAM; // Previous info ID
        public STRVField NNAM; // Next info ID (form a linked list of INFOs for the DIAL). First INFO has an empty PNAM, last has an empty NNAM.
        public DATAField DATA; // Info data
        public STRVField ONAM; // Actor
        public STRVField RNAM; // Race
        public STRVField CNAM; // Class
        public STRVField FNAM; // Faction 
        public STRVField ANAM; // Cell
        public STRVField DNAM; // PC Faction
        public STRVField NAME; // The info response string (512 max)
        public FILEField SNAM; // Sound
        public BYTEField QSTN; // Journal Name
        public BYTEField QSTF; // Journal Finished
        public BYTEField QSTR; // Journal Restart
        public SCVRField SCVR; // String for the function/variable choice
        public UNKNField INTV; //
        public UNKNField FLTV; // The function/variable result for the previous SCVR
        public STRVField BNAM; // Result text (not compiled)

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            if (formatId == GameFormatId.Tes3)
                switch (type)
                {
                    case "INAM": EDID = new STRVField(r, dataSize); DIALRecord.LastRecord?.INFOs.Add(this); return true;
                    case "PNAM": PNAM = new STRVField(r, dataSize); return true;
                    case "NNAM": NNAM = new STRVField(r, dataSize); return true;
                    case "DATA": DATA = new DATAField(r, dataSize); return true;
                    case "ONAM": ONAM = new STRVField(r, dataSize); return true;
                    case "RNAM": RNAM = new STRVField(r, dataSize); return true;
                    case "CNAM": CNAM = new STRVField(r, dataSize); return true;
                    case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                    case "ANAM": ANAM = new STRVField(r, dataSize); return true;
                    case "DNAM": DNAM = new STRVField(r, dataSize); return true;
                    case "NAME": NAME = new STRVField(r, dataSize); return true;
                    case "SNAM": SNAM = new FILEField(r, dataSize); return true;
                    case "QSTN": QSTN = new BYTEField(r, dataSize); return true;
                    case "QSTF": QSTF = new BYTEField(r, dataSize); return true;
                    case "QSTR": QSTR = new BYTEField(r, dataSize); return true;
                    case "SCVR": SCVR = new SCVRField(r, dataSize); return true;
                    case "INTV": INTV = new UNKNField(r, dataSize); return true;
                    case "FLTV": FLTV = new UNKNField(r, dataSize); return true;
                    case "BNAM": BNAM = new STRVField(r, dataSize); return true;
                    default: return false;
                }
            return false;
        }
    }
}