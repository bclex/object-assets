using OA.Core.UI.Html.Styles;
using UnityEngine;

namespace OA.Core.UI.Html
{
    public class HtmlLink
    {
        public RectInt Area;
        public int Index;
        public string HREF;
        public StyleState Style;

        public HtmlLink(int i, StyleState style)
        {
            Area = new RectInt();
            Index = i;
            HREF = style.HREF;
            Style = style;
        }
    }
}
