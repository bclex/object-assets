using System.Text;

namespace OA.Ultima.Data
{
    public struct SpellDefinition
    {
        public static SpellDefinition EmptySpell = new SpellDefinition();

        public readonly string Name;
        public readonly int ID;
        public readonly int GumpIconID;
        public readonly int GumpIconSmallID;
        public readonly Reagents[] Regs;

        public SpellDefinition(string name, int index, int gumpIconID, params Reagents[] regs)
        {
            Name = name;
            ID = index;
            GumpIconID = gumpIconID;
            GumpIconSmallID = gumpIconID - 0x1298;
            Regs = regs;
        }

        public string CreateReagentListString(string separator)
        {
            var b = new StringBuilder();
            for (var i = 0; i < Regs.Length; i++)
            {
                switch (Regs[i])
                {
                    // britanian reagents
                    case Reagents.BlackPearl: b.Append("Black Pearl"); break;
                    case Reagents.Bloodmoss: b.Append("Bloodmoss"); break;
                    case Reagents.Garlic: b.Append("Garlic"); break;
                    case Reagents.Ginseng: b.Append("Ginseng"); break;
                    case Reagents.MandrakeRoot: b.Append("Mandrake Root"); break;
                    case Reagents.Nightshade: b.Append("Nightshade"); break;
                    case Reagents.SulfurousAsh: b.Append("Sulfurous Ash"); break;
                    case Reagents.SpidersSilk: b.Append("Spiders' Silk"); break;
                    // pagan reagents
                    case Reagents.BatWing: b.Append("Bat Wing"); break;
                    case Reagents.GraveDust: b.Append("Grave Dust"); break;
                    case Reagents.DaemonBlood: b.Append("Daemon Blood"); break;
                    case Reagents.NoxCrystal: b.Append("Nox Crystal"); break;
                    case Reagents.PigIron: b.Append("Pig Iron"); break;
                    default: b.Append("Unknown reagent"); break;
                }
                if (i < Regs.Length - 1)
                    b.Append(separator);
            }
            return b.ToString();
        }
    }
}
