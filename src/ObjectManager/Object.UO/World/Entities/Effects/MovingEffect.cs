using OA.Ultima.World.Entities.Items;
using OA.Ultima.World.Entities.Mobiles;
using OA.Ultima.World.EntityViews;
using OA.Ultima.World.Maps;
using System;
using UnityEngine;

namespace OA.Ultima.World.Entities.Effects
{
    public class MovingEffect : AEffect
    {
        public float AngleToTarget;
        float _timeActive;
        float _timeUntilHit;

        int _itemID;
        public int ItemID
        {
            get { return _itemID; }
        }

        public MovingEffect(Map map, int itemID, int hue)
            : base(map)
        {
            Hue = hue;
            itemID &= 0x3fff;
            _itemID = itemID | 0x4000;
        }
        public MovingEffect(Map map, AEntity Source, AEntity Target, int itemID, int hue)
            : this(map, itemID, hue) { base.SetSource(Source); base.SetTarget(Target); }
        public MovingEffect(Map map, AEntity Source, int xTarget, int yTarget, int zTarget, int itemID, int hue)
            : this(map, itemID, hue) { base.SetSource(Source); base.SetTarget(xTarget, yTarget, zTarget); }
        public MovingEffect(Map map, int xSource, int ySource, int zSource, AEntity Target, int itemID, int hue)
            : this(map, itemID, hue) { base.SetSource(xSource, ySource, zSource); base.SetTarget(Target); }
        public MovingEffect(Map map, int xSource, int ySource, int zSource, int xTarget, int yTarget, int zTarget, int itemID, int hue)
            : this(map, itemID, hue) { base.SetSource(xSource, ySource, zSource); base.SetTarget(xTarget, yTarget, zTarget); }
        public MovingEffect(Map map, int sourceSerial, int targetSerial, int xSource, int ySource, int zSource, int xTarget, int yTarget, int zTarget, int itemID, int hue)
            : this(map, itemID, hue)
        {
            var zSrcByte = (sbyte)zSource;
            var zTarByte = (sbyte)zTarget;
            var source = WorldModel.Entities.GetObject<AEntity>(sourceSerial, false);
            if (source != null)
            {
                if (source is Mobile)
                {
                    base.SetSource(source.X, source.Y, source.Z);
                    var mobile = source as Mobile;
                    if ((!mobile.IsClientEntity && !mobile.IsMoving) && ((xSource | ySource | zSource) != 0))
                        source.Position.Set(xSource, ySource, zSrcByte);
                }
                else if (source is Item)
                {
                    base.SetSource(source.X, source.Y, source.Z);
                    var item = source as Item;
                    if ((xSource | ySource | zSource) != 0)
                        item.Position.Set(xSource, ySource, zSrcByte);
                }
                else base.SetSource(xSource, ySource, zSrcByte);
            }
            else base.SetSource(xSource, ySource, zSource);
            var target = WorldModel.Entities.GetObject<AEntity>(targetSerial, false);
            if (target != null)
            {
                if (target is Mobile)
                {
                    base.SetTarget(target);
                    var mobile = target as Mobile;
                    if ((!mobile.IsClientEntity && !mobile.IsMoving) && ((xTarget | yTarget | zTarget) != 0))
                        mobile.Position.Set(xTarget, yTarget, zTarByte);
                }
                else if (target is Item)
                {
                    base.SetTarget(target);
                    Item item = target as Item;
                    if ((xTarget | yTarget | zTarget) != 0)
                        item.Position.Set(xTarget, yTarget, zTarByte);
                }
                else base.SetSource(xTarget, yTarget, zTarByte);
            }
        }

        public override void Update(double frameMS)
        {
            base.Update(frameMS);
            int sx, sy, sz, tx, ty, tz;
            GetSource(out sx, out sy, out sz);
            GetTarget(out tx, out ty, out tz);
            if (_timeUntilHit == 0f)
            {
                _timeActive = 0f;
                _timeUntilHit = (float)Math.Sqrt(Math.Pow((tx - sx), 2) + Math.Pow((ty - sy), 2) + Math.Pow((tz - sz), 2)) * 75f;
            }
            else _timeActive += (float)frameMS;
            if (_timeActive >= _timeUntilHit)
            {
                Dispose();
                return;
            }
            else
            {
                float x, y, z;
                x = (sx + (_timeActive / _timeUntilHit) * (float)(tx - sx));
                y = (sy + (_timeActive / _timeUntilHit) * (float)(ty - sy));
                z = (sz + (_timeActive / _timeUntilHit) * (float)(tz - sz));
                Position.Set((int)x, (int)y, (int)z);
                Position.Offset = new Vector3(x % 1, y % 1, z % 1);
                AngleToTarget = -((float)Math.Atan2((ty - sy), (tx - sx)) + (float)(Math.PI) * (1f / 4f)); // In radians
            }
            // _renderMode:
            // 2: Alpha = 1.0, Additive.
            // 3: Alpha = 1.5, Additive.
            // 4: Alpha = 0.5, AlphaBlend.
            // draw rotated.
        }

        protected override AEntityView CreateView()
        {
            return new MovingEffectView(this);
        }

        public override string ToString()
        {
            return string.Format("MovingEffect");
        }
    }
}
