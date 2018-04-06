using OA.Core;
using OA.Ultima.Core.Graphics;
using OA.Ultima.World.Entities.Mobiles;
using OA.Ultima.World.EntityViews;
using OA.Ultima.World.WorldViews;

namespace OA.Ultima.World
{
    class WorldView : AView
    {
        bool _showingDeathEffect;
        double _deathEffectTime;
        double _lightingGlobal;
        double _lightingPersonal;
        YouAreDeadGump _youAreDead;
        UserInterfaceService _ui;

        public IsometricRenderer Isometric { get; private set; }

        public MiniMapTexture MiniMap { get; private set; }

        protected new WorldModel Model
        {
            get { return (WorldModel)base.Model; }
        }

        /// <summary>
        ///  When AllLabels is true, all entites should display their name above their object.
        /// </summary>
        public static bool AllLabels { get; set; }

        public static int MouseOverHue = 0x038;

        public WorldView(WorldModel model)
            : base(model)
        {
            Isometric = new IsometricRenderer();
            Isometric.Lighting.LightDirection = -0.6f;
            MiniMap = new MiniMapTexture();
            MiniMap.Initialize();
            _ui = Service.Get<UserInterfaceService>();
        }

        public override void Draw(double frameTime)
        {
            var player = WorldModel.Entities.GetPlayerEntity();
            if (player == null)
                return;
            if (Model.Map == null)
                return;
            var center = player.Position;
            if ((player as Mobile).IsAlive)
            {
                AEntityView.Technique = Techniques.Default;
                _showingDeathEffect = false;
                if (_youAreDead != null)
                {
                    _youAreDead.Dispose();
                    _youAreDead = null;
                }
            }
            else
            {
                if (!_showingDeathEffect)
                {
                    _showingDeathEffect = true;
                    _deathEffectTime = 0;
                    _lightingGlobal = Isometric.Lighting.OverallLightning;
                    _lightingPersonal = Isometric.Lighting.PersonalLightning;
                    _ui.AddControl(_youAreDead = new YouAreDeadGump(), 0, 0);
                }
                var msFade = 2000d;
                var msHold = 1000d;
                if (_deathEffectTime < msFade)
                {
                    AEntityView.Technique = Techniques.Default;
                    Isometric.Lighting.OverallLightning = (int)(_lightingGlobal + (0x1f - _lightingGlobal) * ((_deathEffectTime / msFade)));
                    Isometric.Lighting.PersonalLightning = (int)(_lightingPersonal * (1d - (_deathEffectTime / msFade)));
                }
                else if (_deathEffectTime < msFade + msHold)
                {
                    Isometric.Lighting.OverallLightning = 0x1f;
                    Isometric.Lighting.PersonalLightning = 0x00;
                }
                else
                {
                    AEntityView.Technique = Techniques.Grayscale;
                    Isometric.Lighting.OverallLightning = (int)_lightingGlobal;
                    Isometric.Lighting.PersonalLightning = (int)_lightingPersonal;
                    if (_youAreDead != null)
                    {
                        _youAreDead.Dispose();
                        _youAreDead = null;
                    }
                }
                _deathEffectTime += frameTime;
            }
            Isometric.Update(Model.Map, center, Model.Input.MousePick);
            MiniMap.Update(Model.Map, center);
        }
    }
}
