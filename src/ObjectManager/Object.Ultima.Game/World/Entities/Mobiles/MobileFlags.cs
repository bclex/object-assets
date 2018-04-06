using System;

namespace OA.Ultima.World.Entities.Mobiles
{
    [Flags]
    public enum MobileFlag
    {
        None = 0x00,
        Female = 0x02,
        Poisoned = 0x04,
        Blessed = 0x08,
        Warmode = 0x40,
        Hidden = 0x80,
    }

    public class MobileFlags
    {
        /// <summary>
        /// These are the only flags sent by RunUO
        /// 0x02 = female
        /// 0x04 = poisoned
        /// 0x08 = blessed/yellow health bar
        /// 0x40 = warmode
        /// 0x80 = hidden
        /// </summary>
        private MobileFlag _flags;

        public bool IsFemale { get { return ((_flags & MobileFlag.Female) != 0); } }
        public bool IsPoisoned { get { return ((_flags & MobileFlag.Poisoned) != 0); } }
        public bool IsBlessed { get { return ((_flags & MobileFlag.Blessed) != 0); } }
        public bool IsWarMode
        {
            get { return ((_flags & MobileFlag.Warmode) != 0); }
            set
            {
                if (value == true) _flags |= MobileFlag.Warmode;
                else _flags &= ~MobileFlag.Warmode;
            }
        }
        public bool IsHidden { get { return ((_flags & MobileFlag.Hidden) != 0); } }

        public MobileFlags(MobileFlag flags)
        {
            _flags = flags;
        }

        public MobileFlags()
        {
            _flags = (MobileFlag)0;
        }
    }
}
