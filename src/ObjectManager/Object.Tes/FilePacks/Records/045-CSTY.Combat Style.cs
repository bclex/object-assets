using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class CSTYRecord : Record
    {
        public class CSTDField
        {
            public byte DodgePercentChance;
            public byte LeftRightPercentChance;
            public float DodgeLeftRightTimer_Min;
            public float DodgeLeftRightTimer_Max;
            public float DodgeForwardTimer_Min;
            public float DodgeForwardTimer_Max;
            public float DodgeBackTimer_Min;
            public float DodgeBackTimer_Max;
            public float IdleTimer_Min;
            public float IdleTimer_Max;
            public byte BlockPercentChance;
            public byte AttackPercentChance;
            public float RecoilStaggerBonusToAttack;
            public float UnconsciousBonusToAttack;
            public float HandToHandBonusToAttack;
            public byte PowerAttackPercentChance;
            public float RecoilStaggerBonusToPower;
            public float UnconsciousBonusToPowerAttack;
            public byte PowerAttack_Normal;
            public byte PowerAttack_Forward;
            public byte PowerAttack_Back;
            public byte PowerAttack_Left;
            public byte PowerAttack_Right;
            public float HoldTimer_Min;
            public float HoldTimer_Max;
            public byte Flags1;
            public byte AcrobaticDodgePercentChance;
            public float RangeMult_Optimal;
            public float RangeMult_Max;
            public float SwitchDistance_Melee;
            public float SwitchDistance_Ranged;
            public float BuffStandoffDistance;
            public float RangedStandoffDistance;
            public float GroupStandoffDistance;
            public byte RushingAttackPercentChance;
            public float RushingAttackDistanceMult;
            public uint Flags2;

            public CSTDField(UnityBinaryReader r, uint dataSize)
            {
                //if (dataSize != 124 && dataSize != 120 && dataSize != 112 && dataSize != 104 && dataSize != 92 && dataSize != 84)
                //    DodgePercentChance = 0;
                DodgePercentChance = r.ReadByte();
                LeftRightPercentChance = r.ReadByte();
                r.ReadBytes(2); // Unused
                DodgeLeftRightTimer_Min = r.ReadLESingle();
                DodgeLeftRightTimer_Max = r.ReadLESingle();
                DodgeForwardTimer_Min = r.ReadLESingle();
                DodgeForwardTimer_Max = r.ReadLESingle();
                DodgeBackTimer_Min = r.ReadLESingle();
                DodgeBackTimer_Max = r.ReadLESingle();
                IdleTimer_Min = r.ReadLESingle();
                IdleTimer_Max = r.ReadLESingle();
                BlockPercentChance = r.ReadByte();
                AttackPercentChance = r.ReadByte();
                r.ReadBytes(2); // Unused
                RecoilStaggerBonusToAttack = r.ReadLESingle();
                UnconsciousBonusToAttack = r.ReadLESingle();
                HandToHandBonusToAttack = r.ReadLESingle();
                PowerAttackPercentChance = r.ReadByte();
                r.ReadBytes(3); // Unused
                RecoilStaggerBonusToPower = r.ReadLESingle();
                UnconsciousBonusToPowerAttack = r.ReadLESingle();
                PowerAttack_Normal = r.ReadByte();
                PowerAttack_Forward = r.ReadByte();
                PowerAttack_Back = r.ReadByte();
                PowerAttack_Left = r.ReadByte();
                PowerAttack_Right = r.ReadByte();
                r.ReadBytes(3); // Unused
                HoldTimer_Min = r.ReadLESingle();
                HoldTimer_Max = r.ReadLESingle();
                Flags1 = r.ReadByte();
                AcrobaticDodgePercentChance = r.ReadByte();
                r.ReadBytes(2); // Unused
                if (dataSize == 84) return; RangeMult_Optimal = r.ReadLESingle();
                RangeMult_Max = r.ReadLESingle();
                if (dataSize == 92) return; SwitchDistance_Melee = r.ReadLESingle();
                SwitchDistance_Ranged = r.ReadLESingle();
                BuffStandoffDistance = r.ReadLESingle();
                if (dataSize == 104) return; RangedStandoffDistance = r.ReadLESingle();
                GroupStandoffDistance = r.ReadLESingle();
                if (dataSize == 112) return; RushingAttackPercentChance = r.ReadByte();
                r.ReadBytes(3); // Unused
                RushingAttackDistanceMult = r.ReadLESingle();
                if (dataSize == 120) return; Flags2 = r.ReadLEUInt32();
            }
        }

        public struct CSADField
        {
            public float DodgeFatigueModMult;
            public float DodgeFatigueModBase;
            public float EncumbSpeedModBase;
            public float EncumbSpeedModMult;
            public float DodgeWhileUnderAttackMult;
            public float DodgeNotUnderAttackMult;
            public float DodgeBackWhileUnderAttackMult;
            public float DodgeBackNotUnderAttackMult;
            public float DodgeForwardWhileAttackingMult;
            public float DodgeForwardNotAttackingMult;
            public float BlockSkillModifierMult;
            public float BlockSkillModifierBase;
            public float BlockWhileUnderAttackMult;
            public float BlockNotUnderAttackMult;
            public float AttackSkillModifierMult;
            public float AttackSkillModifierBase;
            public float AttackWhileUnderAttackMult;
            public float AttackNotUnderAttackMult;
            public float AttackDuringBlockMult;
            public float PowerAttFatigueModBase;
            public float PowerAttFatigueModMult;

            public CSADField(UnityBinaryReader r, uint dataSize)
            {
                DodgeFatigueModMult = r.ReadLESingle();
                DodgeFatigueModBase = r.ReadLESingle();
                EncumbSpeedModBase = r.ReadLESingle();
                EncumbSpeedModMult = r.ReadLESingle();
                DodgeWhileUnderAttackMult = r.ReadLESingle();
                DodgeNotUnderAttackMult = r.ReadLESingle();
                DodgeBackWhileUnderAttackMult = r.ReadLESingle();
                DodgeBackNotUnderAttackMult = r.ReadLESingle();
                DodgeForwardWhileAttackingMult = r.ReadLESingle();
                DodgeForwardNotAttackingMult = r.ReadLESingle();
                BlockSkillModifierMult = r.ReadLESingle();
                BlockSkillModifierBase = r.ReadLESingle();
                BlockWhileUnderAttackMult = r.ReadLESingle();
                BlockNotUnderAttackMult = r.ReadLESingle();
                AttackSkillModifierMult = r.ReadLESingle();
                AttackSkillModifierBase = r.ReadLESingle();
                AttackWhileUnderAttackMult = r.ReadLESingle();
                AttackNotUnderAttackMult = r.ReadLESingle();
                AttackDuringBlockMult = r.ReadLESingle();
                PowerAttFatigueModBase = r.ReadLESingle();
                PowerAttFatigueModMult = r.ReadLESingle();
            }
        }

        public override string ToString() => $"CSTY: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public CSTDField CSTD; // Standard
        public CSADField CSAD; // Advanced

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "CSTD": CSTD = new CSTDField(r, dataSize); return true;
                case "CSAD": CSAD = new CSADField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}