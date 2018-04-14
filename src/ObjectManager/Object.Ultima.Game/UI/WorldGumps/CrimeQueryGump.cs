using OA.Core;
using OA.Ultima.Core.Network;
using OA.Ultima.Network.Client;
using OA.Ultima.UI.Controls;
using OA.Ultima.World.Entities.Mobiles;

namespace OA.Ultima.UI.WorldGumps
{
    class CrimeQueryGump : Gump
    {
        public Mobile Mobile;

        public CrimeQueryGump(Mobile mobile)
            : base(0, 0)
        {
            Mobile = mobile;

            AddControl(new GumpPic(this, 0, 0, 0x0816, 0));
            AddControl(new TextLabelAscii(this, 40, 30, 1, 118, "This may flag you criminal!", 120));
            ((TextLabelAscii)LastControl).Hue = 997;
            AddControl(new Button(this, 40, 77, 0x817, 0x818, ButtonTypes.Activate, 0, 0));
            AddControl(new Button(this, 100, 77, 0x81A, 0x81B, ButtonTypes.Activate, 1, 1));

            IsMoveable = false;
            MetaData.IsModal = true;
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
            CenterThisControlOnScreen();
        }

        public override void OnButtonClick(int buttonID)
        {
            switch (buttonID)
            {
                case 0:
                    Dispose();
                    break;
                case 1:
                    var network = Service.Get<INetworkClient>();
                    network.Send(new AttackRequestPacket(Mobile.Serial));
                    Dispose();
                    break;
            }
        }
    }
}
