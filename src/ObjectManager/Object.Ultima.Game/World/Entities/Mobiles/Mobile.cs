using OA.Ultima.Data;
using OA.Ultima.World.Entities.Items;
using OA.Ultima.World.Entities.Items.Containers;
using OA.Ultima.World.Entities.Mobiles.Animations;
using OA.Ultima.World.EntityViews;
using OA.Ultima.World.Maps;

namespace OA.Ultima.World.Entities.Mobiles
{
    /// <summary>
    /// A mobile object - monster, NPC, or player.
    /// TODO: This class needs a serious refactor.
    /// </summary>
    public class Mobile : AEntity
    {
        // ============================================================================================================
        // Ctor, Dispose, Update, and CreateView()
        // ============================================================================================================

        public Mobile(Serial serial, Map map)
            : base(serial, map)
        {
            Animation = new MobileAnimation(this);
            Equipment = new MobileEquipment(this);
            _movement = new MobileMovement(this);
            _movement.RequiresUpdate = true;
            Health = new CurrentMaxValue();
            Stamina = new CurrentMaxValue();
            Mana = new CurrentMaxValue();
            Followers = new CurrentMaxValue();
            Weight = new CurrentMaxValue();
            Flags = new MobileFlags();
        }

        public override void Dispose()
        {
            base.Dispose();
            Equipment.ClearEquipment();
        }

        protected override void OnTileChanged(int x, int y)
        {
            base.OnTileChanged(x, y);
            if (Tile == null)
                return;
            if (Body.IsHumanoid)
            {
                var foundChairData = false;
                var items = Tile.GetItemsBetweenZ(Z - 1, Z + 1); // match legacy - sit on chairs between z-1 and z+1.
                foreach (var i in items)
                {
                    Chairs.ChairData data;
                    if (Chairs.CheckItemAsChair(i.ItemID, out data))
                    {
                        foundChairData = true;
                        ChairData = data;
                        SittingZ = i.Z - Z;
                        Animation.Clear();
                        Tile.ForceSort();
                        break;
                    }
                }
                if (!foundChairData)
                    ChairData = Chairs.ChairData.Null;
            }
        }

        public override void Update(double frameMS)
        {
            if (WorldView.AllLabels)
                AddOverhead(MessageTypes.Label, Name, 3, NotorietyHue, false);
            if (!_movement.Position.IsNullPosition)
            {
                // update the movement and then animation objects
                _movement.Update(frameMS);
                // get/update the animation index.
                if (IsMoving)
                {
                    if (IsRunning) Animation.Animate(MobileAction.Run);
                    else Animation.Animate(MobileAction.Walk);
                }
                else if (!Animation.IsAnimating) Animation.Animate(MobileAction.Stand);
                Animation.Update(frameMS);
            }

            base.Update(frameMS);
        }

        protected override AEntityView CreateView()
        {
            return new MobileView(this);
        }

        /// <summary>
        /// Manages the animation state (what animation is playing, how far along is the animation, etc.) for this
        /// mobile view. Exposed as public so that we can receive animations from the server (e.g. emotes).
        /// </summary>
        public readonly MobileAnimation Animation;

        // ============================================================================================================
        // Movement and Facing
        // ============================================================================================================

        protected MobileMovement _movement;

        public Direction Facing
        {
            get { return _movement.Facing & Direction.FacingMask; }
            set { _movement.Facing = value; }
        }

        public Direction DrawFacing
        {
            get
            {
                var facing = Facing;
                if (!IsSitting)
                    return facing;
                return ChairData.GetSittingFacing(facing);
            }
        }

        public bool IsMoving { get { return _movement.IsMoving; } }

        public bool IsRunning { get { return _movement.IsRunning; } }

        public Position3D DestinationPosition
        {
            get
            {
                if (!IsMoving) return this.Position;
                else return _movement.GoalPosition;
            }
        }

        public bool IsSitting
        {
            get
            {
                if (!Animation.IsStanding || _movement.IsMoving) return false;
                if (ChairData.ItemID == Chairs.ChairData.Null.ItemID) return false;
                return true;
            }
        }

        public Chairs.ChairData ChairData = Chairs.ChairData.Null;
        public int SittingZ;

        // ============================================================================================================
        // Properties
        // ============================================================================================================

        public override string Name { get; set; }

        public MobileFlags Flags { get; set; }
        public bool PlayerCanChangeName;

        public int Strength, Dexterity, Intelligence, StatCap, Luck, Gold;
        public CurrentMaxValue Health, Stamina, Mana;
        public CurrentMaxValue Followers;
        public CurrentMaxValue Weight;
        public int ArmorRating, ResistFire, ResistCold, ResistPoison, ResistEnergy;
        public int DamageMin, DamageMax;

        public int Notoriety;

        public bool IsAlive
        {
            get { return Health.Current > 0; }
        }

        // ============================================================================================================
        // Equipment and Mount
        // ============================================================================================================

        public MobileEquipment Equipment;

        public bool IsMounted
        {
            get { return (Equipment[(int)EquipLayer.Mount] != null && Equipment[(int)EquipLayer.Mount].ItemID != 0); }
        }

        public int HairBodyID
        {
            get { return (Equipment[(int)EquipLayer.Hair] == null) ? 0 : Equipment[(int)EquipLayer.Hair].ItemData.AnimID; }
        }
        public int HairHue
        {
            get { return (Equipment[(int)EquipLayer.Hair] == null) ? 0 : Equipment[(int)EquipLayer.Hair].Hue; }
        }
        public int FacialHairBodyID
        {
            get { return (Equipment[(int)EquipLayer.FacialHair] == null) ? 0 : Equipment[(int)EquipLayer.FacialHair].ItemData.AnimID; }
        }
        public int FacialHairHue
        {
            get { return (Equipment[(int)EquipLayer.FacialHair] == null) ? 0 : Equipment[(int)EquipLayer.FacialHair].Hue; }
        }

        public int LightSourceBodyID
        {
            get
            {
                var bodyID = Equipment[(int)EquipLayer.TwoHanded] != null ? Equipment[(int)EquipLayer.TwoHanded].ItemData.AnimID : 0;
                if (bodyID >= 500 && bodyID <= 505) return bodyID;
                else return 0;
            }
        }

        public void WearItem(Item i, int slot)
        {
            Equipment[slot] = i;
            _onUpdated?.Invoke(this);
            if (slot == (int)EquipLayer.Mount)
            {
                // Do we do something here?
            }
        }

        public void RemoveItem(Serial serial)
        {
            Equipment.RemoveBySerial(serial);
            _onUpdated?.Invoke(this);
        }

        public Item GetItem(int slot)
        {
            if (slot == 0) return null;
            else return Equipment[slot];
        }

        public ContainerItem Backpack
        {
            get { return (ContainerItem)Equipment[(int)EquipLayer.Backpack]; }
        }

        public ContainerItem VendorShopContents
        {
            get { return (ContainerItem)Equipment[(int)EquipLayer.ShopBuy]; }
        }

        // ============================================================================================================
        // Appearance and Hues
        // ============================================================================================================

        static int[] _humanoidBodyIDs = {
            183, 184, 185, 186, // savages
            400, 401, 402, 403, // humans
            694, 695,
            744, 745,
            750, 751,
            987, 988, 990, 991,
            994,
            605, 606, 607, 608, // elves
            666, 667 // gargoyles. 666. Clever.
        };

        int _bodyID;
        public Body Body
        {
            get
            {
                if (_bodyID >= 402 && _bodyID <= 403) // 402 == 400 and 403 == 401
                    return _bodyID - 2;
                return _bodyID;
            }
            set
            {
                _bodyID = value;
                _onUpdated?.Invoke(this);
            }
        }

        int _hue;
        public override int Hue
        {
            get
            {
                if (Flags.IsHidden) return 0x3E7;
                if (Flags.IsPoisoned) return 0x1CE;
                return _hue;
            }
            set
            {
                _hue = value;
                _onUpdated?.Invoke(this);
            }
        }

        /// <summary>
        /// 0x1: Innocent (Blue)
        /// 0x2: Friend (Green)
        /// 0x3: Grey (Grey - Non Criminal)
        /// 0x4: Criminal (Grey)
        /// 0x5: Enemy (Orange)
        /// 0x6: Murderer (Red)
        /// 0x7: Invulnerable (Yellow)
        /// </summary>
        public int NotorietyHue
        {
            get
            {
                switch (Notoriety)
                {
                    case 0x01: return 0x059; // 0x1: Innocent (Blue)
                    case 0x02: return 0x03F; // 0x2: Friend (Green)
                    case 0x03: return 0x3B2;// Grey (Grey - Non Criminal)
                    case 0x04: return 0x3B2; // Criminal (Grey)
                    case 0x05: return 0x090; // Enemy (Orange)
                    case 0x06: return 0x022; // Murderer (Red)
                    case 0x07: return 0x035; // Invulnerable (Yellow)
                    default: return 0;
                }
            }
        }

        // ============================================================================================================
        // Animation and Movement
        // ============================================================================================================

        public void Animate(int action, int frameCount, int repeatCount, bool reverse, bool repeat, int delay)
        {
            Animation.Animate(action, frameCount, repeatCount, reverse, repeat, delay);
        }

        public void Mobile_AddMoveEvent(int x, int y, int z, int facing)
        {
            _movement.Mobile_ServerAddMoveEvent(x, y, z, facing);
        }

        public void Move_Instant(int x, int y, int z, int facing)
        {
            _movement.Move_Instant(x, y, z, facing);
        }

        public void PlayerMobile_ChangeFacing(Direction facing)
        {
            _movement.PlayerMobile_ChangeFacing(facing);
        }

        public void PlayerMobile_Move(Direction facing)
        {
            _movement.PlayerMobile_Move(facing);
        }

        public void PlayerMobile_MoveEventAck(int sequence)
        {
            _movement.PlayerMobile_MoveEventAck(sequence);
        }
        public void PlayerMobile_MoveEventRej(int sequenceID, int x, int y, int z, int direction)
        {
            _movement.PlayerMobile_MoveEventRej(sequenceID, x, y, z, direction);
        }

        public override string ToString()
        {
            return base.ToString() + " | " + Name;
        }
    }
}
