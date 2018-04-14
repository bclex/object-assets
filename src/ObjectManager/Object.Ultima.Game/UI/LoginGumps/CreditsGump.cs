using OA.Core.Input;
using OA.Ultima.UI.Controls;
using System.IO;

namespace OA.Ultima.UI.LoginGumps
{
    public class CreditsGump : Gump
    {
        public CreditsGump()
            : base(0, 0)
        {
            AddControl(new GumpPicTiled(this, 0, 0, 800, 600, 0x0588));
            AddControl(new HtmlGumpling(this, 96, 64, 400, 400, 1, 1, ReadCreditsFile()));
            HandlesMouseInput = true;
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            Dispose();
        }

        private string ReadCreditsFile()
        {
            string path = @"Data/credits.txt";
            if (!File.Exists(path))
                return "<span color='#000'>Credits file not found.";
            try
            {
                var text = File.ReadAllText(@"Data\credits.txt");
                return text;
            }
            catch
            {
                return "<span color='#000'>Could not read credits file.";
            }
        }
    }
}
