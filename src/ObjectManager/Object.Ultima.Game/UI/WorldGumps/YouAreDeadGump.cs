using OA.Ultima.UI.Controls;

namespace OA.Ultima.UI.WorldGumps
{
    class YouAreDeadGump : Gump
    {
        public YouAreDeadGump()
            : base(0, 0)
        {
            AddControl(new HtmlGumpling(this, 0, 0, 200, 40, 0, 0, "<big><center>You are dead.</center></big>"));
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
            CenterThisControlOnScreen();
        }
    }
}
