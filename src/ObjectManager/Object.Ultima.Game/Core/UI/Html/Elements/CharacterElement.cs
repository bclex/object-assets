using OA.Core.UI.Html.Styles;

namespace OA.Core.UI.Html.Elements
{
    public class CharacterElement : AElement
    {
        public override int Width
        {
            get
            {
                if (Character < 32)
                    return 0;
                var ch = Style.Font.GetCharacter(Character);
                return ch.Width + ch.ExtraWidth + (Style.IsBold ? 1 : 0);
            }
            set { } // does nothing
        }

        public override int Height
        {
            get { return Style.Font.Height; }
            set { } // does nothing
        }

        public override bool CanBreakAtThisAtom
        {
            get { return Character == ' ' || Character == '\n'; }
        }

        public override bool IsThisAtomABreakingSpace
        {
            get { return Character == ' '; }
        }

        public override bool IsThisAtomALineBreak
        {
            get { return Character == '\n'; }
        }

        public char Character;

        public CharacterElement(StyleState style, char c)
            : base(style)
        {
            Character = c;
        }

        public override string ToString()
        {
            if (IsThisAtomALineBreak)
                return @"\n";
            return Character.ToString();
        }
    }
}
