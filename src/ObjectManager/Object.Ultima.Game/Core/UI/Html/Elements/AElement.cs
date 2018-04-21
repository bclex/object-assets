using OA.Core.UI.Html.Styles;

namespace OA.Core.UI.Html.Elements
{
    public abstract class AElement
    {
        public abstract int Width { get; set; }
        public abstract int Height { get; set; }

        public int Layout_X;
        public int Layout_Y;

        public virtual bool CanBreakAtThisAtom => true;
        public virtual bool IsThisAtomABreakingSpace => false;
        public virtual bool IsThisAtomALineBreak => false;
        public virtual bool IsThisAtomInternalOnly => false;

        public StyleState Style;

        /// <summary>
        /// Creates a new atom.
        /// </summary>
        /// <param name="openTags">This atom will copy the styles from this parameter.</param>
        public AElement(StyleState style)
        {
            Style = new StyleState(style);
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}
