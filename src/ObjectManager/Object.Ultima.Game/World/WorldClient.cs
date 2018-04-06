using OA.Core;
using OA.Ultima.Core;
using OA.Ultima.Data;
using OA.Ultima.Resources;
using OA.Ultima.World.Data;
using OA.Ultima.World.Entities;
using OA.Ultima.World.Entities.Items;
using OA.Ultima.World.Entities.Items.Containers;
using OA.Ultima.World.Entities.Mobiles;
using OA.Ultima.World.Entities.Multis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Timers;
using UnityEngine;

namespace OA.Ultima.World
{
    class WorldClient : IDisposable
    {
        Timer _keepAliveTimer;
        readonly INetworkClient _network;
        UserInterfaceService _userInterface;
        WorldModel _world;

        public WorldClient(WorldModel world)
        {
            _world = world;
            _network = Service.Get<INetworkClient>();
            _userInterface = Service.Get<UserInterfaceService>();
        }

        public void Initialize()
        {
            Register<DamagePacket>(0x0B, 0x07, ReceiveDamage);
            Register<StatusInfoPacket>(0x11, -1, ReceiveStatusInfo);
            Register<ObjectInfoPacket>(0x1A, -1, ReceiveWorldItem);
            Register<AsciiMessagePacket>(0x1C, -1, ReceiveAsciiMessage);
            Register<RemoveEntityPacket>(0x1D, 5, ReceiveDeleteObject);
            Register<MobileUpdatePacket>(0x20, 19, ReceiveMobileUpdate);
            Register<MovementRejectPacket>(0x21, 8, ReceiveMoveRej);
            Register<MoveAcknowledgePacket>(0x22, 3, ReceiveMoveAck);
            Register<DragEffectPacket>(0x23, 26, ReceiveDragItem);
            Register<OpenContainerPacket>(0x24, 7, ReceiveContainer);
            Register<AddSingleItemToContainerPacket>(0x25, ClientVersion.HasExtendedAddItemPacket(Settings.UltimaOnline.PatchVersion) ? 21 : 20, ReceiveAddSingleItemToContainer);
            Register<LiftRejectionPacket>(0x27, 2, ReceiveRejectMoveItemRequest);
            Register<ResurrectionMenuPacket>(0x2C, 2, ReceiveResurrectionMenu);
            Register<MobileAttributesPacket>(0x2D, 17, ReceiveMobileAttributes);
            Register<WornItemPacket>(0x2E, 15, ReceiveWornItem);
            Register<SwingPacket>(0x2F, 10, ReceiveOnSwing);
            Register<SendSkillsPacket>(0x3A, -1, ReceiveSkillsList);
            Register<ContainerContentPacket>(0x3C, -1, ReceiveAddMultipleItemsToContainer);
            Register<PersonalLightLevelPacket>(0x4E, 6, ReceivePersonalLightLevel);
            Register<OverallLightLevelPacket>(0x4F, 2, ReceiveOverallLightLevel);
            Register<PopupMessagePacket>(0x53, 2, ReceivePopupMessage);
            Register<PlaySoundEffectPacket>(0x54, 12, ReceivePlaySoundEffect);
            Register<TimePacket>(0x5B, 4, ReceiveTime);
            Register<WeatherPacket>(0x65, 4, ReceiveSetWeather);
            Register<BookPagesPacket>(0x66, -1, ReceiveBookPages);
            Register<TargetCursorPacket>(0x6C, 19, ReceiveTargetCursor);
            Register<PlayMusicPacket>(0x6D, 3, ReceivePlayMusic);
            Register<MobileAnimationPacket>(0x6E, 14, ReceiveMobileAnimation);
            Register<GraphicEffectPacket>(0x70, 28, ReceiveGraphicEffect);
            Register<WarModePacket>(0x72, 5, ReceiveWarMode);
            Register<VendorBuyListPacket>(0x74, -1, ReceiveOpenBuyWindow);
            Register<SubServerPacket>(0x76, 16, ReceiveNewSubserver);
            Register<MobileMovingPacket>(0x77, 17, ReceiveMobileMoving);
            Register<MobileIncomingPacket>(0x78, -1, ReceiveMobileIncoming);
            Register<DisplayMenuPacket>(0x7C, -1, ReceiveDisplayMenu);
            Register<OpenPaperdollPacket>(0x88, 66, ReceiveOpenPaperdoll);
            Register<CorpseClothingPacket>(0x89, -1, ReceiveCorpseClothing);
            Register<BookHeaderOldPacket>(0x93, 99, ReceiveBookHeaderOld);
            Register<PlayerMovePacket>(0x97, 2, ReceivePlayerMove);
            Register<RequestNameResponsePacket>(0x98, -1, ReceiveRequestNameResponse);
            Register<TargetCursorMultiPacket>(0x99, 26, ReceiveTargetCursorMulti);
            Register<VendorSellListPacket>(0x9E, -1, ReceiveSellList);
            Register<UpdateHealthPacket>(0xA1, 9, ReceiveUpdateHealth);
            Register<UpdateManaPacket>(0xA2, 9, ReceiveUpdateMana);
            Register<UpdateStaminaPacket>(0xA3, 9, ReceiveUpdateStamina);
            Register<OpenWebBrowserPacket>(0xA5, -1, ReceiveOpenWebBrowser);
            Register<TipNoticePacket>(0xA6, -1, ReceiveTipNotice);
            Register<ChangeCombatantPacket>(0xAA, 5, ReceiveChangeCombatant);
            Register<UnicodeMessagePacket>(0xAE, -1, ReceiveUnicodeMessage);
            Register<DeathAnimationPacket>(0xAF, 13, ReceiveDeathAnimation);
            Register<DisplayGumpFastPacket>(0xB0, -1, ReceiveDisplayGumpFast);
            Register<ObjectHelpResponsePacket>(0xB7, -1, ReceiveObjectHelpResponse);
            Register<SupportedFeaturesPacket>(0xB9, ClientVersion.HasExtendedFeatures(Settings.UltimaOnline.PatchVersion) ? 5 : 3, ReceiveEnableFeatures);
            Register<QuestArrowPacket>(0xBA, 6, ReceiveQuestArrow);
            Register<SeasonChangePacket>(0xBC, 3, ReceiveSeasonalInformation);
            Register<GeneralInfoPacket>(0xBF, -1, ReceiveGeneralInfo);
            Register<GraphicEffectHuedPacket>(0xC0, 36, ReceiveHuedEffect);
            Register<MessageLocalizedPacket>(0xC1, -1, ReceiveCLILOCMessage);
            Register<InvalidMapEnablePacket>(0xC6, 1, ReceiveInvalidMapEnable);
            Register<GraphicEffectExtendedPacket>(0xC7, 49, ReceiveOnParticleEffect);
            Register<GlobalQueuePacket>(0xCB, 7, ReceiveGlobalQueueCount);
            Register<MessageLocalizedAffixPacket>(0xCC, -1, ReceiveMessageLocalizedAffix);
            Register<Extended0x78Packet>(0xD3, -1, ReceiveExtended0x78);
            Register<BookHeaderNewPacket>(0xD4, -1, ReceiveBookHeaderNew);
            Register<ObjectPropertyListPacket>(0xD6, -1, ReceiveObjectPropertyList);
            Register<CustomHousePacket>(0xD8, -1, ReceiveSendCustomHouse);
            Register<ObjectPropertyListUpdatePacket>(0xDC, 9, ReceiveToolTipRevision);
            Register<CompressedGumpPacket>(0xDD, -1, ReceiveCompressedGump);
            Register<ProtocolExtensionPacket>(0xF0, -1, ReceiveProtocolExtension);
            /* Deprecated (not used by RunUO) and/or not implmented
             * Left them here incase we need to implement in the future
            network.Register<HealthBarStatusPacket>(0x17, 12, OnHealthBarStatusUpdate);
            network.Register<KickPlayerPacket>(0x26, 5, OnKickPlayer);
            network.Register<DropItemFailedPacket>(0x28, 5, OnDropItemFailed);
            network.Register<PaperdollClothingAddAckPacket>(0x29, 1, OnPaperdollClothingAddAck
            network.Register<RecvPacket>(0x30, 5, OnAttackOk);
            network.Register<BloodPacket>(0x2A, 5, OnBlood);
            network.Register<RecvPacket>(0x33, -1, OnPauseClient);
            network.Register<RecvPacket>(0x90, -1, OnMapMessage);
            network.Register<RecvPacket>(0x9C, -1, OnHelpRequest);
            network.Register<RecvPacket>(0xAB, -1, OnGumpDialog);
            network.Register<RecvPacket>(0xB2, -1, OnChatMessage);
            network.Register<RecvPacket>(0xC4, -1, OnSemivisible);
            network.Register<RecvPacket>(0xD2, -1, OnExtended0x20);
            network.Register<RecvPacket>(0xDB, -1, OnCharacterTransferLog);
            network.Register<RecvPacket>(0xDE, -1, OnUpdateMobileStatus);
            network.Register<RecvPacket>(0xDF, -1, OnBuffDebuff);
            network.Register<RecvPacket>(0xE2, -1, OnMobileStatusAnimationUpdate);
            */
            MobileMovement.SendMoveRequestPacket += InternalOnEntity_SendMoveRequestPacket;
        }

        public void Dispose()
        {
            StopKeepAlivePackets();
            _network.Unregister(this);
            MobileMovement.SendMoveRequestPacket -= InternalOnEntity_SendMoveRequestPacket;
        }

        public void Register<T>(int id, int length, Action<T> onReceive) where T : IRecvPacket
        {
            _network.Register(this, id, length, onReceive);
        }

        public void SendWorldLoginPackets()
        {
            GetMySkills();
            SendClientVersion();
            SendClientScreenSize();
            SendClientLocalization();
            // Packet: BF 00 0A 00 0F 0A 00 00 00 1F
            // Packet: 09 00 00 00 02  
            // Packet: 06 80 00 00 17
            GetMyBasicStatus();
            // Packet: D6 00 0B 00 00 00 02 00 00 00 17
            // Packet: D6 00 37 40 00 00 FB 40 00 00 FD 40 00 00 FE 40
            //         00 00 FF 40 00 01 00 40 00 01 02 40 00 01 03 40
            //         00 01 04 40 00 01 05 40 00 01 06 40 00 01 07 40
            //         00 01 24 40 00 01 26 
        }

        public void StartKeepAlivePackets()
        {
            _keepAliveTimer = new Timer(
                e => SendKeepAlivePacket(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(4));
        }

        void StopKeepAlivePackets()
        {
            if (_keepAliveTimer != null)
                _keepAliveTimer.Dispose();
        }

        void SendKeepAlivePacket()
        {
            _network.Send(new UOSEKeepAlivePacket());
        }

        public void SendGumpMenuSelect(int id, int gumpId, int buttonId, int[] switchIds, Tuple<short, string>[] textEntries)
        {
            _network.Send(new GumpMenuSelectPacket(id, gumpId, buttonId, switchIds, textEntries));
        }

        /// <summary>
        /// Sends the server the client version. Version is specified in EngineVars.
        /// </summary>
        public void SendClientVersion()
        {
            if (Settings.UltimaOnline.PatchVersion.Length != 4)
                Utils.Warning("Cannot send seed packet: Version array is incorrectly sized.");
            else
                _network.Send(new ClientVersionPacket(Settings.UltimaOnline.PatchVersion));
        }

        public void GetMySkills()
        {
            _network.Send(new MobileQueryPacket(MobileQueryPacket.StatusType.Skills, WorldModel.PlayerSerial));
        }

        public void SendClientScreenSize()
        {
            _network.Send(new ReportClientScreenSizePacket(800, 600));
        }

        public void SendClientLocalization()
        {
            _network.Send(new ReportClientLocalizationPacket("ENU"));
        }

        public void GetMyBasicStatus()
        {
            _network.Send(new MobileQueryPacket(MobileQueryPacket.StatusType.BasicStatus, WorldModel.PlayerSerial));
        }

        void ReceiveTargetCursor(TargetCursorPacket p)
        {
            _world.Cursor.SetTargeting((WorldCursor.TargetType)p.CommandType, p.CursorID, p.CursorType);
        }

        void ReceiveTargetCursorMulti(TargetCursorMultiPacket p)
        {
            _world.Cursor.SetTargetingMulti(p.DeedSerial, p.MultiModel, p.CursorType);
        }

        void InternalOnEntity_SendMoveRequestPacket(MoveRequestPacket p)
        {
            _network.Send(p);
        }

        // ============================================================================================================
        // Effect handling
        // ============================================================================================================

        void ReceiveGraphicEffect(GraphicEffectPacket p)
        {
            WorldModel.Effects.Add(p);
        }

        void ReceiveHuedEffect(GraphicEffectHuedPacket p)
        {
            WorldModel.Effects.Add(p);
        }

        void ReceiveOnParticleEffect(GraphicEffectExtendedPacket p)
        {
            WorldModel.Effects.Add(p);
        }

        // ============================================================================================================
        // Entity handling
        // ============================================================================================================

        void ReceiveAddMultipleItemsToContainer(ContainerContentPacket p)
        {
            if (p.Items.Length == 0)
                return;
            // special handling for spellbook contents
            if (p.AllItemsInSameContainer)
            {
                var container = WorldModel.Entities.GetObject<ContainerItem>(p.Items[0].ContainerSerial, true);
                if (SpellbookData.GetSpellBookTypeFromItemID(container.ItemID) != SpellBookTypes.Unknown)
                {
                    var data = new SpellbookData(container, p);
                    (container as SpellBook).ReceiveSpellData(data.BookType, data.SpellsBitfield);
                }
            }
            foreach (var pItem in p.Items)
            {
                var item = CreateItem(pItem.Serial, pItem.ItemID, pItem.Hue, pItem.Amount);
                item.InContainerPosition = new Vector2Int(pItem.X, pItem.Y);
                PlaceItemInContainer(item, pItem.ContainerSerial);
            }
        }

        void ReceiveAddSingleItemToContainer(AddSingleItemToContainerPacket p)
        {
            var item = CreateItem(p.Serial, p.ItemId, p.Hue, p.Amount);
            item.InContainerPosition = new Vector2Int(p.X, p.Y);
            PlaceItemInContainer(item, p.ContainerSerial);
        }

        void PlaceItemInContainer(Item item, Serial cSerial)
        {
            // This code is necessary sanity checking: It may be possible that the server will ask us to add an item to
            // a mobile, which this codebase does not currently handle.
            var entity = WorldModel.Entities.GetObject<AEntity>(cSerial, false);
            if (entity == null)
                entity = WorldModel.Entities.GetObject<ContainerItem>(cSerial, true);
            if (entity is ContainerItem)
                (entity as ContainerItem).AddItem(item);
            else Utils.Warning($"Illegal PlaceItemInContainer({item}, {cSerial}): container is {entity.GetType()}.");
        }

        Item CreateItem(Serial serial, int itemID, int nHue, int amount)
        {
            Item item;
            if (itemID == 0x2006)
                // special case for corpses.
                item = WorldModel.Entities.GetObject<Corpse>((int)serial, true);
            else
            {
                if (TileData.ItemData[itemID].IsContainer)
                {
                    // special case for spellbooks.
                    if (SpellBook.IsSpellBookItem((ushort)itemID)) item = WorldModel.Entities.GetObject<SpellBook>(serial, true);
                    else item = WorldModel.Entities.GetObject<ContainerItem>(serial, true);
                }
                else
                {
                    // special case for books
                    if (Books.IsBookItem((ushort)itemID)) item = WorldModel.Entities.GetObject<BaseBook>(serial, true);
                    else item = WorldModel.Entities.GetObject<Item>(serial, true);
                }
            }
            if (item == null)
                return null;
            item.Amount = amount;
            item.ItemID = itemID;
            item.Hue = nHue;
            return item;
        }

        void ReceiveContainer(OpenContainerPacket p)
        {
            ContainerItem item;
            // Special case for 0x30, which tells us to open a buy from vendor window.
            if (p.GumpId == 0x30)
            {
                var mobile = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
                if (mobile == null) { } // log error - shopkeeper does not exist?
                else item = mobile.VendorShopContents;
            }
            else
            {
                item = WorldModel.Entities.GetObject<ContainerItem>(p.Serial, false);
                // log error - item does not exist
                if (item == null) _world.Interaction.ChatMessage("Client: Object {item.Serial} has no support for a container object!");
                else _world.Interaction.OpenContainerGump(item);
            }
        }

        void ReceiveWorldItem(ObjectInfoPacket p)
        {
            // Now create the GameObject.
            // If the iItemID < 0x4000, this is a regular game object.
            // If the iItemID >= 0x4000, then this is a multiobject.
            if (p.ItemID < 0x4000)
            {
                var item = CreateItem(p.Serial, p.ItemID, p.Hue, p.Amount);
                item.Position.Set(p.X, p.Y, p.Z);
            }
            else
            {
                var multiID = p.ItemID - 0x4000;
                var multi = WorldModel.Entities.GetObject<Multi>(p.Serial, true);
                multi.Position.Set(p.X, p.Y, p.Z);
                multi.MultiID = p.ItemID;
            }
        }

        void ReceiveWornItem(WornItemPacket p)
        {
            var item = CreateItem(p.Serial, p.ItemId, p.Hue, 0);
            WorldModel.Entities.AddWornItem(item, p.Layer, p.ParentSerial);
            if (item.PropertyList.Hash == 0)
                _network.Send(new QueryPropertiesPacket(item.Serial));
        }

        void ReceiveDeleteObject(RemoveEntityPacket p)
        {
            WorldModel.Entities.RemoveEntity(p.Serial);
        }

        void ReceiveMobileIncoming(MobileIncomingPacket p)
        {
            var mobile = WorldModel.Entities.GetObject<Mobile>(p.Serial, true);
            mobile.Body = p.BodyID;
            mobile.Hue = p.Hue;
            mobile.Move_Instant(p.X, p.Y, p.Z, p.Direction);
            mobile.Flags = p.Flags;
            mobile.Notoriety = p.Notoriety;
            mobile.Notoriety = p.Notoriety;
            for (var i = 0; i < p.Equipment.Length; i++)
            {
                var item = CreateItem(p.Equipment[i].Serial, p.Equipment[i].GumpId, p.Equipment[i].Hue, 1);
                mobile.WearItem(item, p.Equipment[i].Layer);
                if (item.PropertyList.Hash == 0)
                    _network.Send(new QueryPropertiesPacket(item.Serial));
            }
            if (mobile.Name == null || mobile.Name == string.Empty)
            {
                mobile.Name = "Unknown";
                _network.Send(new RequestNamePacket(p.Serial));
            }
            _network.Send(new SingleClickPacket(p.Serial)); // look at the object so we receive its stats.
        }

        void ReceiveDeathAnimation(DeathAnimationPacket p)
        {
            var m = WorldModel.Entities.GetObject<Mobile>(p.PlayerSerial, false);
            var c = WorldModel.Entities.GetObject<Corpse>(p.CorpseSerial, false);
            if (m == null) Utils.Warning("DeathAnimation received for mobile which does not exist.");
            else if (c == null) Utils.Warning("DeathAnimation received for corpse which does not exist.");
            else
            {
                c.Facing = m.DrawFacing;
                c.MobileSerial = p.PlayerSerial;
                c.PlayDeathAnimation();
            }
        }

        void ReceiveDragItem(DragEffectPacket p)
        {
            // This is sent by the server to display an item being dragged from one place to another.
            // Note that this does not actually move the item, it just displays an animation.
            // var iSourceGround = false;
            // var iDestGround = false;
            if (p.Source == Serial.World)
            {
                // iSourceGround = true;
            }
            if (p.Destination == Serial.World)
            {
                // iDestGround = true;
            }
            AnnounceUnhandledPacket(p);
        }

        void ReceiveMobileAttributes(MobileAttributesPacket p)
        {
            var mobile = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
            if (mobile == null)
                return;
            mobile.Health.Current = p.CurrentHits;
            mobile.Health.Max = p.MaxHits;
            mobile.Mana.Current = p.CurrentMana;
            mobile.Mana.Max = p.MaxMana;
            mobile.Stamina.Current = p.CurrentStamina;
            mobile.Stamina.Max = p.MaxStamina;
        }

        void ReceiveMobileAnimation(MobileAnimationPacket p)
        {
            var mobile = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
            if (mobile == null)
                return;
            mobile.Animate(p.Action, p.FrameCount, p.RepeatCount, p.Reverse, p.Repeat, p.Delay);
        }

        void ReceiveMobileMoving(MobileMovingPacket p)
        {
            var mobile = WorldModel.Entities.GetObject<Mobile>(p.Serial, true);
            if (mobile == null)
                return;
            mobile.Body = p.BodyID;
            mobile.Flags = p.Flags;
            mobile.Notoriety = p.Notoriety;
            if (mobile.IsClientEntity)
                return;
            if (mobile.Position.IsNullPosition) mobile.Move_Instant(p.X, p.Y, p.Z, p.Direction);
            else mobile.Mobile_AddMoveEvent(p.X, p.Y, p.Z, p.Direction);
        }

        void ReceiveMobileUpdate(MobileUpdatePacket p)
        {
            var mobile = WorldModel.Entities.GetObject<Mobile>(p.Serial, true);
            if (mobile == null)
                return;
            mobile.Body = p.BodyID;
            mobile.Flags = p.Flags;
            mobile.Hue = p.Hue;
            mobile.Move_Instant(p.X, p.Y, p.Z, p.Direction);
            if (mobile.Name == null || mobile.Name == string.Empty)
            {
                mobile.Name = "Unknown";
                _network.Send(new RequestNamePacket(p.Serial));
            }
        }

        void ReceiveMoveAck(MoveAcknowledgePacket p)
        {
            var player = WorldModel.Entities.GetPlayerEntity();
            player.PlayerMobile_MoveEventAck(p.Sequence);
            player.Notoriety = p.Notoriety;
        }

        void ReceiveMoveRej(MovementRejectPacket p)
        {
            var player = WorldModel.Entities.GetPlayerEntity();
            player.PlayerMobile_MoveEventRej(p.Sequence, p.X, p.Y, p.Z, p.Direction);
        }

        void ReceivePlayerMove(PlayerMovePacket p)
        {
            AnnounceUnhandledPacket(p);
        }

        void ReceiveRejectMoveItemRequest(LiftRejectionPacket p)
        {
            _world.Interaction.ChatMessage("Could not pick up item: " + p.ErrorMessage);
            _world.Interaction.ClearHolding();
        }

        // ============================================================================================================
        // Corpse handling
        // ============================================================================================================

        void ReceiveCorpseClothing(CorpseClothingPacket p)
        {
            var corpse = WorldModel.Entities.GetObject<Corpse>(p.CorpseSerial, false);
            if (corpse == null)
                return;
            foreach (var i in p.Items)
            {
                var item = WorldModel.Entities.GetObject<Item>(i.Serial, false);
                if (item != null)
                    corpse.Equipment[i.Layer] = item;
            }
        }

        // ============================================================================================================
        // Combat handling
        // ============================================================================================================

        void ReceiveChangeCombatant(ChangeCombatantPacket p)
        {
            if (p.Serial > 0x00000000)
                _world.Interaction.LastTarget = p.Serial;
        }

        void ReceiveDamage(DamagePacket p)
        {
            var entity = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
            if (entity == null)
                return;
            _world.Interaction.ChatMessage(string.Format("{0} takes {1} damage!", entity.Name, p.Damage));
        }

        void ReceiveOnSwing(SwingPacket p)
        {
            // this changes our last target - does this behavior match legacy?
            if (p.Attacker == WorldModel.PlayerSerial)
                _world.Interaction.LastTarget = p.Defender;
        }

        void ReceiveWarMode(WarModePacket p)
        {
            WorldModel.Entities.GetPlayerEntity().Flags.IsWarMode = p.WarMode;
        }

        void ReceiveUpdateMana(UpdateManaPacket p)
        {
            var entity = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
            if (entity == null)
                return;
            entity.Mana.Update(p.Current, p.Max);
        }

        void ReceiveUpdateStamina(UpdateStaminaPacket p)
        {
            var entity = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
            if (entity == null)
                return;
            entity.Stamina.Update(p.Current, p.Max);
        }

        void ReceiveUpdateHealth(UpdateHealthPacket p)
        {
            var entity = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
            if (entity == null)
                return;
            entity.Health.Update(p.Current, p.Max);
        }

        // ============================================================================================================
        // Chat / messaging handling
        // ============================================================================================================

        void ReceiveCLILOCMessage(MessageLocalizedPacket p)
        {
            // get the resource provider
            var provider = Service.Get<IResourceProvider>();
            var strCliLoc = constructCliLoc(provider.GetString(p.CliLocNumber), p.Arguements);
            ReceiveTextMessage(p.MessageType, strCliLoc, p.Font, p.Hue, p.Serial, p.SpeakerName, true);
        }

        void ReceiveAsciiMessage(AsciiMessagePacket p)
        {
            ReceiveTextMessage(p.MsgType, p.Text, p.Font, p.Hue, p.Serial, p.SpeakerName, false);
        }

        void ReceiveUnicodeMessage(UnicodeMessagePacket p)
        {
            ReceiveTextMessage(p.MsgType, p.Text, p.Font, p.Hue, p.Serial, p.SpeakerName, true);
        }

        void ReceiveMessageLocalizedAffix(MessageLocalizedAffixPacket p)
        {
            // get the resource provider
            var provider = Service.Get<IResourceProvider>();
            var localizedString = string.Format(p.Flag_IsPrefix ? "{1}{0}" : "{0}{1}", constructCliLoc(provider.GetString(p.CliLocNumber), p.Arguements), p.Affix);
            ReceiveTextMessage(p.MessageType, localizedString, p.Font, p.Hue, p.Serial, p.SpeakerName, true);
        }

        string constructCliLoc(string baseCliloc, string arg = null, bool capitalize = false)
        {
            if (string.IsNullOrEmpty(baseCliloc))
                return string.Empty;
            // get the resource provider
            var provider = Service.Get<IResourceProvider>();
            if (arg == null)
            {
                if (capitalize) return Utility.CapitalizeFirstCharacter(baseCliloc);
                else return baseCliloc;
            }
            else
            {
                var args = arg.Split('\t');
                for (var i = 0; i < args.Length; i++)
                    if (args[i].Length > 0 && args[i].Substring(0, 1) == "#")
                    {
                        var clilocID = Convert.ToInt32(args[i].Substring(1));
                        args[i] = provider.GetString(clilocID);
                    }
                var construct = baseCliloc;
                for (var i = 0; i < args.Length; i++)
                {
                    var iBeginReplace = construct.IndexOf('~', 0);
                    var iEndReplace = construct.IndexOf('~', iBeginReplace + 1);
                    if (iBeginReplace != -1 && iEndReplace != -1) construct = construct.Substring(0, iBeginReplace) + args[i] + construct.Substring(iEndReplace + 1, construct.Length - iEndReplace - 1);
                    else construct = baseCliloc;
                }
                if (capitalize) return Utility.CapitalizeFirstCharacter(construct);
                else return construct;
            }
        }

        void ReceiveTextMessage(MessageTypes msgType, string text, int font, ushort hue, Serial serial, string speakerName, bool asUnicode)
        {
            // PlayerState.Journaling.AddEntry(text, font, hue, speakerName, asUnicode);
            Overhead overhead;
            switch (msgType)
            {
                case MessageTypes.Normal:
                case MessageTypes.SpeechUnknown:
                    if (serial.IsValid)
                    {
                        overhead = WorldModel.Entities.AddOverhead(msgType, serial, text, font, hue, asUnicode);
                        PlayerState.Journaling.AddEntry(text, font, hue, speakerName, asUnicode);
                    }
                    else
                    {
                        _world.Interaction.ChatMessage(text, font, hue, asUnicode);
                        PlayerState.Journaling.AddEntry(text, font, hue, string.Empty, asUnicode);
                    }
                    break;
                case MessageTypes.System:
                    _world.Interaction.ChatMessage("[SYSTEM] " + text, font, hue, asUnicode);
                    break;
                case MessageTypes.Emote:
                    if (serial.IsValid)
                    {
                        overhead = WorldModel.Entities.AddOverhead(msgType, serial, string.Format("* {0} *", text), font, hue, asUnicode);
                        PlayerState.Journaling.AddEntry(string.Format("* {0} *", text), font, hue, speakerName, asUnicode);
                    }
                    else
                    {
                        PlayerState.Journaling.AddEntry(text, font, hue, string.Format("* {0} *", text), asUnicode);
                    }
                    break;
                case MessageTypes.Label:
                    _world.Interaction.CreateLabel(msgType, serial, text, font, hue, asUnicode);
                    PlayerState.Journaling.AddEntry(text, font, hue, "You see", asUnicode);
                    break;
                case MessageTypes.Focus: // on player?
                    _world.Interaction.ChatMessage("[FOCUS] " + text, font, hue, asUnicode);
                    break;
                case MessageTypes.Whisper:
                    _world.Interaction.ChatMessage("[WHISPER] " + text, font, hue, asUnicode);
                    break;
                case MessageTypes.Yell:
                    _world.Interaction.ChatMessage("[YELL] " + text, font, hue, asUnicode);
                    break;
                case MessageTypes.Spell:
                    if (serial.IsValid)
                    {
                        overhead = WorldModel.Entities.AddOverhead(msgType, serial, text, font, hue, asUnicode);
                        PlayerState.Journaling.AddEntry(text, font, hue, speakerName, asUnicode);
                    }
                    break;
                case MessageTypes.Guild:
                    _world.Interaction.ChatMessage($"[GUILD] {speakerName}: {text}", font, hue, asUnicode);
                    break;
                case MessageTypes.Alliance:
                    _world.Interaction.ChatMessage($"[ALLIANCE] {speakerName}: {text}", font, hue, asUnicode);
                    break;
                case MessageTypes.Command:
                    _world.Interaction.ChatMessage("[COMMAND] " + text, font, hue, asUnicode);
                    break;
                case MessageTypes.PartyDisplayOnly:
                    _world.Interaction.ChatMessage($"[PARTY] {speakerName}: {text}", font, hue, asUnicode);
                    break;
                case MessageTypes.Information:
                    _world.Interaction.CreateLabel(msgType, serial, text, font, hue, asUnicode);
                    break;
                default:
                    Utils.Warning($"Speech p with unknown msgType parameter received. MsgType={msgType} Msg={text}");
                    break;
            }
        }

        // ============================================================================================================
        // Gump & Menu handling
        // ============================================================================================================

        void ReceiveResurrectionMenu(ResurrectionMenuPacket p)
        {
            switch (p.ResurrectionAction)
            {
                case 0x00: break; // Notify client of their death.
                case 0x01: break; // Client has chosen to resurrect with penalties.
                case 0x02: break; // Client has chosen to play as ghost.

            }
        }

        void ReceivePopupMessage(PopupMessagePacket p)
        {
            MsgBoxGump.Show(p.Message, MsgBoxTypes.OkOnly);
        }

        void ReceiveOpenBuyWindow(VendorBuyListPacket p)
        {
            var entity = WorldModel.Entities.GetObject<Item>(p.VendorPackSerial, false);
            if (entity == null)
                return;
            _userInterface.RemoveControl<VendorBuyGump>();
            _userInterface.AddControl(new VendorBuyGump(entity, p), 200, 200);
        }

        void ReceiveSellList(VendorSellListPacket p)
        {
            _userInterface.RemoveControl<VendorSellGump>();
            _userInterface.AddControl(new VendorSellGump(p), 200, 200);
        }

        void ReceiveOpenPaperdoll(OpenPaperdollPacket p)
        {
            if (_userInterface.GetControl<PaperDollGump>(p.Serial) == null)
                _userInterface.AddControl(new PaperDollGump(p.Serial, p.MobileTitle), 400, 100);
        }

        void ReceiveCompressedGump(CompressedGumpPacket p)
        {
            if (p.HasData)
            {
                string[] gumpPieces;
                if (TryParseGumplings(p.GumpData, out gumpPieces))
                {
                    var g = (Gump)_userInterface.AddControl(new Gump(p.GumpSerial, p.GumpTypeID, gumpPieces, p.TextLines), p.X, p.Y);
                    g.IsMoveable = true;
                }
            }
        }

        void ReceiveDisplayGumpFast(DisplayGumpFastPacket p)
        {
            AnnounceUnhandledPacket(p);
        }

        void ReceiveDisplayMenu(DisplayMenuPacket p)
        {
            AnnounceUnhandledPacket(p);
        }

        bool TryParseGumplings(string gumpData, out string[] pieces)
        {
            var i = new List<string>();
            var dataIndex = 0;
            while (dataIndex < gumpData.Length)
            {
                if (gumpData.Substring(dataIndex) == "\0")
                    break;
                else
                {
                    var begin = gumpData.IndexOf("{", dataIndex);
                    var end = gumpData.IndexOf("}", dataIndex + 1);
                    if (begin != -1 && end != -1)
                    {
                        var sub = gumpData.Substring(begin + 1, end - begin - 1).Trim();
                        i.Add(sub);
                        dataIndex = end;
                    }
                    else break;
                }
            }

            pieces = i.ToArray();
            return (pieces.Length > 0);
        }

        //
        // Other packets
        // 

        void ReceiveNewSubserver(SubServerPacket p)
        {
            // this packet does not matter on modern server software that handles an entire shard on one server.
        }

        void ReceiveObjectHelpResponse(ObjectHelpResponsePacket p)
        {
            AnnounceUnhandledPacket(p);
        }

        void ReceiveObjectPropertyList(ObjectPropertyListPacket p)
        {
            // get the resource provider
            var provider = Service.Get<IResourceProvider>();
            var entity = WorldModel.Entities.GetObject<AEntity>(p.Serial, false);
            if (entity == null)
                return; // received property list for entity that does not exist.
            entity.PropertyList.Hash = p.Hash;
            entity.PropertyList.Clear();
            for (var i = 0; i < p.CliLocs.Count; i++)
            {
                var strCliLoc = provider.GetString(p.CliLocs[i]);
                if (p.Arguements[i] == string.Empty) strCliLoc = constructCliLoc(strCliLoc, capitalize: true);
                else strCliLoc = constructCliLoc(strCliLoc, p.Arguements[i], true);
                if (i == 0) strCliLoc = string.Format("<span color='#ff0'>{0}</span>", Utility.CapitalizeFirstCharacter(strCliLoc.Trim()));
                entity.PropertyList.AddProperty(strCliLoc);
            }
        }

        void ReceiveSendCustomHouse(CustomHousePacket p)
        {
            CustomHousing.UpdateCustomHouseData(p.HouseSerial, p.RevisionHash, p.PlaneCount, p.Planes);
            var multi = WorldModel.Entities.GetObject<Multi>(p.HouseSerial, false);
            if (multi.CustomHouseRevision != p.RevisionHash)
            {
                var house = CustomHousing.GetCustomHouseData(p.HouseSerial);
                multi.AddCustomHousingTiles(house);
            }
        }

        void ReceiveSkillsList(SendSkillsPacket p)
        {
            foreach (var skill in p.Skills)
            {
                var entry = PlayerState.Skills.SkillEntry(skill.SkillID);
                if (entry != null)
                {
                    entry.Value = skill.SkillValue;
                    entry.ValueUnmodified = skill.SkillValueUnmodified;
                    entry.LockType = skill.SkillLock;
                    entry.Cap = skill.SkillCap;
                }
            }
        }

        void ReceiveStatusInfo(StatusInfoPacket p)
        {
            if (p.StatusTypeFlag >= 6)
                throw (new Exception("KR Status not handled."));
            var mobile = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
            if (mobile == null)
                return;
            mobile.Name = p.PlayerName;
            mobile.Strength = p.Strength;
            mobile.Dexterity = p.Dexterity;
            mobile.Intelligence = p.Intelligence;
            mobile.Health.Update(p.CurrentHealth, p.MaxHealth);
            mobile.Stamina.Update(p.CurrentStamina, p.MaxStamina);
            mobile.Mana.Update(p.CurrentMana, p.MaxMana);
            mobile.Followers.Update(p.Followers, p.MaxFollowers);
            mobile.Weight.Update(p.Weight, p.MaxWeight);
            mobile.StatCap = p.StatCap;
            mobile.Luck = p.Luck;
            mobile.Gold = p.GoldInInventory;
            mobile.ArmorRating = p.ArmorRating;
            mobile.ResistFire = p.ResistFire;
            mobile.ResistCold = p.ResistCold;
            mobile.ResistPoison = p.ResistPoison;
            mobile.ResistEnergy = p.ResistEnergy;
            mobile.DamageMin = p.DmgMin;
            mobile.DamageMax = p.DmgMax;
            mobile.PlayerCanChangeName = p.NameChangeFlag;
        }

        void ReceiveTime(TimePacket p)
        {
            _world.Interaction.ChatMessage(string.Format("The current server time is {0}:{1}:{2}", p.Hour, p.Minute, p.Second));
        }

        void ReceiveTipNotice(TipNoticePacket p)
        {
            AnnounceUnhandledPacket(p);
        }

        void ReceiveToolTipRevision(ObjectPropertyListUpdatePacket p)
        {
            if (!PlayerState.ClientFeatures.TooltipsEnabled)
                return;
            var entity = WorldModel.Entities.GetObject<AEntity>(p.Serial, false);
            if (entity == null)
                Utils.Warning($"Received tooltip for entity {p.Serial} before entity received.");
            else if (entity.PropertyList.Hash != p.RevisionHash)
                _network.Send(new QueryPropertiesPacket(p.Serial));
        }

        void AnnounceUnhandledPacket(IRecvPacket p)
        {
            Utils.Warn($"Client: Unhandled {p.GetType().Name} [ID:{p.Id}]");
        }

        void ReceiveExtended0x78(Extended0x78Packet p)
        {
            AnnounceUnhandledPacket(p);
        }

        void ReceiveGeneralInfo(GeneralInfoPacket p)
        {
            // Documented here: http://docs.polserver.com/packets/index.php?Packet=0xBF
            switch (p.Subcommand)
            {
                case GeneralInfoPacket.CloseGump:
                    var closeGumpInfo = p.Info as CloseGumpInfo;
                    var control = _userInterface.GetControlByTypeID(closeGumpInfo.GumpTypeID);
                    (control as Gump)?.OnButtonClick(closeGumpInfo.GumpButtonID);
                    break;
                case GeneralInfoPacket.Party:
                    var partyInfo = p.Info as PartyInfo;
                    switch (partyInfo.SubsubCommand)
                    {
                        case PartyInfo.CommandPartyList:
                            PlayerState.Partying.ReceivePartyMemberList(partyInfo.Info as PartyMemberListInfo);
                            break;
                        case PartyInfo.CommandRemoveMember:
                            PlayerState.Partying.ReceiveRemovePartyMember(partyInfo.Info as PartyRemoveMemberInfo);
                            break;
                        case PartyInfo.CommandPrivateMessage:
                        case PartyInfo.CommandPublicMessage:
                            PartyMessageInfo msg = partyInfo.Info as PartyMessageInfo;
                            PartyMember member = PlayerState.Partying.GetMember(msg.Source);
                            // note: msx752 identified hue 50 for "targeted to : " and 34 for "Help me.. I'm stunned !!"
                            var hue = (ushort)(msg.IsPrivate ? Settings.UserInterface.PartyPrivateMsgColor : Settings.UserInterface.PartyMsgColor);
                            ReceiveTextMessage(MessageTypes.PartyDisplayOnly, msg.Message, 3, hue, 0xFFFFFFF, member.Name, true);
                            break;
                        case PartyInfo.CommandInvitation:
                            PlayerState.Partying.ReceiveInvitation(partyInfo.Info as PartyInvitationInfo);
                            break;
                    }
                    break;
                case GeneralInfoPacket.SetMap:
                    var mapInfo = p.Info as MapIndexInfo;
                    _world.MapIndex = mapInfo.MapID;
                    break;
                case GeneralInfoPacket.ShowLabel:
                    var labelInfo = p.Info as ShowLabelInfo;
                    var item = WorldModel.Entities.GetObject<AEntity>(labelInfo.Serial, false);
                    var provider = Service.Get<IResourceProvider>();
                    item.Name = constructCliLoc(provider.GetString(labelInfo.LabelIndex));
                    _world.Interaction.CreateLabel(MessageTypes.Label, item.Serial, item.Name, 3, 946, true);
                    break;
                case GeneralInfoPacket.ContextMenu:
                    var menuInfo = p.Info as ContextMenuInfo;
                    var input = Service.Get<IInputService>();
                    _userInterface.AddControl(new ContextMenuGump(menuInfo.Menu), input.MousePosition.X - 10, input.MousePosition.Y - 20);
                    break;
                case GeneralInfoPacket.MapDiff:
                    TileMatrixDataPatch.EnableMapDiffs(p.Info as MapDiffInfo);
                    _world.Map.ReloadStatics();
                    break;
                case GeneralInfoPacket.ExtendedStats:
                    var extendedStats = p.Info as ExtendedStatsInfo;
                    if (extendedStats.Serial != WorldModel.PlayerSerial)
                        Utils.Warning("Extended Stats packet (0xBF subcommand 0x19) received for a mobile not our own.");
                    else
                    {
                        PlayerState.StatLocks.StrengthLock = extendedStats.Locks.Strength;
                        PlayerState.StatLocks.DexterityLock = extendedStats.Locks.Dexterity;
                        PlayerState.StatLocks.IntelligenceLock = extendedStats.Locks.Intelligence;
                    }
                    break;
                case GeneralInfoPacket.SpellBookContents:
                    var spellbook = (p.Info as SpellBookContentsInfo).Spellbook;
                    WorldModel.Entities.GetObject<SpellBook>(spellbook.Serial, true).ReceiveSpellData(spellbook.BookType, spellbook.SpellsBitfield);
                    break;
                case GeneralInfoPacket.HouseRevision:
                    var houseInfo = p.Info as HouseRevisionInfo;
                    if (CustomHousing.IsHashCurrent(houseInfo.Revision.Serial, houseInfo.Revision.Hash))
                    {
                        var multi = WorldModel.Entities.GetObject<Multi>(houseInfo.Revision.Serial, false);
                        if (multi == null) { } // received a house revision for a multi that does not exist.
                        else if (multi.CustomHouseRevision != houseInfo.Revision.Hash)
                        {
                            var house = CustomHousing.GetCustomHouseData(houseInfo.Revision.Serial);
                            multi.AddCustomHousingTiles(house);
                        }
                    }
                    else _network.Send(new RequestCustomHousePacket(houseInfo.Revision.Serial));
                    break;
                case GeneralInfoPacket.AOSAbilityIconConfirm: // (AOS) Ability icon confirm.
                    // no data, just (bf 00 05 21)
                    // What do we do with this???
                    break;
            }
        }

        void ReceiveGlobalQueueCount(GlobalQueuePacket p)
        {
            _world.Interaction.ChatMessage("System: There are currently " + p.Count + " available calls in the global queue.");
        }

        void ReceiveInvalidMapEnable(InvalidMapEnablePacket p)
        {
            AnnounceUnhandledPacket(p);
        }

        void ReceiveOpenWebBrowser(OpenWebBrowserPacket p)
        {
            if (!ValidateUrl(p.WebsiteUrl))
                return;
            try
            {
                Process.Start(p.WebsiteUrl);
            }
            catch (Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == unchecked((int)0x80004005))
                    MsgBoxGump.Show(noBrowser.Message, MsgBoxTypes.OkOnly);
            }
            catch (Exception other)
            {
                MsgBoxGump.Show(other.Message, MsgBoxTypes.OkOnly);
            }
        }

        bool ValidateUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            if (url.IndexOf("http", StringComparison.Ordinal) != 0)
                url = $"http://{url}";
            Uri uri;
            var result = Uri.TryCreate(url, UriKind.Absolute, out uri);
            if (!result)
                return false;
            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                return false;
            return true;
        }

        void ReceiveOverallLightLevel(OverallLightLevelPacket p)
        {
            // byte iLightLevel = reader.ReadByte();
            // 0x00 - day
            // 0x09 - OSI night
            // 0x1F - Black
            // Max normal val = 0x1F
            ((WorldView)_world.GetView()).Isometric.Lighting.OverallLightning = p.LightLevel;
        }

        void ReceivePersonalLightLevel(PersonalLightLevelPacket p)
        {
            // int iCreatureID = reader.ReadInt();
            // byte iLightLevel = reader.ReadByte();
            // 0x00 - day
            // 0x09 - OSI night
            // 0x1F - Black
            // Max normal val = 0x1F
            ((WorldView)_world.GetView()).Isometric.Lighting.PersonalLightning = p.LightLevel;
        }

        void ReceivePlayMusic(PlayMusicPacket p)
        {
            var service = Service.Get<AudioService>();
            service.PlayMusic(p.MusicID);
        }

        void ReceivePlaySoundEffect(PlaySoundEffectPacket p)
        {
            var service = Service.Get<AudioService>();
            service.PlaySound(p.SoundModel, spamCheck: true);
        }

        void ReceiveQuestArrow(QuestArrowPacket p)
        {
            AnnounceUnhandledPacket(p);
        }

        void ReceiveRequestNameResponse(RequestNameResponsePacket p)
        {
            var mobile = WorldModel.Entities.GetObject<Mobile>(p.Serial, false);
            if (mobile == null)
                return;
            mobile.Name = p.MobileName;
        }

        void ReceiveSeasonalInformation(SeasonChangePacket p)
        {
            if (p.SeasonChanged)
                _world.Map.Season = p.Season;
        }

        void ReceiveSetWeather(WeatherPacket p)
        {
            AnnounceUnhandledPacket(p);
        }

        void ReceiveBookPages(BookPagesPacket p)
        {
            var book = WorldModel.Entities.GetObject<BaseBook>(p.Serial, false);
            book.Pages = p.Pages;
            _userInterface.AddControl(new BookGump(book), 200, 200);
        }

        void ReceiveBookHeaderNew(BookHeaderNewPacket p)
        {
            var book = WorldModel.Entities.GetObject<BaseBook>(p.Serial, true);
            book.IsEditable = (p.Flag0 == 1 && p.Flag1 == 1);
            book.Title = p.Title;
            book.Author = p.Author;
        }

        void ReceiveBookHeaderOld(BookHeaderOldPacket p)
        {
            AnnounceUnhandledPacket(p);
        }

        void ReceiveEnableFeatures(SupportedFeaturesPacket p)
        {
            PlayerState.ClientFeatures.SetFlags(p.Flags);
        }

        void ReceiveProtocolExtension(ProtocolExtensionPacket p)
        {
            switch (p.Subcommand)
            {
                case ProtocolExtensionPacket.SubcommandNegotiateFeatures:
                    PlayerState.DisabledFeatures = p.DisabledFeatures;
                    _network.Send(new NegotiateFeatureResponsePacket());
                    break;
                default:
                    Utils.Warning($"Unhandled protocol extension packet (0xF0) with subcommand: 0x{p.Subcommand:x2}");
                    break;
            }
        }
    }
}
