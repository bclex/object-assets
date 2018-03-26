using OA.Ultima.World.Entities;
using UnityEngine;

namespace OA.Ultima.World.Input
{
    public class MouseOverItem
    {
        public Vector2Int InTexturePoint;
        public AEntity Entity;

        internal MouseOverItem(Vector2Int inTexturePoint, AEntity entity)
        {
            InTexturePoint = inTexturePoint;
            Entity = entity;
        }
    }
}
