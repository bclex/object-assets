using OA.Ultima.World.Entities;
using UnityEngine;

namespace OA.Ultima.World.Input
{
    public class MousePicking
    {
        MouseOverItem _overObject;
        MouseOverItem _overGround;

        public AEntity MouseOverObject => _overObject?.Entity;

        public AEntity MouseOverGround => _overGround?.Entity;

        public Vector2Int MouseOverObjectPoint
        {
            get { return _overObject == null ? Vector2Int.zero : _overObject.InTexturePoint; }
        }

        public const PickType DefaultPickType = PickType.PickStatics | PickType.PickObjects;

        public PickType PickOnly { get; set; }

        public Vector2Int Position { get; set; }

        public MousePicking()
        {
            PickOnly = PickType.PickNothing;
        }

        public void UpdateOverEntities(MouseOverList list, Vector2Int mousePosition)
        {
            _overObject = list.GetForemostMouseOverItem(mousePosition);
            _overGround = list.GetForemostMouseOverItem<Ground>(mousePosition);
        }
    }
}
