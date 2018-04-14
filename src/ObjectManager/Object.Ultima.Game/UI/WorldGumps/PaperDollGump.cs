using OA.Core;
using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Core.Network;
using OA.Ultima.Network.Client;
using OA.Ultima.UI.Controls;
using OA.Ultima.World;
using OA.Ultima.World.Entities.Mobiles;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Ultima.UI.WorldGumps
{
    class PaperDollGump : Gump
    {
        private enum Buttons
        {
            Help,
            Options,
            LogOut,
            Quests,
            Skills,
            Guild,
            PeaceWarToggle,
            Status
        }

        public Mobile Mobile { get; private set; }

        public string Title { get; private set; }

        WorldModel _world;
        INetworkClient _client;

        bool _isWarMode;
        Button _warModeBtn;

        readonly int[] PeaceModeBtnGumps = { 0x07e5, 0x07e6, 0x07e7 };
        readonly int[] WarModeBtnGumps = { 0x07e8, 0x07e9, 0x07ea };

        GumpPic _virtueMenuButton;

        public PaperDollGump()
            : base(0, 0)
        {
        }

        public PaperDollGump(Serial serial, string mobileTitle)
            : this()
        {
            var mobile = WorldModel.Entities.GetObject<Mobile>(serial, false);
            if (mobile != null)
            {
                Mobile = mobile;
                Title = mobileTitle;
                BuildGump();
            }
        }

        private void BuildGump()
        {
            _world = Service.Get<WorldModel>();
            _client = Service.Get<INetworkClient>();

            IsMoveable = true;
            SaveOnWorldStop = true;
            GumpLocalID = Mobile.Serial;

            if (Mobile.IsClientEntity)
            {
                AddControl(new GumpPic(this, 0, 0, 0x07d0, 0));

                // HELP
                AddControl(new Button(this, 185, 44 + 27 * 0, 0x07ef, 0x07f0, ButtonTypes.Activate, 0,
                    (int)Buttons.Help));
                ((Button)LastControl).GumpOverID = 0x07f1;
                // OPTIONS
                AddControl(new Button(this, 185, 44 + 27 * 1, 0x07d6, 0x07d7, ButtonTypes.Activate, 0,
                    (int)Buttons.Options));
                ((Button)LastControl).GumpOverID = 0x07d8;
                // LOG OUT
                AddControl(new Button(this, 185, 44 + 27 * 2, 0x07d9, 0x07da, ButtonTypes.Activate, 0,
                    (int)Buttons.LogOut));
                ((Button)LastControl).GumpOverID = 0x07db;
                // QUESTS
                AddControl(new Button(this, 185, 44 + 27 * 3, 0x57b5, 0x57b7, ButtonTypes.Activate, 0,
                    (int)Buttons.Quests));
                ((Button)LastControl).GumpOverID = 0x57b6;
                // SKILLS
                AddControl(new Button(this, 185, 44 + 27 * 4, 0x07df, 0x07e0, ButtonTypes.Activate, 0,
                    (int)Buttons.Skills));
                ((Button)LastControl).GumpOverID = 0x07e1;
                // GUILD
                AddControl(new Button(this, 185, 44 + 27 * 5, 0x57b2, 0x57b4, ButtonTypes.Activate, 0,
                    (int)Buttons.Guild));
                ((Button)LastControl).GumpOverID = 0x57b3;
                // PEACE / WAR
                _isWarMode = Mobile.Flags.IsWarMode;
                int[] btngumps = _isWarMode ? WarModeBtnGumps : PeaceModeBtnGumps;
                _warModeBtn = (Button)AddControl(new Button(this, 185, 44 + 27 * 6, btngumps[0], btngumps[1], ButtonTypes.Activate, 0,
                    (int)Buttons.PeaceWarToggle));
                ((Button)LastControl).GumpOverID = btngumps[2];
                // STATUS
                AddControl(new Button(this, 185, 44 + 27 * 7, 0x07eb, 0x07ec, ButtonTypes.Activate, 0,
                    (int)Buttons.Status));
                ((Button)LastControl).GumpOverID = 0x07ed;

                // Virtue menu
                AddControl(_virtueMenuButton = new GumpPic(this, 80, 8, 0x0071, 0));
                _virtueMenuButton.MouseDoubleClickEvent += VirtueMenu_MouseDoubleClickEvent;

                // Special moves book
                // AddControl(new GumpPic(this, 158, 200, 0x2B34, 0));
                // LastControl.MouseDoubleClickEvent += SpecialMoves_MouseDoubleClickEvent;

                // PARTY MANIFEST CALLER
                AddControl(new GumpPic(this, 44, 195, 2002, 0));
                LastControl.MouseDoubleClickEvent += PartyManifest_MouseDoubleClickEvent;

                // equipment slots for hat/earrings/neck/ring/bracelet
                AddControl(new EquipmentSlot(this, 2, 76, Mobile, EquipLayer.Helm));
                AddControl(new EquipmentSlot(this, 2, 76 + 22 * 1, Mobile, EquipLayer.Earrings));
                AddControl(new EquipmentSlot(this, 2, 76 + 22 * 2, Mobile, EquipLayer.Neck));
                AddControl(new EquipmentSlot(this, 2, 76 + 22 * 3, Mobile, EquipLayer.Ring));
                AddControl(new EquipmentSlot(this, 2, 76 + 22 * 4, Mobile, EquipLayer.Bracelet));

                // Paperdoll control!
                AddControl(new PaperdollInteractable(this, 8, 21, Mobile));
            }
            else
            {
                AddControl(new GumpPic(this, 0, 0, 0x07d1, 0));

                // Paperdoll
                AddControl(new PaperdollInteractable(this, 8, 21, Mobile));
            }

            // name and title
            AddControl(new HtmlGumpling(this, 35, 260, 180, 42, 0, 0, string.Format("<span color=#222 style='font-family:uni0;'>{0}", Title)));
        }

        public override void Dispose()
        {
            if (_virtueMenuButton != null)
                _virtueMenuButton.MouseDoubleClickEvent -= VirtueMenu_MouseDoubleClickEvent;
            base.Dispose();
        }
        private void PartyManifest_MouseDoubleClickEvent(AControl control, int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                if (UserInterface.GetControl<PartyGump>() == null)
                    UserInterface.AddControl(new PartyGump(), 200, 40);
                else
                    UserInterface.RemoveControl<PartyGump>();
            }
        }
        private void SpecialMoves_MouseDoubleClickEvent(AControl control, int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                // open special moves book.
            }
        }

        private void VirtueMenu_MouseDoubleClickEvent(AControl control, int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
                _client.Send(new GumpMenuSelectPacket(Mobile.Serial, 0x000001CD, 0x00000001, new int[1] { Mobile.Serial }, null));
        }

        protected override void OnInitialize()
        {
            SetSavePositionName(Mobile.IsClientEntity ? "paperdoll_self" : "paperdoll");
            base.OnInitialize();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (Mobile != null && Mobile.IsDisposed)
                Mobile = null;
            if (Mobile == null)
            {
                Dispose();
                return;
            }

            // switch the graphics on the peace/war btn if this is the player entity and warmode flag has changed.
            if (Mobile.IsClientEntity && _isWarMode != Mobile.Flags.IsWarMode)
            {
                _isWarMode = Mobile.Flags.IsWarMode;
                var btngumps = _isWarMode ? WarModeBtnGumps : PeaceModeBtnGumps;
                _warModeBtn.GumpUpID = btngumps[0];
                _warModeBtn.GumpDownID = btngumps[1];
                _warModeBtn.GumpOverID = btngumps[2];
            }

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            base.Draw(spriteBatch, position, frameMS);
        }

        public override void OnButtonClick(int buttonID)
        {
            switch ((Buttons)buttonID)
            {
                case Buttons.Help:
                    _client.Send(new RequestHelpPacket());
                    break;

                case Buttons.Options:
                    if (UserInterface.GetControl<OptionsGump>() == null) UserInterface.AddControl(new OptionsGump(), 80, 80);
                    else UserInterface.RemoveControl<OptionsGump>();
                    break;

                case Buttons.LogOut:
                    // MsgBoxGump g = MsgBoxGump.Show("Quit Ultima Online?", MsgBoxTypes.OkCancel);
                    // g.OnClose = logout_OnClose;
                    UserInterface.AddControl(new LogoutGump(), 0, 0);
                    break;

                case Buttons.Quests:
                    _client.Send(new QuestGumpRequestPacket(Mobile.Serial));
                    break;

                case Buttons.Skills:
                    _client.Send(new MobileQueryPacket(MobileQueryPacket.StatusType.Skills, Mobile.Serial));
                    if (UserInterface.GetControl<SkillsGump>() == null) UserInterface.AddControl(new SkillsGump(), 80, 80);
                    else UserInterface.RemoveControl<SkillsGump>();
                    break;

                case Buttons.Guild:
                    _client.Send(new GuildGumpRequestPacket(Mobile.Serial));
                    break;

                case Buttons.PeaceWarToggle:
                    _world.Interaction.ToggleWarMode();
                    break;

                case Buttons.Status:
                    StatusGump.Toggle(Mobile.Serial);
                    break;
            }
        }

        private void logout_OnClose()
        {
            _world.Disconnect();
        }

        public override int GetHashCode()
        {
            if (Mobile == null)
                return base.GetHashCode();
            else
                return Mobile.Serial;
        }

        public override bool SaveGump(out Dictionary<string, object> data)
        {
            data = new Dictionary<string, object>();
            data.Add("serial", (int)Mobile.Serial);
            return true;
        }

        public override bool RestoreGump(Dictionary<string, object> data)
        {
            int serial;
            if (data.ContainsKey("serial"))
            {
                serial = (int)data["serial"];
                var mobile = WorldModel.Entities.GetObject<Mobile>(serial, false);
                if (mobile != null)
                {
                    Mobile = mobile;
                    BuildGump();
                    return true;
                }
            }
            return false;
        }
    }
}