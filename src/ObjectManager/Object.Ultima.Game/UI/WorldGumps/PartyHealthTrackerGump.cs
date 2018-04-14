using OA.Ultima.Player;
using OA.Ultima.Player.Partying;
using OA.Ultima.UI.Controls;
using OA.Ultima.World;

namespace OA.Ultima.UI.WorldGumps
{
    class PartyHealthTrackerGump : Gump
    {
        Serial _serial;

        Button _btnPrivateMsg;
        GumpPic[] _barBGs;
        readonly GumpPicWithWidth[] _bars;
        readonly TextLabel _name;

        public PartyHealthTrackerGump(PartyMember member)
            : base(member.Serial, 0)
        {
            while (UserInterface.GetControl<MobileHealthTrackerGump>() != null)
                UserInterface.GetControl<MobileHealthTrackerGump>(member.Serial).Dispose();
            IsMoveable = false;
            IsUncloseableWithRMB = true;
            _serial = member.Serial;
            //AddControl(m_Background = new ResizePic(this, 0, 0, 3000, 131, 48));//I need opacity %1 background

            AddControl(_name = new TextLabel(this, 1, 0, 1, member.Name));
            //m_Background.MouseDoubleClickEvent += Background_MouseDoubleClickEvent; //maybe private message calling?
            _barBGs = new GumpPic[3];
            int sameX = 15;
            int sameY = 3;
            if (WorldModel.Entities.GetPlayerEntity().Serial != member.Serial)//you can't send a message to self
                AddControl(_btnPrivateMsg = new Button(this, 0, 20, 11401, 11402, ButtonTypes.Activate, member.Serial, 0));//private party message / use bandage ??
            AddControl(_barBGs[0] = new GumpPic(this, sameX, 15 + sameY, 9750, 0));
            AddControl(_barBGs[1] = new GumpPic(this, sameX, 24 + sameY, 9750, 0));
            AddControl(_barBGs[2] = new GumpPic(this, sameX, 33 + sameY, 9750, 0));
            _bars = new GumpPicWithWidth[3];
            AddControl(_bars[0] = new GumpPicWithWidth(this, sameX, 15 + sameY, 40, 0, 1f));//I couldn't find correct visual
            AddControl(_bars[1] = new GumpPicWithWidth(this, sameX, 24 + sameY, 9751, 0, 1f));//I couldn't find correct visual
            AddControl(_bars[2] = new GumpPicWithWidth(this, sameX, 33 + sameY, 41, 0, 1f));//I couldn't find correct visual

            // bars should not handle mouse input, pass it to the background gump.
            for (var i = 0; i < _barBGs.Length; i++)//???
                _bars[i].HandlesMouseInput = false;
        }

        public override void OnButtonClick(int buttonID)
        {
            if (buttonID == 0)//private message
                PlayerState.Partying.BeginPrivateMessage(_btnPrivateMsg.ButtonParameter);
        }

        public override void Update(double totalMS, double frameMS)
        {
            var member = PlayerState.Partying.GetMember(_serial);
            if (member == null)
            {
                Dispose();
                return;
            }
            _name.Text = member.Name;
            var mobile = member.Mobile;
            if (mobile == null)
                _bars[0].PercentWidthDrawn = _bars[1].PercentWidthDrawn = _bars[2].PercentWidthDrawn = 0f;
            else
            {
                _bars[0].PercentWidthDrawn = ((float)mobile.Health.Current / mobile.Health.Max);
                _bars[1].PercentWidthDrawn = ((float)mobile.Mana.Current / mobile.Mana.Max);
                _bars[2].PercentWidthDrawn = ((float)mobile.Stamina.Current / mobile.Stamina.Max);
                // I couldn't find correct visual
                //if (Mobile.Flags.IsBlessed)
                //    m_Bars[0].GumpID = 0x0809;
                //else if (Mobile.Flags.IsPoisoned)
                //    m_Bars[0].GumpID = 0x0808;
            }
            base.Update(totalMS, frameMS);
        }
    }
}