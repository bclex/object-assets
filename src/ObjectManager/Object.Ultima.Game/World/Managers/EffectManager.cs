using OA.Core;
using OA.Ultima.World.Entities.Effects;
using System.Collections.Generic;

namespace OA.Ultima.World.Managers
{
    class EffectManager
    {
        WorldModel _model;
        readonly List<AEffect> _effects;

        public EffectManager(WorldModel model)
        {
            _model = model;
            _effects = new List<AEffect>();
        }

        public void Add(GraphicEffectPacket packet)
        {
            var hasHueData = (packet as GraphicEffectHuedPacket != null);
            var hasParticles = (packet as GraphicEffectExtendedPacket != null); // we don't yet handle these.
            if (hasParticles)
                Utils.Warning("Unhandled particles in an effects packet.");
            AEffect effect = null;
            var hue = hasHueData ? ((GraphicEffectHuedPacket)packet).Hue : 0;
            var blend = hasHueData ? (int)((GraphicEffectHuedPacket)packet).BlendMode : 0;
            switch (packet.EffectType)
            {
                case GraphicEffectType.Moving:
                    if (packet.ItemID <= 0)
                        return;
                    effect = new MovingEffect(_model.Map, packet.SourceSerial, packet.TargetSerial,
                        packet.SourceX, packet.SourceY, packet.SourceZ,
                        packet.TargetX, packet.TargetY, packet.TargetZ,
                        packet.ItemID, hue);
                    effect.BlendMode = blend;
                    if (packet.DoesExplode)
                        effect.Children.Add(new AnimatedItemEffect(_model.Map, packet.TargetSerial,
                            packet.TargetX, packet.TargetY, packet.TargetZ,
                            0x36cb, hue, 9));
                    break;
                case GraphicEffectType.Lightning:
                    effect = new LightningEffect(_model.Map, packet.SourceSerial,
                        packet.SourceX, packet.SourceY, packet.SourceZ, hue);
                    break;
                case GraphicEffectType.FixedXYZ:
                    if (packet.ItemID <= 0)
                        return;
                    effect = new AnimatedItemEffect(_model.Map,
                        packet.SourceX, packet.SourceY, packet.SourceZ,
                        packet.ItemID, hue, packet.Duration);
                    effect.BlendMode = blend;
                    break;
                case GraphicEffectType.FixedFrom:
                    if (packet.ItemID <= 0)
                        return;
                    effect = new AnimatedItemEffect(_model.Map, packet.SourceSerial,
                        packet.SourceX, packet.SourceY, packet.SourceZ,
                        packet.ItemID, hue, packet.Duration);
                    effect.BlendMode = blend;
                    break;
                case GraphicEffectType.ScreenFade:
                    Utils.Warning("Unhandled screen fade effect.");
                    break;
                default:
                    Utils.Warning("Unhandled effect.");
                    return;
            }
            if (effect != null)
                Add(effect);
        }

        public void Add(AEffect e)
        {
            _effects.Add(e);
        }

        public void Update(double frameMS)
        {
            for (var i = 0; i < _effects.Count; i++)
            {
                var effect = _effects[i];
                effect.Update(frameMS);
                if (effect.IsDisposed)
                {
                    _effects.RemoveAt(i);
                    i--;
                    if (effect.ChildrenCount > 0)
                        for (var j = 0; j < effect.Children.Count; j++)
                            _effects.Add(effect.Children[j]);
                }
            }
        }
    }
}
