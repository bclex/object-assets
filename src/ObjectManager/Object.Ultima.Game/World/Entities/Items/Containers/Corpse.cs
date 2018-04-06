using OA.Ultima.Core;
using OA.Ultima.Data;
using OA.Ultima.World.EntityViews;
using OA.Ultima.World.Maps;

namespace OA.Ultima.World.Entities.Items.Containers
{
    class Corpse : ContainerItem
    {
        public Serial MobileSerial = 0;

        public float Frame = 0.999f;
        public Body Body { get { return Amount; } }

         Direction m_Facing = Direction.Nothing;
        public Direction Facing
        {
            get { return m_Facing & Direction.FacingMask; }
            set { m_Facing = value; }
        }

        public readonly bool DieForwards;

        public Corpse(Serial serial, Map map)
            : base(serial, map)
        {
            Equipment = new MobileEquipment(this);
            DieForwards = Utility.RandomValue(0, 1) == 0;
        }

        protected override AEntityView CreateView()
        {
            return new MobileView(this);
        }

        public override void Update(double frameMS)
        {
            base.Update(frameMS);
            Frame += ((float)frameMS / 500f);
            if (Frame >= 1f)
                Frame = 0.999f;
        }

        public void PlayDeathAnimation()
        {
            Frame = 0f;
        }

        public MobileEquipment Equipment;

        public override void RemoveItem(Serial serial)
        {
            base.RemoveItem(serial);
            Equipment.RemoveBySerial(serial);
            _onUpdated?.Invoke(this);
        }
    }
}
