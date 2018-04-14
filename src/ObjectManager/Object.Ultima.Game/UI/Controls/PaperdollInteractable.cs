using OA.Core;
using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.World;
using OA.Ultima.World.Entities;
using OA.Ultima.World.Entities.Mobiles;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    class PaperdollInteractable : Gump
    {
        bool _isFemale;
        bool _isElf;
        GumpPicBackpack _backpack;
        WorldModel _world;

        public PaperdollInteractable(AControl parent, int x, int y, Mobile sourceEntity)
            : base(0, 0)
        {
            Parent = parent;
            Position = new Vector2Int(x, y);
            _isFemale = sourceEntity.Flags.IsFemale;
            SourceEntity = sourceEntity;
            _world = Service.Get<WorldModel>();
        }

        public override void Dispose()
        {
            _sourceEntity.ClearCallBacks(OnEntityUpdated, OnEntityDisposed);
            if (_backpack != null)
                _backpack.MouseDoubleClickEvent -= On_Dblclick_Backpack;
            base.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (_sourceEntity != null)
            {
                _isFemale = ((Mobile)_sourceEntity).Flags.IsFemale;
                _isElf = false;
            }
            base.Update(totalMS, frameMS);
        }

        void OnEntityUpdated(AEntity entity)
        {
            ClearControls();
            // Add the base gump - the semi-naked paper doll.
            if (true)
            {
                var bodyID = 12 + (_isElf ? 2 : 0) + (_isFemale ? 1 : 0); // ((Mobile)m_sourceEntity).BodyID;
                GumpPic paperdoll = (GumpPic)AddControl(new GumpPic(this, 0, 0, bodyID, ((Mobile)_sourceEntity).Hue));
                paperdoll.HandlesMouseInput = true;
                paperdoll.IsPaperdoll = true;
            }
            // Loop through the items on the mobile and create the gump pics.
            for (var i = 0; i < _drawOrder.Length; i++)
            {
                var item = ((Mobile)_sourceEntity).GetItem((int)_drawOrder[i]);
                if (item == null)
                    continue;
                var canPickUp = true;
                switch (_drawOrder[i])
                {
                    case PaperDollEquipSlots.FacialHair:
                    case PaperDollEquipSlots.Hair:
                        canPickUp = false;
                        break;
                    default:
                        break;
                }
                AddControl(new ItemGumplingPaperdoll(this, 0, 0, item));
                ((ItemGumplingPaperdoll)LastControl).SlotIndex = (int)i;
                ((ItemGumplingPaperdoll)LastControl).IsFemale = _isFemale;
                ((ItemGumplingPaperdoll)LastControl).CanPickUp = canPickUp;
            }
            // If this object has a backpack, add it last.
            if (((Mobile)_sourceEntity).GetItem((int)PaperDollEquipSlots.Backpack) != null)
            {
                var backpack = ((Mobile)_sourceEntity).GetItem((int)PaperDollEquipSlots.Backpack);
                AddControl(_backpack = new GumpPicBackpack(this, -7, 0, backpack));
                _backpack.HandlesMouseInput = true;
                _backpack.MouseDoubleClickEvent += On_Dblclick_Backpack;
            }
        }

        void OnEntityDisposed(AEntity entity)
        {
            Dispose();
        }

        void On_Dblclick_Backpack(AControl control, int x, int y, MouseButton button)
        {
            var backpack = ((Mobile)_sourceEntity).Backpack;
            _world.Interaction.DoubleClick(backpack);
        }

        AEntity _sourceEntity;
        public AEntity SourceEntity
        {
            set
            {
                if (value != _sourceEntity)
                {
                    if (_sourceEntity != null)
                    {
                        _sourceEntity.ClearCallBacks(OnEntityUpdated, OnEntityDisposed);
                        _sourceEntity = null;
                    }
                    if (value is Mobile)
                    {
                        _sourceEntity = value;
                        // update the gump
                        OnEntityUpdated(_sourceEntity);
                        // if the entity changes in the future, update the gump again
                        _sourceEntity.SetCallbacks(OnEntityUpdated, OnEntityDisposed);
                    }
                }
            }
            get { return _sourceEntity; }
        }

        enum PaperDollEquipSlots
        {
            Body = 0,
            RightHand = 1,
            LeftHand = 2,
            Footwear = 3,
            Legging = 4,
            Shirt = 5,
            Head = 6,
            Gloves = 7,
            Ring = 8,
            Talisman = 9,
            Neck = 10,
            Hair = 11,
            Belt = 12,
            Chest = 13,
            Bracelet = 14,
            Unused = 15,
            FacialHair = 16,
            Sash = 17,
            Earring = 18,
            Sleeves = 19,
            Back = 20,
            Backpack = 21,
            Robe = 22,
            Skirt = 23,
            // skip 24, inner legs (!!! do we really skip this?)
        }

        static PaperDollEquipSlots[] _drawOrder = {
            PaperDollEquipSlots.Footwear,
            PaperDollEquipSlots.Legging,
            PaperDollEquipSlots.Shirt,
            PaperDollEquipSlots.Sleeves,
            PaperDollEquipSlots.Gloves,
            PaperDollEquipSlots.Ring,
            PaperDollEquipSlots.Talisman,
            PaperDollEquipSlots.Neck,
            PaperDollEquipSlots.Belt,
            PaperDollEquipSlots.Chest,
            PaperDollEquipSlots.Bracelet,
            PaperDollEquipSlots.Hair,
            PaperDollEquipSlots.FacialHair,
            PaperDollEquipSlots.Head,
            PaperDollEquipSlots.Sash,
            PaperDollEquipSlots.Earring,
            PaperDollEquipSlots.Back,
            PaperDollEquipSlots.Skirt,
            PaperDollEquipSlots.Robe,
            PaperDollEquipSlots.LeftHand,
            PaperDollEquipSlots.RightHand
        };
    }
}
