using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;
using OA.Ultima.Data;

namespace OA.Ultima.Network.Server
{
    public class StatusInfoPacket : RecvPacket
    {
        public readonly Serial Serial;
        public readonly string PlayerName;
        public readonly short CurrentHealth;
        public readonly short MaxHealth;
        public readonly bool NameChangeFlag;
        public readonly byte StatusTypeFlag;
        public readonly Genders Sex;
        public readonly short Strength;
        public readonly short Dexterity;
        public readonly short Intelligence;
        public readonly short CurrentStamina;
        public readonly short MaxStamina;
        public readonly short CurrentMana;
        public readonly short MaxMana;
        public readonly int GoldInInventory;
        public readonly short ArmorRating;
        public readonly short Weight;
        public readonly short MaxWeight;
        public readonly Races Race;
        public readonly short StatCap;
        public readonly byte Followers;
        public readonly byte MaxFollowers;
        public readonly short ResistFire;
        public readonly short ResistCold;
        public readonly short ResistPoison;
        public readonly short ResistEnergy;
        public readonly short Luck;
        public readonly short DmgMin;
        public readonly short DmgMax;
        public readonly short TithingPoints;

        public StatusInfoPacket(PacketReader reader)
            : base(0x11, "StatusInfo")
        {
            Serial = reader.ReadInt32();
            PlayerName = reader.ReadString(30);
            CurrentHealth = reader.ReadInt16();
            MaxHealth = reader.ReadInt16();
            NameChangeFlag = reader.ReadByte() != 0x00; // 0x1 = allowed, 0 = not allowed
            StatusTypeFlag = reader.ReadByte();
            if (StatusTypeFlag > 0)
            {
                Sex = (Genders)reader.ReadByte(); // 0=male, 1=female
                Strength = reader.ReadInt16();
                Dexterity = reader.ReadInt16();
                Intelligence = reader.ReadInt16();
                CurrentStamina = reader.ReadInt16();
                MaxStamina = reader.ReadInt16();
                CurrentMana = reader.ReadInt16();
                MaxMana = reader.ReadInt16();
                GoldInInventory = reader.ReadInt32();
                ArmorRating = reader.ReadInt16();
                Weight = reader.ReadInt16();
                if (StatusTypeFlag >= 5)
                {
                    MaxWeight = reader.ReadInt16();
                    Race = (Races)reader.ReadByte();
                }
                if (StatusTypeFlag >= 3)
                {
                    StatCap = reader.ReadInt16();
                    Followers = reader.ReadByte();
                    MaxFollowers = reader.ReadByte();
                }
                if (StatusTypeFlag >= 4)
                {
                    ResistFire = reader.ReadInt16();
                    ResistCold = reader.ReadInt16();
                    ResistPoison = reader.ReadInt16();
                    ResistEnergy = reader.ReadInt16();
                    Luck = reader.ReadInt16();
                    DmgMin = reader.ReadInt16();
                    DmgMax = reader.ReadInt16();
                    TithingPoints = reader.ReadInt16();
                }
            }
        }
    }
}
