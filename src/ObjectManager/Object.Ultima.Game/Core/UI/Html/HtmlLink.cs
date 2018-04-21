namespace OA.Core.UI.Html
{
    public class HtmlLink
    {
        public Rectangle Area;
        public int Index;
        public string HREF;
        public StyleState Style;

        public HtmlLink(int i, StyleState style)
        {
            Area = new Rectangle();
            Index = i;
            HREF = style.HREF;
            Style = style;
        }
    }
}
