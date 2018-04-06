using OA.Ultima.World.Entities.Items;
using OA.Ultima.World.Entities.Mobiles;
using OA.Ultima.World.EntityViews;
using OA.Ultima.World.Maps;

namespace OA.Ultima.World.Entities.Effects
{
    public class AnimatedItemEffect : AEffect
    {
        public int ItemID;
        public int Duration;

        public AnimatedItemEffect(Map map, int itemID, int hue, int duration)
            : base(map)
        {
            ItemID = (itemID & 0x3fff);
            Hue = hue;
            Duration = duration;
        }
        public AnimatedItemEffect(Map map, AEntity Source, int ItemID, int Hue, int duration)
            : this(map, ItemID, Hue, duration) { SetSource(Source); }
        public AnimatedItemEffect(Map map, int Source, int ItemID, int Hue, int duration)
            : this(map, Source, 0, 0, 0, ItemID, Hue, duration) { }
        public AnimatedItemEffect(Map map, int xSource, int ySource, int zSource, int ItemID, int Hue, int duration)
            : this(map, ItemID, Hue, duration) { SetSource(xSource, ySource, zSource); }
        public AnimatedItemEffect(Map map, int sourceSerial, int xSource, int ySource, int zSource, int ItemID, int Hue, int duration)
            : this(map, ItemID, Hue, duration)
        {
            var zSrcByte = (sbyte)zSource;
            var source = WorldModel.Entities.GetObject<AEntity>(sourceSerial, false);
            if (source != null)
            {
                if (source is Mobile)
                {
                    var mobile = source as Mobile;
                    if ((!mobile.IsClientEntity && !mobile.IsMoving) && (((xSource != 0) || (ySource != 0)) || (zSource != 0)))
                        mobile.Position.Set(xSource, ySource, zSrcByte);
                    SetSource(mobile);
                }
                else if (source is Item)
                {
                    var item = source as Item;
                    if (((xSource != 0) || (ySource != 0)) || (zSource != 0))
                        item.Position.Set(xSource, ySource, zSrcByte);
                    SetSource(item);
                }
                else SetSource(xSource, ySource, zSource);
            }
            else SetSource(xSource, ySource, zSource);
        }

        public override void Update(double frameMS)
        {
            base.Update(frameMS);
            if (FramesActive >= Duration)
                Dispose();
            else
            {
                int x, y, z;
                GetSource(out x, out y, out z);
                Position.Set(x, y, z);
            }
        }

        protected override AEntityView CreateView()
        {
            return new AnimatedItemEffectView(this);
        }

        public override string ToString()
        {
            return string.Format("AnimatedItemEffect");
        }
    }
}
