using OA.Core;
using OA.Core.UI;
using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Resources;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    class PaperdollLargeUninteractable : AControl
    {
        public enum EquipSlots
        {
            Body = 0, First = Body,
            RightHand = 1,
            LeftHand = 2,
            Footwear = 3,
            Legging = 4,
            Shirt = 5,
            Head = 5,
            Gloves = 7,
            Ring = 8,
            Talisman = 9,
            Neck = 10,
            Hair = 11,
            Belt = 12,
            Chest = 13,
            Bracelet = 14,
            // skip 15, unused
            FacialHair = 16,
            Sash = 17,
            Earring = 18,
            Sleeves = 19,
            Back = 20,
            Backpack = 21,
            Robe = 22,
            Skirt = 23,
            // skip 24, inner legs (!!! do we really skip this?)
            Max = 23,
        }

        int[] _equipmentSlots = new int[(int)EquipSlots.Max];
        readonly int[] _hueSlots = new int[(int)EquipSlots.Max];

        bool _isFemale;
        public int Gender { set { _isFemale = (value == 1) ? true : false; } }
        bool _isElf;
        public int Race { set { _isElf = (value == 1) ? true : false; } }

        public bool IsCharacterCreation;

        public void SetSlotEquipment(EquipSlots slot, int gumpID)
        {
            _equipmentSlots[(int)slot] = gumpID;
        }

        public void SetSlotHue(EquipSlots slot, int gumpID)
        {
            _hueSlots[(int)slot] = gumpID;
        }

        public int this[EquipSlots slot]
        {
            set { _equipmentSlots[(int)slot] = value; }
            get { return _equipmentSlots[(int)slot]; }
        }

        PaperdollLargeUninteractable(AControl parent)
            : base(parent)
        {
        }

        public PaperdollLargeUninteractable(AControl parent, int x, int y)
            : this(parent)
        {
            Position = new Vector2Int(x, y);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            var slotsToDraw = { EquipSlots.Body, EquipSlots.Footwear, EquipSlots.Legging, EquipSlots.Shirt, EquipSlots.Hair, EquipSlots.FacialHair };
            for (var i = 0; i < slotsToDraw.Length; i++)
            {
                var bodyID = 0;
                var hue = hueSlot(slotsToDraw[i]);
                var hueGreyPixelsOnly = true;
                switch (slotsToDraw[i])
                {
                    case EquipSlots.Body:
                        if (_isElf) bodyID = _isFemale ? 1893 : 1894;
                        else bodyID = _isFemale ? 1888 : 1889;
                        break;
                    case EquipSlots.Footwear:
                        bodyID = _isFemale ? 1891 : 1890;
                        hue = 900;
                        break;
                    case EquipSlots.Legging:
                        bodyID = _isFemale ? 1892 : 1848;
                        hue = 348;
                        break;
                    case EquipSlots.Shirt:
                        bodyID = _isFemale ? 1812 : 1849;
                        hue = 792;
                        break;
                    case EquipSlots.Hair:
                        if (equipmentSlot(EquipSlots.Hair) != 0)
                        {
                            bodyID = _isFemale ?
                                HairStyles.FemaleGumpIDForCharacterCreationFromItemID(equipmentSlot(EquipSlots.Hair)) :
                                HairStyles.MaleGumpIDForCharacterCreationFromItemID(equipmentSlot(EquipSlots.Hair));
                            hueGreyPixelsOnly = false;
                        }
                        break;
                    case EquipSlots.FacialHair:
                        if (equipmentSlot(EquipSlots.FacialHair) != 0)
                        {
                            bodyID = _isFemale ?
                                0 : HairStyles.FacialHairGumpIDForCharacterCreationFromItemID(equipmentSlot(EquipSlots.FacialHair));
                            hueGreyPixelsOnly = false;
                        }
                        break;
                }

                if (bodyID != 0)
                {
                    // this is silly, we should be keeping a local copy of the body texture.
                    var provider = Service.Get<IResourceProvider>();
                    spriteBatch.Draw2D(provider.GetUITexture(bodyID), new Vector3(position.x, position.y, 0), Utility.GetHueVector(hue, hueGreyPixelsOnly, false, false));
                }
            }
        }

        int equipmentSlot(EquipSlots slotID)
        {
            return _equipmentSlots[(int)slotID];
        }

        int hueSlot(EquipSlots slotID)
        {
            return _hueSlots[(int)slotID];
        }
    }
}
