using OA.Core;
using OA.Core.Input;
using OA.Core.UI;
using OA.Core.Windows;
using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Core.Network;
using OA.Ultima.Network.Client;
using OA.Ultima.Player;
using OA.Ultima.Resources;
using OA.Ultima.UI;
using OA.Ultima.UI.Controls;
using OA.Ultima.UI.WorldGumps;
using OA.Ultima.World.Entities;
using OA.Ultima.World.Entities.Items;
using OA.Ultima.World.Entities.Items.Containers;
using OA.Ultima.World.Entities.Mobiles;
using System;
using UnityEngine;

namespace OA.Ultima.World.Input
{
    /// <summary>
    /// Handles targeting, holding items, and dropping items (both into the UI and into the world).
    /// Draws the mouse cursor and any held item.
    /// </summary>
    class WorldCursor : UltimaCursor
    {
        // private variables
        Item _mouseOverItem;
        int _mouseOverItemSavedHue;

        // services
        INetworkClient _network;
        UserInterfaceService _userInterface;
        IInputService _input;
        WorldModel _world;

        public Item MouseOverItem
        {
            get { return _mouseOverItem; }
            protected set
            {
                if (_mouseOverItem == value)
                    return;
                if (_mouseOverItem != null)
                    _mouseOverItem.Hue = _mouseOverItemSavedHue;
                if (value == null)
                {
                    _mouseOverItem = null;
                    _mouseOverItemSavedHue = 0;
                }
                else
                {
                    _mouseOverItem = value;
                    _mouseOverItemSavedHue = _mouseOverItem.Hue;
                    _mouseOverItem.Hue = WorldView.MouseOverHue;
                }
            }
        }

        public WorldCursor(WorldModel model)
        {
            _network = Service.Get<INetworkClient>();
            _userInterface = Service.Get<UserInterfaceService>();
            _input = Service.Get<IInputService>();
            _world = model;
            InternalRegisterInteraction();
        }

        public override void Update()
        {
            MouseOverItem = null;
            if (IsHoldingItem && _input.HandleMouseEvent(MouseEvent.Up, UltimaGameSettings.UserInterface.Mouse.InteractionButton))
            {
                if (_world.Input.IsMouseOverUI)
                {
                    // mouse over ui
                    AControl target = _userInterface.MouseOverControl;
                    // attempt to drop the item onto an interface. The only acceptable targets for dropping items are:
                    // 1. ItemGumplings that...
                    //    a. ...represent containers (like a bag icon)
                    //    b. ...are of the same itemType and are generic, and can thus be merged.
                    //    c. ...are neither of the above; we attempt to drop the held item on top of the targeted item, if the targeted item is within a container.
                    // 2. Gumps that represent open Containers (GumpPicContainers, e.g. an open GumpPic of a chest)
                    // 3. Paperdolls for my character and equipment slots.
                    // 4. Backpack gumppics (seen in paperdolls).
                    if (target is ItemGumpling && !(target is ItemGumplingPaperdoll))
                    {
                        var targetItem = ((ItemGumpling)target).Item;
                        MouseOverItem = targetItem;
                        if (targetItem.ItemData.IsContainer) // 1.a.
                            DropHeldItemToContainer((ContainerItem)targetItem);
                        else if (HeldItem.ItemID == targetItem.ItemID && HeldItem.ItemData.IsGeneric) // 1.b.
                            MergeHeldItem(targetItem);
                        else // 1.c.
                        {
                            if (targetItem.Parent != null && targetItem.Parent is ContainerItem)
                                DropHeldItemToContainer(targetItem.Parent as ContainerItem,
                                    target.X + (_input.MousePosition.x - target.ScreenX) - _heldItemOffset.X,
                                    target.Y + (_input.MousePosition.y - target.ScreenY) - _heldItemOffset.Y);
                        }
                    }
                    else if (target is GumpPicContainer)
                    {
                        ContainerItem targetItem = (ContainerItem)((GumpPicContainer)target).Item;
                        MouseOverItem = targetItem;

                        int x = (int)_input.MousePosition.x - _heldItemOffset.X - (target.X + target.Parent.X);
                        int y = (int)_input.MousePosition.y - _heldItemOffset.Y - (target.Y + target.Parent.Y);
                        DropHeldItemToContainer(targetItem, x, y);
                    }
                    else if (target is ItemGumplingPaperdoll || (target is GumpPic && ((GumpPic)target).IsPaperdoll) || (target is EquipmentSlot))
                    {
                        if (HeldItem.ItemData.IsWearable)
                            WearHeldItem();
                    }
                    else if (target is GumpPicBackpack)
                    {
                        DropHeldItemToContainer((ContainerItem)((GumpPicBackpack)target).BackpackItem);
                    }
                }
                else if (_world.Input.IsMouseOverWorld)
                {
                    // mouse over world
                    AEntity mouseOverEntity = _world.Input.MousePick.MouseOverObject;

                    if (mouseOverEntity != null)
                    {
                        int x, y, z;
                        if (mouseOverEntity is Mobile) MergeHeldItem(mouseOverEntity); // if ((mouseOverEntity as Mobile).IsClientEntity)
                        else if (mouseOverEntity is Corpse) MergeHeldItem(mouseOverEntity);
                        else if (mouseOverEntity is Item || mouseOverEntity is StaticItem)
                        {
                            x = (int)mouseOverEntity.Position.X;
                            y = (int)mouseOverEntity.Position.Y;
                            z = (int)mouseOverEntity.Z;
                            if (mouseOverEntity is StaticItem)
                            {
                                z += ((StaticItem)mouseOverEntity).ItemData.Height;
                                DropHeldItemToWorld(x, y, z);
                            }
                            else if (mouseOverEntity is Item)
                            {
                                var targetItem = mouseOverEntity as Item;
                                MouseOverItem = targetItem;
                                if (targetItem.ItemID == HeldItem.ItemID && HeldItem.ItemData.IsGeneric)
                                    MergeHeldItem(targetItem);
                                else
                                {
                                    z += ((Item)mouseOverEntity).ItemData.Height;
                                    DropHeldItemToWorld(x, y, z);
                                }
                            }
                        }
                        else if (mouseOverEntity is Ground)
                        {
                            x = (int)mouseOverEntity.Position.X;
                            y = (int)mouseOverEntity.Position.Y;
                            z = (int)mouseOverEntity.Z;
                            DropHeldItemToWorld(x, y, z);
                        }
                        else
                        {
                            // over text?
                            return;
                        }
                    }
                }
            }

            if (IsTargeting)
            {
                if (_input.HandleKeyboardEvent(KeyboardEvent.Press, WinKeys.Escape, false, false, false))
                    SetTargeting(TargetType.Nothing, 0, 0);
                if (_input.HandleMouseEvent(MouseEvent.Click, UltimaGameSettings.UserInterface.Mouse.InteractionButton))
                {
                    // If isTargeting is true, then the target cursor is active and we are waiting for the player to target something.
                    switch (_targeting)
                    {
                        case TargetType.Object:
                        case TargetType.Position:
                            if (_world.Input.IsMouseOverUI)
                            {
                                // get object under mouse cursor.
                                var target = _userInterface.MouseOverControl;
                                if (target is ItemGumpling)
                                    // ItemGumping is the base class for all items, containers, and paperdoll items.
                                    mouseTargetingEventObject(((ItemGumpling)target).Item);
                                else if (target.RootParent is MobileHealthTrackerGump)
                                    // this is a mobile's mini-status gump (health bar, etc.) We can target it to cast spells on that mobile.
                                    mouseTargetingEventObject(((MobileHealthTrackerGump)target.RootParent).Mobile);
                            }
                            else if (_world.Input.IsMouseOverWorld)
                            {
                                // Send Select Object or Select XYZ packet, depending on the entity under the mouse cursor.
                                _world.Input.MousePick.PickOnly = PickType.PickStatics | PickType.PickObjects;
                                mouseTargetingEventObject(_world.Input.MousePick.MouseOverObject);
                            }
                            break;
                        case TargetType.MultiPlacement:
                            // select X, Y, Z
                            mouseTargetingEventXYZ(_world.Input.MousePick.MouseOverObject);
                            break;
                        default: throw new Exception("Unknown targetting type!");
                    }
                }
            }
            if (MouseOverItem == null && _world.Input.MousePick.MouseOverObject is Item)
            {
                var item = _world.Input.MousePick.MouseOverObject as Item;
                if (item is StaticItem || item.ItemData.Weight == 255)
                    return;
                MouseOverItem = _world.Input.MousePick.MouseOverObject as Item;
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        // ============================================================================================================
        // Drawing routines
        // ============================================================================================================

        HuedTexture _itemSprite;
        int _itemSpriteArtIndex = -1;

        public int ItemSpriteArtIndex
        {
            get { return _itemSpriteArtIndex; }
            set
            {
                if (value != _itemSpriteArtIndex)
                {
                    _itemSpriteArtIndex = value;
                    var provider = Service.Get<IResourceProvider>();
                    var art = provider.GetItemTexture(_itemSpriteArtIndex);
                    if (art == null)
                        // shouldn't we have a debug texture to show that we are missing this cursor art? !!!
                        _itemSprite = null;
                    else
                    {
                        var sourceRect = new RectInt(0, 0, art.width, art.height);
                        _itemSprite = new HuedTexture(art, Vector2Int.zero, sourceRect, 0);
                    }
                }
            }
        }

        protected override void BeforeDraw(SpriteBatchUI spriteBatch, Vector2Int position)
        {
            var player = WorldModel.Entities.GetPlayerEntity();
            // Hue the cursor if not in warmode and in trammel.
            if (WorldModel.IsInWorld && !player.Flags.IsWarMode && (_world.MapIndex == 1)) CursorHue = 2414;
            else CursorHue = 0;
            if (IsHoldingItem)
            {
                ItemSpriteArtIndex = HeldItem.DisplayItemID;
                if (_itemSprite != null)
                {
                    _itemSprite.Hue = HeldItem.Hue;
                    _itemSprite.Offset = _heldItemOffset;
                    if (HeldItem.Amount > 1 && HeldItem.ItemData.IsGeneric && HeldItem.DisplayItemID == HeldItem.ItemID)
                    {
                        int offset = HeldItem.ItemData.Unknown4;
                        _itemSprite.Draw(spriteBatch, new Vector2Int(position.x - 5, position.y - 5));
                    }
                    _itemSprite.Draw(spriteBatch, position);
                }
                // set up to draw standard cursor sprite above item art.
                base.BeforeDraw(spriteBatch, position);
            }
            else if (IsTargeting)
            {
                var artworkIndex = 8310;
                if (WorldModel.IsInWorld && player.Flags.IsWarMode)
                    // Over the interface or not in world. Display a default cursor.
                    artworkIndex -= 23;
                CursorSpriteArtIndex = artworkIndex;
                CursorOffset = new Vector2Int(13, 13);
                // sourceRect = new RectInt(1, 1, 46, 34);
                /*if (_targetingMulti != -1)
                {
                    // UNIMPLEMENTED !!! Draw a transparent multi
                }*/
            }
            else if ((_world.Input.ContinuousMouseMovementCheck || _world.Input.IsMouseOverWorld) && !_userInterface.IsModalControlOpen)
            {
                var resolution = UltimaGameSettings.UserInterface.PlayWindowGumpResolution;
                var mouseDirection = DirectionHelper.DirectionFromPoints(new Vector2Int(resolution.Width / 2, resolution.Height / 2), _world.Input.MouseOverWorldPosition);
                var artIndex = 0;
                switch (mouseDirection)
                {
                    case Direction.North:
                        CursorOffset = new Vector2Int(29, 1);
                        artIndex = 8299;
                        break;
                    case Direction.Right:
                        CursorOffset = new Vector2Int(41, 9);
                        artIndex = 8300;
                        break;
                    case Direction.East:
                        CursorOffset = new Vector2Int(36, 24);
                        artIndex = 8301; break;
                    case Direction.Down:
                        CursorOffset = new Vector2Int(14, 33);
                        artIndex = 8302;
                        break;
                    case Direction.South:
                        CursorOffset = new Vector2Int(2, 26);
                        artIndex = 8303;
                        break;
                    case Direction.Left:
                        CursorOffset = new Vector2Int(2, 10);
                        artIndex = 8304;
                        break;
                    case Direction.West:
                        CursorOffset = new Vector2Int(1, 1);
                        artIndex = 8305;
                        break;
                    case Direction.Up:
                        CursorOffset = new Vector2Int(4, 2);
                        artIndex = 8298;
                        break;
                    default:
                        CursorOffset = new Vector2Int(2, 10);
                        artIndex = 8309;
                        break;
                }
                if (WorldModel.IsInWorld && player.Flags.IsWarMode)
                    // Over the interface or not in world. Display a default cursor.
                    artIndex -= 23;
                CursorSpriteArtIndex = artIndex;
            }
            // cursor is over UI or there is a modal message box open. Set up to draw standard cursor sprite.
            else base.BeforeDraw(spriteBatch, position);
        }

        protected override void DrawTooltip(SpriteBatchUI spritebatch, Vector2Int position)
        {
            // Do not draw tooltips if:
            // * Client version is lower than the point at which tooltips are enabled.
            // * Player is holding an item.
            if (!PlayerState.ClientFeatures.TooltipsEnabled || IsHoldingItem)
            {
                if (_tooltip != null)
                {
                    _tooltip.Dispose();
                    _tooltip = null;
                }
                return;
            }
            // Draw tooltips for items:
            // 1. Items in the world (MouseOverItem)
            // 2. ItemGumplings (both in paperdoll and in containers)
            // 3. the Backpack icon (in paperdolls).
            if (MouseOverItem != null && MouseOverItem.PropertyList.HasProperties)
            {
                if (_tooltip == null) _tooltip = new Tooltip(MouseOverItem);
                else _tooltip.UpdateEntity(MouseOverItem);
                _tooltip.Draw(spritebatch, position.x, position.y + 24);
            }
            else if (_world.Input.MousePick.MouseOverObject != null && _world.Input.MousePick.MouseOverObject is Mobile && _world.Input.MousePick.MouseOverObject.PropertyList.HasProperties)
            {
                var entity = _world.Input.MousePick.MouseOverObject;
                if (_tooltip == null) _tooltip = new Tooltip(entity);
                else _tooltip.UpdateEntity(entity);
                _tooltip.Draw(spritebatch, position.x, position.y + 24);
            }
            else if (_userInterface.IsMouseOverUI && _userInterface.MouseOverControl != null &&
                _userInterface.MouseOverControl is ItemGumpling && (_userInterface.MouseOverControl as ItemGumpling).Item.PropertyList.HasProperties)
            {
                var entity = (_userInterface.MouseOverControl as ItemGumpling).Item;
                if (_tooltip == null) _tooltip = new Tooltip(entity);
                else _tooltip.UpdateEntity(entity);
                _tooltip.Draw(spritebatch, position.x, position.y + 24);
            }
            else if (_userInterface.IsMouseOverUI && _userInterface.MouseOverControl != null &&
                _userInterface.MouseOverControl is GumpPicBackpack && (_userInterface.MouseOverControl as GumpPicBackpack).BackpackItem.PropertyList.HasProperties)
            {
                var entity = (_userInterface.MouseOverControl as GumpPicBackpack).BackpackItem;
                if (_tooltip == null) _tooltip = new Tooltip(entity);
                else _tooltip.UpdateEntity(entity);
                _tooltip.Draw(spritebatch, position.x, position.y + 24);
            }
            else base.DrawTooltip(spritebatch, position);
        }

        // ============================================================================================================
        // Targeting enum and routines
        // ============================================================================================================

        public enum TargetType
        {
            Nothing = -1,
            Object = 0,
            Position = 1,
            MultiPlacement = 2
        }

        TargetType _targeting = TargetType.Nothing;
        int _targetCursorID;
        byte _targetCursorType;

        public TargetType Targeting
        {
            get { return _targeting; }
        }

        public bool IsTargeting
        {
            get { return _targeting != TargetType.Nothing; }
        }

        public void ClearTargetingWithoutTargetCancelPacket()
        {
            _targeting = TargetType.Nothing;
        }

        public void SetTargeting(TargetType targeting, int cursorID, byte cursorType)
        {
            if (_targeting != targeting || cursorID != _targetCursorID || cursorType != _targetCursorType)
            {
                if (targeting == TargetType.Nothing)
                    _network.Send(new TargetCancelPacket(_targetCursorID, _targetCursorType));
                // if we start targeting, we cancel movement.
                else _world.Input.ContinuousMouseMovementCheck = false;
                _targeting = targeting;
                _targetCursorID = cursorID;
                _targetCursorType = cursorType;
            }
        }

        int _multiModel;
        public void SetTargetingMulti(Serial deedSerial, int model, byte targetType)
        {
            SetTargeting(TargetType.MultiPlacement, (int)deedSerial, targetType);
            _multiModel = model;
        }

        void mouseTargetingEventXYZ(AEntity selectedEntity)
        {
            // Send the targetting event back to the server!
            var modelNumber = 0;
            if (selectedEntity is StaticItem) modelNumber = ((StaticItem)selectedEntity).ItemID;
            else modelNumber = 0;
            // Send the target ...
            _network.Send(new TargetXYZPacket((short)selectedEntity.Position.X, (short)selectedEntity.Position.Y, (short)selectedEntity.Z, (ushort)modelNumber, _targetCursorID, _targetCursorType));
            // ... and clear our targeting cursor.
            ClearTargetingWithoutTargetCancelPacket();
        }

        void mouseTargetingEventObject(AEntity selectedEntity)
        {
            // If we are passed a null object, keep targeting.
            if (selectedEntity == null)
                return;
            var serial = selectedEntity.Serial;
            // Send the targetting event back to the server
            if (serial.IsValid)
                _network.Send(new TargetObjectPacket(selectedEntity, _targetCursorID, _targetCursorType));
            else
            {
                var modelNumber = 0;
                if (selectedEntity is StaticItem) modelNumber = ((StaticItem)selectedEntity).ItemID;
                else modelNumber = 0;
                _network.Send(new TargetXYZPacket((short)selectedEntity.Position.X, (short)selectedEntity.Position.Y, (short)selectedEntity.Z, (ushort)modelNumber, _targetCursorID, _targetCursorType));
            }
            // Clear our target cursor.
            ClearTargetingWithoutTargetCancelPacket();
        }

        // ============================================================================================================
        // Interaction routines
        // ============================================================================================================

        void InternalRegisterInteraction()
        {
            _world.Interaction.OnPickupItem += PickUpItem;
            _world.Interaction.OnClearHolding += ClearHolding;
        }

        void InternalUnregisterInteraction()
        {
            _world.Interaction.OnPickupItem -= PickUpItem;
            _world.Interaction.OnClearHolding -= ClearHolding;
        }

        // ============================================================================================================
        // Pickup/Drop/Hold item routines
        // ============================================================================================================

        Item _heldItem;
        internal Item HeldItem
        {
            get { return _heldItem; }
            set
            {
                if (value == null && _heldItem != null)
                    _userInterface.RemoveInputBlocker(this);
                else if (value != null && _heldItem == null)
                    _userInterface.AddInputBlocker(this);
                _heldItem = value;
            }
        }

        Vector2Int _heldItemOffset = Vector2Int.zero;

        public bool IsHoldingItem
        {
            get { return HeldItem != null; }
        }

        /// <summary>
        /// Picks up an item. For stacks, picks up entire stack if shift is down or picking up from a corpse.
        /// Otherwise, shows "pick up how many?" gump unless amountToPickUp param is set or amount is 1. 
        /// </summary>
        void PickUpItem(Item item, int x, int y, int? amountToPickUp = null)
        {
            if (!_input.IsShiftDown && !amountToPickUp.HasValue && !(item is Corpse) && item.Amount > 1)
            {
                var gump = new SplitItemStackGump(item, new Vector2Int(x, y));
                _userInterface.AddControl(gump, _input.MousePosition.x - 80, _input.MousePosition.y - 40);
                _userInterface.AttemptDragControl(gump, _input.MousePosition, true);
            }
            else PickupItemWithoutAmountCheck(item, x, y, amountToPickUp.HasValue ? amountToPickUp.Value : item.Amount);
        }

        /// <summary>
        /// Picks up item/amount from stack. If item cannot be picked up, nothing happens. If item is within container,
        /// removes it from the containing entity. Informs server we picked up the item. Server can cancel pick up.
        /// Note: I am unsure what will happen if we can pick up an item and add to inventory before server can cancel.
        /// </summary>
        void PickupItemWithoutAmountCheck(Item item, int x, int y, int amount)
        {
            if (!item.TryPickUp())
                return;
            // Removing item from parent causes client "in range" check. Set position to parent entity position.
            if (item.Parent != null)
            {
                item.Position.Set(item.Parent.Position.X, item.Parent.Position.Y, item.Parent.Position.Z);
                if (item.Parent is Mobile)
                    (item.Parent as Mobile).RemoveItem(item.Serial);
                else if (item.Parent is ContainerItem)
                {
                    var parent = item.Parent;
                    if (parent is Corpse) (parent as Corpse).RemoveItem(item.Serial);
                    else (parent as ContainerItem).RemoveItem(item.Serial);
                }
                item.Parent = null;
            }
            RecursivelyCloseItemGumps(item);
            item.Amount = amount;
            HeldItem = item;
            _heldItemOffset = new Vector2Int(x, y);
            _network.Send(new PickupItemPacket(item.Serial, amount));
        }

        void RecursivelyCloseItemGumps(Item item)
        {
            _userInterface.RemoveControl<Gump>(item.Serial);
            if (item is ContainerItem)
                foreach (Item child in (item as ContainerItem).Contents)
                    RecursivelyCloseItemGumps(child);
        }

        void MergeHeldItem(AEntity target)
        {
            _network.Send(new DropItemPacket(HeldItem.Serial, 0xFFFF, 0xFFFF, 0, 0, target.Serial));
            ClearHolding();
        }

        void DropHeldItemToWorld(int X, int Y, int Z)
        {
            Serial serial;
            var entity = _world.Input.MousePick.MouseOverObject;
            if (entity is Item && ((Item)entity).ItemData.IsContainer)
            {
                serial = entity.Serial;
                X = Y = 0xFFFF;
                Z = 0;
            }
            else serial = Serial.World;
            _network.Send(new DropItemPacket(HeldItem.Serial, (ushort)X, (ushort)Y, (byte)Z, 0, serial));
            ClearHolding();
        }

        void DropHeldItemToContainer(ContainerItem container)
        {
            // get random coords and drop the item there.
            var bounds = ContainerData.Get(container.ItemID).Bounds;
            var x = Utility.RandomValue(bounds.Left, bounds.Right);
            var y = Utility.RandomValue(bounds.Top, bounds.Bottom);
            DropHeldItemToContainer(container, x, y);
        }

        void DropHeldItemToContainer(ContainerItem container, int x, int y)
        {
            var bounds = ContainerData.Get(container.ItemID).Bounds;
            var provider = Service.Get<IResourceProvider>();
            var itemTexture = provider.GetItemTexture(HeldItem.DisplayItemID);
            if (x < bounds.Left)
                x = bounds.Left;
            if (x > bounds.Right - itemTexture.width)
                x = bounds.Right - itemTexture.width;
            if (y < bounds.Top)
                y = bounds.Top;
            if (y > bounds.Bottom - itemTexture.height)
                y = bounds.Bottom - itemTexture.height;
            _network.Send(new DropItemPacket(HeldItem.Serial, (ushort)x, (ushort)y, 0, 0, container.Serial));
            ClearHolding();
        }

        void WearHeldItem()
        {
            _network.Send(new DropToLayerPacket(HeldItem.Serial, 0x00, WorldModel.PlayerSerial));
            ClearHolding();
        }

        void ClearHolding()
        {
            HeldItem = null;
        }
    }
}
