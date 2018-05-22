using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class ROADRecord : Record
    {
        public override string ToString() => $"ROAD:";
        public UNKNField PGRP { get; set; }
        public UNKNField PGRR { get; set; }

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "PGRP": PGRP = new UNKNField(r, dataSize); return true;
                case "PGRR": PGRR = new UNKNField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}