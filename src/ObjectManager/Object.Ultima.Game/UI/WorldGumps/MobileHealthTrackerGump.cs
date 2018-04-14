using OA.Core;
using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.Core.Network;
using OA.Ultima.Network.Client;
using OA.Ultima.UI.Controls;
using OA.Ultima.World;
using OA.Ultima.World.Entities.Mobiles;

namespace OA.Ultima.UI.WorldGumps
{
    class MobileHealthTrackerGump : Gump
    {
        public Mobile Mobile { get; private set; }

        GumpPic _background;
        GumpPicWithWidth[] _bars;
        GumpPic[] _barBGs;
        TextEntry _nameEntry;
        readonly WorldModel _world;

        public MobileHealthTrackerGump(Mobile mobile)
            : base(mobile.Serial, 0)
        {
            while (UserInterface.GetControl<MobileHealthTrackerGump>(mobile.Serial) != null)
                UserInterface.GetControl<MobileHealthTrackerGump>(mobile.Serial).Dispose();

            IsMoveable = true;

            Mobile = mobile;
            _world = Service.Get<WorldModel>();

            if (Mobile.IsClientEntity)
            {
                AddControl(_background = new GumpPic(this, 0, 0, 0x0803, 0));
                _background.MouseDoubleClickEvent += Background_MouseDoubleClickEvent;
                _barBGs = new GumpPic[3];
                AddControl(_barBGs[0] = new GumpPic(this, 34, 10, 0x0805, 0));
                AddControl(_barBGs[1] = new GumpPic(this, 34, 24, 0x0805, 0));
                AddControl(_barBGs[2] = new GumpPic(this, 34, 38, 0x0805, 0));
                _bars = new GumpPicWithWidth[3];
                AddControl(_bars[0] = new GumpPicWithWidth(this, 34, 10, 0x0806, 0, 1f));
                AddControl(_bars[1] = new GumpPicWithWidth(this, 34, 24, 0x0806, 0, 1f));
                AddControl(_bars[2] = new GumpPicWithWidth(this, 34, 38, 0x0806, 0, 1f));
            }
            else
            {
                AddControl(_background = new GumpPic(this, 0, 0, 0x0804, 0));
                _background.MouseDoubleClickEvent += Background_MouseDoubleClickEvent;
                _barBGs = new GumpPic[1];
                AddControl(_barBGs[0] = new GumpPic(this, 34, 38, 0x0805, 0));
                _bars = new GumpPicWithWidth[1];
                AddControl(_bars[0] = new GumpPicWithWidth(this, 34, 38, 0x0806, 0, 1f));
                AddControl(_nameEntry = new TextEntry(this, 17, 16, 124, 20, 0, 0, 99, mobile.Name));
                SetupMobileNameEntry();
            }

            // bars should not handle mouse input, pass it to the background gump.
            for (var i = 0; i < _barBGs.Length; i++)
            {
                _barBGs[i].HandlesMouseInput = false;
                _bars[i].HandlesMouseInput = false;
            }
        }

        public override void Dispose()
        {
            _background.MouseDoubleClickEvent -= Background_MouseDoubleClickEvent;
            base.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            _bars[0].PercentWidthDrawn = ((float)Mobile.Health.Current / Mobile.Health.Max);
            if (Mobile.Flags.IsBlessed)
                _bars[0].GumpID = 0x0809;
            else if (Mobile.Flags.IsPoisoned)
                _bars[0].GumpID = 0x0808;

            if (Mobile.IsClientEntity)
            {
                if (Mobile.Flags.IsWarMode) _background.GumpID = 0x0807;
                else _background.GumpID = 0x0803;
                _bars[1].PercentWidthDrawn = ((float)Mobile.Stamina.Current / Mobile.Stamina.Max);
                _bars[2].PercentWidthDrawn = ((float)Mobile.Mana.Current / Mobile.Mana.Max);
            }
            else
            {
                // this doesn't change anything, but might as well leave it in incase we do want to change the graphic
                // based on some future condition.
                _background.GumpID = 0x0804;
                if (Mobile.PlayerCanChangeName != _nameEntry.IsEditable)
                    SetupMobileNameEntry();
            }

            base.Update(totalMS, frameMS);
        }

        private void Background_MouseDoubleClickEvent(AControl caller, int x, int y, MouseButton button)
        {
            if (Mobile.IsClientEntity)
                StatusGump.Toggle(Mobile.Serial);
            else
            {
                _world.Interaction.LastTarget = Mobile.Serial;

                // Attack
                if (WorldModel.Entities.GetPlayerEntity().Flags.IsWarMode)
                    _world.Interaction.AttackRequest(Mobile);
                // Open Paperdoll
                else
                    _world.Interaction.DoubleClick(Mobile);
            }
        }

        private void SetupMobileNameEntry()
        {
            if (Mobile.PlayerCanChangeName)
            {
                _nameEntry.IsEditable = true;
                _nameEntry.LeadingHtmlTag = "<span color='#808' style='font-family:uni0;'>";
            }
            else
            {
                _nameEntry.IsEditable = false;
                _nameEntry.LeadingHtmlTag = "<span color='#444' style='font-family:uni0;'>";
            }
        }

        public override void OnKeyboardReturn(int textID, string text)
        {
            var client = Service.Get<INetworkClient>();
            client.Send(new RenameCharacterPacket(Mobile.Serial, text));
        }
    }
}
