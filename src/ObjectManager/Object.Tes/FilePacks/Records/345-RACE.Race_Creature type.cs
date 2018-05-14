using OA.Core;
using System;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class RACERecord : Record
    {
        public struct RADTField
        {
            public struct SkillBonus
            {
                public int SkillId;
                public int Bonus;
            }
            public SkillBonus[] SkillBonuses;
            public int[] Strength; // (Male/Female)
            public int[] Intelligence;
            public int[] Willpower;
            public int[] Agility;
            public int[] Speed;
            public int[] Endurance;
            public int[] Personality;
            public int[] Luck;
            public float[] Height;
            public float[] Weight;
            public int Flags; // 1 = Playable 2 = Beast Race

            public RADTField(UnityBinaryReader r, uint dataSize)
            {
                SkillBonuses = new SkillBonus[7];
                for (var i = 0; i < SkillBonuses.Length; i++)
                    SkillBonuses[i] = new SkillBonus { SkillId = r.ReadLEInt32(), Bonus = r.ReadLEInt32() };
                Strength = new[] { r.ReadLEInt32(), r.ReadLEInt32() };
                Intelligence = new[] { r.ReadLEInt32(), r.ReadLEInt32() };
                Willpower = new[] { r.ReadLEInt32(), r.ReadLEInt32() };
                Agility = new[] { r.ReadLEInt32(), r.ReadLEInt32() };
                Speed = new[] { r.ReadLEInt32(), r.ReadLEInt32() };
                Endurance = new[] { r.ReadLEInt32(), r.ReadLEInt32() };
                Personality = new[] { r.ReadLEInt32(), r.ReadLEInt32() };
                Luck = new[] { r.ReadLEInt32(), r.ReadLEInt32() };
                Height = new[] { r.ReadLESingle(), r.ReadLESingle() };
                Weight = new[] { r.ReadLESingle(), r.ReadLESingle() };
                Flags = r.ReadLEInt32();
            }
        }

        public override string ToString() => $"RACE: {EDID.Value}";
        public STRVField EDID { get; set; } // Race ID
        public STRVField FNAM; // Race name
        public RADTField RADT; // Race data
        public List<STRVField> NPCSs = new List<STRVField>(); // Special power/ability name
        public STRVField DESC; // Race description

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize)
        {
            switch (type)
            {
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                case "RADT": RADT = new RADTField(r, dataSize); return true;
                case "NPCS": NPCSs.Add(new STRVField(r, dataSize)); return true;
                case "DESC": DESC = new STRVField(r, dataSize); return true;
                default: return false;
            }
        }

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize) => throw new NotImplementedException();
    }
}