using OA.Core;
using OA.Core.UI;
using OA.Ultima.Core.Network;
using OA.Ultima.Network.Client;
using OA.Ultima.Network.Client.Partying;
using OA.Ultima.Network.Server.GeneralInfo;
using OA.Ultima.UI;
using OA.Ultima.UI.WorldGumps;
using OA.Ultima.World;
using System.Collections.Generic;

namespace OA.Ultima.Player.Partying
{
    public class PartySystem
    {
        Serial _leaderSerial;
        Serial _invitingPartyLeader;
        List<PartyMember> _partyMembers = new List<PartyMember>();
        bool _allowPartyLoot;

        public Serial LeaderSerial => _leaderSerial;
        public List<PartyMember> Members => _partyMembers;
        public bool InParty => _partyMembers.Count > 1;
        public bool PlayerIsLeader => InParty && PlayerState.Partying.LeaderSerial == WorldModel.PlayerSerial;

        public bool AllowPartyLoot
        {
            get { return _allowPartyLoot; }
            set
            {
                _allowPartyLoot = value;
                var network = Service.Get<INetworkClient>();
                network.Send(new PartyCanLootPacket(_allowPartyLoot));
            }
        }

        public void ReceivePartyMemberList(PartyMemberListInfo info)
        {
            var wasInParty = InParty;
            _partyMembers.Clear();
            for (var i = 0; i < info.Count; i++)
                AddMember(info.Serials[i]);
            RefreshPartyGumps();
            if (InParty && !wasInParty)
                AllowPartyLoot = _allowPartyLoot;
        }

        public void ReceiveRemovePartyMember(PartyRemoveMemberInfo info)
        {
            _partyMembers.Clear();
            for (var i = 0; i < info.Count; i++)
                AddMember(info.Serials[i]);
            RefreshPartyGumps();
        }

        public void ReceiveInvitation(PartyInvitationInfo info)
        {
            _invitingPartyLeader = info.PartyLeaderSerial;
        }

        public void AddMember(Serial serial)
        {
            var index = _partyMembers.FindIndex(p => p.Serial == serial);
            if (index != -1)
                _partyMembers.RemoveAt(index);
            _partyMembers.Add(new PartyMember(serial));
            var network = Service.Get<INetworkClient>();
            network.Send(new MobileQueryPacket(MobileQueryPacket.StatusType.BasicStatus, serial));
        }

        public PartyMember GetMember(int index)
        {
            if (index >= 0 && index < _partyMembers.Count)
                return _partyMembers[index];
            return null;
        }

        public PartyMember GetMember(Serial serial)
        {
            return _partyMembers.Find(p => p.Serial == serial);
        }

        public void LeaveParty()
        {
            var network = Service.Get<INetworkClient>();
            network.Send(new PartyLeavePacket());
            _partyMembers.Clear();
            _leaderSerial = Serial.Null;
            var ui = Service.Get<UserInterfaceService>();
            ui.RemoveControl<PartyHealthTrackerGump>();
        }

        public void DoPartyCommand(string text)
        {
            // I do this a little differently than the legacy client. With legacy, if you type "/add this other player,
            // please ?" the client will detect the first word is "add" and request an add player target. Instead, I
            // interpret this as a message, and send the message "add this other player, please?" as a party message.
            var network = Service.Get<INetworkClient>();
            var world = Service.Get<WorldModel>();
            var command = text.ToLower();
            var commandHandled = false;
            switch (command)
            {
                case "help":
                    ShowPartyHelp();
                    commandHandled = true;
                    break;
                case "add":
                    RequestAddPartyMemberTarget();
                    commandHandled = true;
                    break;
                case "rem":
                case "remove":
                    if (InParty && PlayerIsLeader)
                    {
                        world.Interaction.ChatMessage("Who would you like to remove from your party?", 3, 10, false);
                        network.Send(new PartyRequestRemoveTargetPacket());
                    }
                    commandHandled = true;
                    break;
                case "accept":
                    if (!InParty && _invitingPartyLeader.IsValid)
                    {
                        network.Send(new PartyAcceptPacket(_invitingPartyLeader));
                        _leaderSerial = _invitingPartyLeader;
                        _invitingPartyLeader = Serial.Null;
                    }
                    commandHandled = true;
                    break;
                case "decline":
                    if (!InParty && _invitingPartyLeader.IsValid)
                    {
                        network.Send(new PartyDeclinePacket(_invitingPartyLeader));
                        _invitingPartyLeader = Serial.Null;
                    }
                    commandHandled = true;
                    break;
                case "quit":
                    LeaveParty();
                    commandHandled = true;
                    break;
            }
            if (!commandHandled)
                network.Send(new PartyPublicMessagePacket(text));
        }

        internal void BeginPrivateMessage(int serial)
        {
            var member = GetMember((Serial)serial);
            if (member != null)
            {
                var chat = Service.Get<ChatControl>();
                chat.SetModeToPartyPrivate(member.Name, member.Serial);
            }
        }

        public void SendPartyPrivateMessage(Serial serial, string text)
        {
            var world = Service.Get<WorldModel>();
            var recipient = GetMember(serial);
            if (recipient != null)
            {
                var network = Service.Get<INetworkClient>();
                network.Send(new PartyPrivateMessagePacket(serial, text));
                world.Interaction.ChatMessage($"To {recipient.Name}: {text}", 3, UltimaGameSettings.UserInterface.PartyPrivateMsgColor, false);
            }
            else world.Interaction.ChatMessage("They are no longer in your party.", 3, UltimaGameSettings.UserInterface.PartyPrivateMsgColor, false);
        }

        internal void RequestAddPartyMemberTarget()
        {
            var network = Service.Get<INetworkClient>();
            var world = Service.Get<WorldModel>();
            if (!InParty)
            {
                _leaderSerial = WorldModel.PlayerSerial;
                network.Send(new PartyRequestAddTargetPacket());
            }
            else if (InParty && PlayerIsLeader)
                network.Send(new PartyRequestAddTargetPacket());
            else if (InParty && !PlayerIsLeader)
                world.Interaction.ChatMessage("You may only add members to the party if you are the leader.", 3, 10, false);
        }

        public void RefreshPartyGumps()
        {
            var ui = Service.Get<UserInterfaceService>();
            ui.RemoveControl<PartyHealthTrackerGump>();
            for (var i = 0; i < Members.Count; i++)
                ui.AddControl(new PartyHealthTrackerGump(Members[i]), 5, 40 + (48 * i));
            Gump gump;
            if ((gump = ui.GetControl<PartyGump>()) != null)
            {
                var x = gump.X;
                var y = gump.Y;
                ui.RemoveControl<PartyGump>();
                ui.AddControl(new PartyGump(), x, y);
            }
        }

        public void RemoveMember(Serial serial)
        {
            var network = Service.Get<INetworkClient>();
            network.Send(new PartyRemoveMemberPacket(serial));
            var index = _partyMembers.FindIndex(p => p.Serial == serial);
            if (index != -1)
                _partyMembers.RemoveAt(index);
        }

        public void ShowPartyHelp()
        {
            var world = Service.Get<WorldModel>();
            world.Interaction.ChatMessage("/add       - add a new member or create a party", 3, 51, true);
            world.Interaction.ChatMessage("/rem       - kick a member from your party", 3, 51, true);
            world.Interaction.ChatMessage("/accept    - join a party", 3, 51, true);
            world.Interaction.ChatMessage("/decline   - decline a party invitation", 3, 51, true);
            world.Interaction.ChatMessage("/quit      - leave your current party", 3, 51, true);
        }
    }
}