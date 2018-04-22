using OA.Core.UI.Html.Styles;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Core.UI.Html
{
    public class HtmlLinkList
    {
        public static HtmlLinkList Empty => new HtmlLinkList();

        readonly List<HtmlLink> _links = new List<HtmlLink>();

        public HtmlLink this[int index]
        {
            get
            {
                if (_links.Count == 0)
                    return null;
                if (index >= _links.Count)
                    index = _links.Count - 1;
                if (index < 0)
                    index = 0;
                return _links[index];
            }
        }

        public int Count
        {
            get { return _links.Count; }
        }

        public HtmlLink AddLink(StyleState style, RectInt area)
        {
            HtmlLink matched = null;
            foreach (var link in _links)
                if (link.Style.HREF == style.HREF && link.Area.Right == area.Left)
                {
                    var overlap = link.Area.Top < area.Bottom && area.Top < link.Area.Bottom;
                    if (overlap)
                    {
                        matched = link;
                        matched.Area.width = matched.Area.width + area.width;
                        if (matched.Area.y > area.y)
                            matched.Area.y = area.y;
                        if (matched.Area.Bottom < area.Bottom)
                            matched.Area.height += (area.Bottom - matched.Area.Bottom);
                        break;
                    }
                }
            if (matched == null)
            {
                _links.Add(new HtmlLink(_links.Count, style));
                matched = _links[_links.Count - 1];
                matched.Area = area;
            }
            return matched;
        }

        public void Clear()
        {
            _links.Clear();
        }

        public HtmlLink RegionfromPoint(Vector2Int p)
        {
            var index = -1;
            for (var i = 0; i < _links.Count; i++)
                if (_links[i].Area.Contains(p))
                    index = i;
            if (index == -1) return null;
            else return _links[index];
        }

        public HtmlLink Region(int index)
        {
            return _links[index];
        }
    }
}
