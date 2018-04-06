using OA.Ultima.Core.Graphics;
using OA.Ultima.World.EntityViews;
using OA.Ultima.World.Input;
using OA.Ultima.World.Maps;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Ultima.World.WorldViews
{
    static class OverheadsView
    {
        static List<ViewWithDrawInfo> _views = new List<ViewWithDrawInfo>();

        public static void AddView(AEntityView view, Vector3 drawPosition)
        {
            _views.Add(new ViewWithDrawInfo(view, drawPosition));
        }

        public static void Render(SpriteBatch3D spriteBatch, MouseOverList mouseOver, Map map, bool roofHideFlag)
        {
            if (_views.Count > 0)
            {
                for (var i = 0; i < _views.Count; i++)
                    _views[i].View.Draw(spriteBatch, _views[i].DrawPosition, mouseOver, map, roofHideFlag);
                _views.Clear();
            }
        }

        private struct ViewWithDrawInfo
        {
            public readonly AEntityView View;
            public readonly Vector3 DrawPosition;

            public ViewWithDrawInfo(AEntityView view, Vector3 drawPosition)
            {
                View = view;
                DrawPosition = drawPosition;
            }
        }
    }
}
