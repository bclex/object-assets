using OA.Core;
using OA.Core.UI;
using OA.Ultima.Core.Network;
using OA.Ultima.Data;
using OA.Ultima.Network.Client;
using OA.Ultima.Player;
using OA.Ultima.UI;
using OA.Ultima.UI.WorldGumps;
using OA.Ultima.World.Entities;
using OA.Ultima.World.Entities.Items;
using OA.Ultima.World.Entities.Items.Containers;
using OA.Ultima.World.Entities.Mobiles;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Ultima.World
{
    /// <summary>
    /// Hosts methods for interacting with the world.
    /// </summary>
    class WorldInteraction
    {
        WorldModel _world;
        INetworkClient _network;
        UserInterfaceService _userInterface;

        public WorldInteraction(WorldModel world)
        {
            _network = Service.Get<INetworkClient>();
            _userInterface = Service.Get<UserInterfaceService>();
            _world = world;
        }

        private Serial _lastTarget;
        public Serial LastTarget
        {
            get { return _lastTarget; }
            set
            {
                _lastTarget = value;
                _network.Send(new MobileQueryPacket(MobileQueryPacket.StatusType.BasicStatus, _lastTarget));
            }
        }

        /// <summary>
        /// For items, if server.expansion is less than AOS, sends single click packet.
        /// For mobiles, always sends a single click.
        /// Requests a context menu regardless of version.
        /// </summary>
        public void SingleClick(AEntity e)
        {
            if (!PlayerState.ClientFeatures.TooltipsEnabled)
                _network.Send(new SingleClickPacket(e.Serial));
            _network.Send(new RequestContextMenuPacket(e.Serial));
        }

        public void DoubleClick(AEntity e) // used by itemgumpling, paperdollinteractable, topmenu, worldinput.
        {
            if (e != null)
                _network.Send(new DoubleClickPacket(e.Serial));
        }

        public void AttackRequest(Mobile mobile)
        {
            // Do nothing on Invulnerable
            if (mobile.Notoriety == 0x7) { }
            // Attack Innocents, Reds and Greys
            else if (mobile.Notoriety == 0x1 || mobile.Notoriety == 0x3 || mobile.Notoriety == 0x4 || mobile.Notoriety == 0x5 || mobile.Notoriety == 0x6)
                _network.Send(new AttackRequestPacket(mobile.Serial));
            // CrimeQuery is enabled, ask before attacking others
            else if (UltimaGameSettings.UserInterface.CrimeQuery)
                _userInterface.AddControl(new CrimeQueryGump(mobile), 0, 0);
            // CrimeQuery is disabled, so attack without asking
            else _network.Send(new AttackRequestPacket(mobile.Serial));
        }

        public void ToggleWarMode() // used by paperdollgump.
        {
            _network.Send(new RequestWarModePacket(!((Mobile)WorldModel.Entities.GetPlayerEntity()).Flags.IsWarMode));
        }

        public void UseSkill(int index) // used by WorldInteraction
        {
            _network.Send(new RequestSkillUsePacket(index));
        }

        public void CastSpell(int index)
        {
            _network.Send(new CastSpellPacket(index));
        }

        public void ChangeSkillLock(SkillEntry skill)
        {
            if (skill == null)
                return;
            var nextLockState = (byte)(skill.LockType + 1);
            if (nextLockState > 2)
                nextLockState = 0;
            _network.Send(new SetSkillLockPacket((ushort)skill.Index, nextLockState));
            skill.LockType = nextLockState;
        }

        public void BookHeaderNewChange(Serial serial, string title, string author)
        {
            _network.Send(new BookHeaderNewChangePacket(serial, title, author));
        }

        public void BookHeaderOldChange(Serial serial, string title, string author)
        {
            // Not yet implemented
            // m_Network.Send(new BookHeaderOldChangePacket(serial, title, author));
        }

        public void BookPageChange(Serial serial, int page, string[] lines)
        {
            _network.Send(new BookPageChangePacket(serial, page, lines));
        }

        public Gump OpenContainerGump(AEntity entity) // used by ultimaclient.
        {
            Gump gump;
            if ((gump = (Gump)_userInterface.GetControl(entity.Serial)) != null)
                gump.Dispose();
            else
            {
                if (entity is Corpse)
                {
                    gump = new ContainerGump(entity, 0x2006);
                    _userInterface.AddControl(gump, 96, 96);
                }
                else if (entity is SpellBook)
                {
                    gump = new SpellbookGump((SpellBook)entity);
                    _userInterface.AddControl(gump, 96, 96);
                }
                else
                {
                    gump = new ContainerGump(entity, ((ContainerItem)entity).ItemID);
                    _userInterface.AddControl(gump, 64, 64);
                }
            }
            return gump;
        }

        List<QueuedMessage> _chatQueue = new List<QueuedMessage>();

        public void ChatMessage(string text) // used by gump
        {
            ChatMessage(text, 0, 0, true);
        }

        public void ChatMessage(string text, int font, int hue, bool asUnicode)
        {
            _chatQueue.Add(new QueuedMessage(text, font, hue, asUnicode));
            var chat = Service.Get<ChatControl>();
            if (chat != null)
            {
                foreach (var msg in _chatQueue)
                    chat.AddLine(msg.Text, msg.Font, msg.Hue, msg.AsUnicode);
                _chatQueue.Clear();
            }
        }

        class QueuedMessage
        {
            public string Text;
            public int Hue;
            public int Font;
            public bool AsUnicode;

            public QueuedMessage(string text, int font, int hue, bool asUnicode)
            {
                Text = text;
                Hue = hue;
                Font = font;
                AsUnicode = asUnicode;
            }
        }

        public void CreateLabel(MessageTypes msgType, Serial serial, string text, int font, int hue, bool asUnicode)
        {
            if (serial.IsValid) WorldModel.Entities.AddOverhead(msgType, serial, text, font, hue, asUnicode);
            else ChatMessage(text, font, hue, asUnicode);
        }

        // ============================================================================================================
        // Cursor handling routines.
        // ============================================================================================================

        public Action<Item, int, int, int?> OnPickupItem;
        public Action OnClearHolding;

        internal void PickupItem(Item item, Vector2Int offset, int? amount = null)
        {
            if (item == null)
                return;
            if (OnPickupItem == null)
                return;
            OnPickupItem(item, offset.x, offset.y, amount);
        }

        internal void ClearHolding()
        {
            if (OnClearHolding == null)
                return;
            OnClearHolding();
        }
    }
}
