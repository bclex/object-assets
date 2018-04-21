using OA.Core.UI.Html.Styles;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Core.UI.Html.Elements
{
    /// <summary>
    /// Blocks fit their content. They can be assigned width, height, and alignment.
    /// </summary>
    class BlockElement : AElement
    {
        public List<AElement> Children = new List<AElement>();
        public BlockElement Parent;

        public string Tag;

        RectInt _area;
        public RectInt Area
        {
            get { return _area; }
            set { _area = value; }
        }

        public override int Width
        {
            get { return _area.width; }
            set { _area.width = value; }
        }

        public override int Height
        {
            get { return _area.height; }
            set { _area.height = value; }
        }

        public Alignments Alignment = Alignments.Default;

        public int Layout_MinWidth;
        public int Layout_MaxWidth;

        public BlockElement(string tag, StyleState style)
            : base(style)
        {
            Tag = tag;
        }

        public void AddAtom(AElement atom)
        {
            Children.Add(atom);
            if (atom is BlockElement)
                (atom as BlockElement).Parent = this;
        }

        public override string ToString() => Tag;

        public bool Err_Cant_Fit_Children;
    }
}
