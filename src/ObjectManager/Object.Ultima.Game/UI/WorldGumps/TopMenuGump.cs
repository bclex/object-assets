using OA.Core;
using OA.Core.UI;
using OA.Ultima.Core.Network;
using OA.Ultima.Network.Client;
using OA.Ultima.UI.Controls;
using OA.Ultima.World;
using OA.Ultima.World.Entities.Mobiles;

namespace OA.Ultima.UI.WorldGumps
{
    class TopMenuGump : Gump
    {
        WorldModel _world;
        INetworkClient _client;

        public TopMenuGump()
            : base(0, 0)
        {
            IsUncloseableWithRMB = true;
            IsMoveable = true;
            
            // maximized view
            AddControl(new ResizePic(this, 0, 0, 9200, 610, 27), 1);
            AddControl(new Button(this, 5, 3, 5540, 5542, 0, 2, 0), 1);
            ((Button)LastControl).GumpOverID = 5541;
            // buttons are 2443 small, 2445 big
            // 30, 93, 201, 309, 417, 480, 543
            // map, paperdollB, inventoryB, journalB, chat, help, < ? >
            AddControl(new Button(this, 30, 3, 2443, 2443, ButtonTypes.Activate, 0, (int)Buttons.Map), 1);
            ((Button)LastControl).Caption = "<basefont color=#000000>Map";
            AddControl(new Button(this, 93, 3, 2445, 2445, ButtonTypes.Activate, 0, (int)Buttons.Paperdoll), 1);
            ((Button)LastControl).Caption = "<basefont color=#000000>Paperdoll";
            AddControl(new Button(this, 201, 3, 2445, 2445, ButtonTypes.Activate, 0, (int)Buttons.Inventory), 1);
            ((Button)LastControl).Caption = "<basefont color=#000000>Inventory";
            AddControl(new Button(this, 309, 3, 2445, 2445, ButtonTypes.Activate, 0, (int)Buttons.Journal), 1);
            ((Button)LastControl).Caption = "<basefont color=#000000>Journal";
            AddControl(new Button(this, 417, 3, 2443, 2443, ButtonTypes.Activate, 0, (int)Buttons.Chat), 1);
            ((Button)LastControl).Caption = "<basefont color=#000000>Chat";
            AddControl(new Button(this, 480, 3, 2443, 2443, ButtonTypes.Activate, 0, (int)Buttons.Help), 1);
            ((Button)LastControl).Caption = "<basefont color=#000000>Help";
            AddControl(new Button(this, 543, 3, 2443, 2443, ButtonTypes.Activate, 0, (int)Buttons.Question), 1);
            ((Button)LastControl).Caption = "<basefont color=#000000>Debug";
            // minimized view
            AddControl(new ResizePic(this, 0, 0, 9200, 30, 27), 2);
            AddControl(new Button(this, 5, 3, 5537, 5539, 0, 1, 0), 2);
            ((Button)LastControl).GumpOverID = 5538;

            _world = Service.Get<WorldModel>();
            _client = Service.Get<INetworkClient>();

            MetaData.Layer = UILayer.Over;
        }
        
        protected override void OnInitialize()
        {
            SetSavePositionName("topmenu");
            base.OnInitialize();
        }

        public override void OnButtonClick(int buttonID)
        {
            switch ((Buttons)buttonID)
            {
                case Buttons.Map:
                    MiniMapGump.Toggle();
                    break;
                case Buttons.Paperdoll:
                    var player = (Mobile)WorldModel.Entities.GetPlayerEntity();
                    if (UserInterface.GetControl<PaperDollGump>(player.Serial) == null)
                        _client.Send(new DoubleClickPacket(player.Serial | Serial.ProtectedAction)); // additional flag keeps us from being dismounted.
                    else
                        UserInterface.RemoveControl<PaperDollGump>(player.Serial);
                    break;
                case Buttons.Inventory:
                    // opens the player's backpack.
                    var mobile = WorldModel.Entities.GetPlayerEntity();
                    var backpack = mobile.Backpack;
                    _world.Interaction.DoubleClick(backpack);
                    break;
                case Buttons.Journal:
                    if (UserInterface.GetControl<JournalGump>() == null)
                        UserInterface.AddControl(new JournalGump(), 80, 80);
                    else
                        UserInterface.RemoveControl<JournalGump>();
                    break;
                case Buttons.Chat:
                    break;
                case Buttons.Help:
                    break;
                case Buttons.Question:
                    if (UserInterface.GetControl<DebugGump>() == null)
                        UserInterface.AddControl(new DebugGump(), 50, 50);
                    else
                        UserInterface.RemoveControl<DebugGump>();
                    break;
            }
        }

        enum Buttons
        {
            Map,
            Paperdoll,
            Inventory,
            Journal,
            Chat,
            Help,
            Question
        }
    }
}
