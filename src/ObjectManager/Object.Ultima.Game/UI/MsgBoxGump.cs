using OA.Ultima.UI.Controls;
using System;

namespace OA.Ultima.UI
{
    public enum MsgBoxTypes
    {
        OkOnly,
        OkCancel
    }

    public class MsgBoxGump : Gump
    {
        /// <summary>
        /// Opens a modal message box with either 'OK' or 'OK and Cancel' buttons.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="type"></param>
        public static MsgBoxGump Show(string msg, MsgBoxTypes type)
        {
            MsgBoxGump gump = new MsgBoxGump(msg, type);
            return gump;
        }

        string _msg;
        HtmlGumpling _text;
        MsgBoxTypes _type;

        public Action OnClose;
        public Action OnCancel;

        private MsgBoxGump(string msg, MsgBoxTypes msgBoxType)
            : base(0, 0)
        {
            _msg = "<big color=000000>" + msg;
            _type = msgBoxType;
            UserInterface.AddControl(this, 0, 0);
            MetaData.IsModal = true;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (IsInitialized && _text == null)
            {
                ResizePic resize;
                _text = new HtmlGumpling(this, 10, 10, 200, 200, 0, 0, _msg);
                int width = _text.Width + 20;
                AddControl(resize = new ResizePic(this, 0, 0, 9200, width, _text.Height + 45));
                AddControl(_text);
                // Add buttons
                switch (_type)
                {
                    case MsgBoxTypes.OkOnly:
                        AddControl(new Button(this, (_text.Width + 20) / 2 - 23, _text.Height + 15, 2074, 2075, ButtonTypes.Activate, 0, 0));
                        ((Button)LastControl).GumpOverID = 2076;
                        break;
                    case MsgBoxTypes.OkCancel:
                        AddControl(new Button(this, (width / 2) - 46 - 10, _text.Height + 15, 0x817, 0x818, ButtonTypes.Activate, 0, 1));
                        ((Button)LastControl).GumpOverID = 0x819;
                        AddControl(new Button(this, (width / 2) + 10, _text.Height + 15, 2074, 2075, ButtonTypes.Activate, 0, 0));
                        ((Button)LastControl).GumpOverID = 2076;
                        break;
                }
                
                base.Update(totalMS, frameMS);
                CenterThisControlOnScreen();
            }
            base.Update(totalMS, frameMS);
        }

        public override void OnButtonClick(int buttonID)
        {
            switch (buttonID)
            {
                case 0:
                    OnClose?.Invoke();
                    break;
                case 1:
                    OnCancel?.Invoke();
                    break;
            }
            Dispose();
        }
    }
}
