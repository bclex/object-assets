using OA.Core;
using OA.Ultima.World.Entities.Effects;
using OA.Ultima.World.Entities.Items.Containers;
using OA.Ultima.World.Entities.Mobiles;
using OA.Ultima.World.EntityViews;
using UnityEngine;

namespace OA.Ultima.World.Entities
{
    public class DeferredEntity : AEntity
    {
        Vector3 _drawPosition;
        AEntityView _baseView;

        public DeferredEntity(AEntity entity, Vector3 drawPosition, int z)
            : base(entity.Serial, entity.Map)
        {
            _baseView = GetBaseView(entity);
            _drawPosition = drawPosition;
            Position.Set(int.MinValue, int.MinValue, z);
        }

        AEntityView GetBaseView(AEntity entity)
        {
            if (entity is Mobile) return (MobileView)entity.GetView();
            if (entity is Corpse) return (MobileView)entity.GetView();
            if (entity is LightningEffect) return (LightningEffectView)entity.GetView();
            if (entity is AnimatedItemEffect) return (AnimatedItemEffectView)entity.GetView();
            if (entity is MovingEffect) return (MovingEffectView)entity.GetView();
            Utils.Critical("Cannot defer this type of object.");
            return null;
        }

        protected override AEntityView CreateView()
        {
            return new DeferredView(this, _drawPosition, _baseView);
        }

        public override string ToString() => $"{base.ToString()} | deferred";
    }
}
