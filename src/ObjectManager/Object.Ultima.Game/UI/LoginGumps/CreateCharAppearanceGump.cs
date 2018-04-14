using OA.Core;
using OA.Ultima.Login.Data;
using OA.Ultima.Resources;
using OA.Ultima.UI.Controls;
using System;

namespace OA.Ultima.UI.LoginGumps
{
    class CreateCharAppearanceGump : Gump
    {
        enum Buttons
        {
            BackButton,
            ForwardButton,
            QuitButton
        }

        event Action _onForward;
        event Action _onBackward;

        public string Name { get { return _txtName.Text; } set { _txtName.Text = value; } }
        public int Gender { get { return _gender.Index; } set { } }
        public int Race { get { return 1; } set { } } // hard coded to human
        public int HairID
        {
            get { return (Gender == 0) ? HairStyles.MaleIDs[_hairMale.Index] : HairStyles.FemaleIDs[_hairFemale.Index]; }
            set
            {
                for (var i = 0; i < 10; i++)
                    if (value == ((Gender == 0) ? HairStyles.MaleIDs[i] : HairStyles.FemaleIDs[i]))
                    {
                        _hairMale.Index = i;
                        _hairFemale.Index = i;
                        break;
                    }
            }
        }
        public int FacialHairID
        {
            get { return (Gender == 0) ? HairStyles.FacialHairIDs[_facialHairMale.Index] : 0; }
            set
            {
                for (var i = 0; i < 8; i++)
                    if (value == HairStyles.FacialHairIDs[i])
                    {
                        _facialHairMale.Index = i;
                        break;
                    }
            }
        }
        public int SkinHue
        {
            get { return _skinHue.HueValue; }
            set { _skinHue.HueValue = value; }
        }
        public int HairHue
        {
            get { return _hairHue.HueValue; }
            set { _hairHue.HueValue = value; }
        }
        public int FacialHairHue
        {
            get { return (Gender == 0) ? _facialHairHue.HueValue : 0; }
            set { _facialHairHue.HueValue = value; }
        }

        internal void RestoreData(CreateCharacterData m_Data)
        {
            Name = m_Data.Name;
            Gender = m_Data.Gender;
            HairID = m_Data.HairStyleID;
            FacialHairID = m_Data.FacialHairStyleID;
            SkinHue = m_Data.SkinHue;
            HairHue = m_Data.HairHue;
            FacialHairHue = m_Data.FacialHairHue;
        }

        TextEntry _txtName;
        DropDownList _gender;
        DropDownList _hairMale;
        DropDownList _facialHairMale;
        DropDownList _hairFemale;
        ColorPicker _skinHue;
        ColorPicker _hairHue;
        ColorPicker _facialHairHue;
        PaperdollLargeUninteractable _paperdoll;

        public CreateCharAppearanceGump(Action onForward, Action onBackward)
            : base(0, 0)
        {
            _onForward = onForward;
            _onBackward = onBackward;

            // get the resource provider
            var provider = Service.Get<IResourceProvider>();

            // backdrop
            AddControl(new GumpPicTiled(this, 0, 0, 800, 600, 9274));
            AddControl(new GumpPic(this, 0, 0, 5500, 0));
            // character name 
            AddControl(new GumpPic(this, 280, 53, 1801, 0));
            _txtName = new TextEntry(this, 238, 70, 234, 20, 0, 0, 29, string.Empty);
            _txtName.LeadingHtmlTag = "<span color='#000' style='font-family:uni0;'>";
            AddControl(new ResizePic(this, _txtName));
            AddControl(_txtName);
            // character window
            AddControl(new GumpPic(this, 238, 98, 1800, 0));
            // paperdoll
            _paperdoll = new PaperdollLargeUninteractable(this, 237, 97);
            _paperdoll.IsCharacterCreation = true;
            AddControl(_paperdoll);

            // left option window
            AddControl(new ResizePic(this, 82, 125, 3600, 151, 310));
            // this is the place where you would put the race selector.
            // if you do add it, move everything else in this left window down by 45 pixels
            // gender
            AddControl(new TextLabelAscii(this, 100, 141, 9, 2036, provider.GetString(3000120)), 1);
            AddControl(_gender = new DropDownList(this, 97, 154, 122, new string[] { provider.GetString(3000118), provider.GetString(3000119) }, 2, 0, false));
            // hair (male)
            AddControl(new TextLabelAscii(this, 100, 186, 9, 2036, provider.GetString(3000121)), 1);
            AddControl(_hairMale = new DropDownList(this, 97, 199, 122, HairStyles.MaleHairNames, 6, 0, false), 1);
            // facial hair (male)
            AddControl(new TextLabelAscii(this, 100, 231, 9, 2036, provider.GetString(3000122)), 1);
            AddControl(_facialHairMale = new DropDownList(this, 97, 244, 122, HairStyles.FacialHair, 6, 0, false), 1);
            // hair (female)
            AddControl(new TextLabelAscii(this, 100, 186, 9, 2036, provider.GetString(3000121)), 2);
            AddControl(_hairFemale = new DropDownList(this, 97, 199, 122, HairStyles.FemaleHairNames, 6, 0, false), 2);

            // right option window
            AddControl(new ResizePic(this, 475, 125, 3600, 151, 310));
            // skin tone
            AddControl(new TextLabelAscii(this, 489, 141, 9, 2036, provider.GetString(3000183)));
            AddControl(_skinHue = new ColorPicker(this, new Rectangle(490, 154, 120, 24), new Rectangle(490, 140, 120, 280), 7, 8, Data.Hues.SkinTones));
            // hair color
            AddControl(new TextLabelAscii(this, 489, 186, 9, 2036, provider.GetString(3000184)));
            AddControl(_hairHue = new ColorPicker(this, new Rectangle(490, 199, 120, 24), new Rectangle(490, 140, 120, 280), 8, 6, Data.Hues.HairTones));
            // facial hair color (male)
            AddControl(new TextLabelAscii(this, 489, 231, 9, 2036, provider.GetString(3000185)), 1);
            AddControl(_facialHairHue = new ColorPicker(this, new Rectangle(490, 244, 120, 24), new Rectangle(490, 140, 120, 280), 8, 6, Data.Hues.HairTones), 1);

            // back button
            AddControl(new Button(this, 586, 435, 5537, 5539, ButtonTypes.Activate, 0, (int)Buttons.BackButton), 0);
            ((Button)LastControl).GumpOverID = 5538;
            // forward button
            AddControl(new Button(this, 610, 435, 5540, 5542, ButtonTypes.Activate, 0, (int)Buttons.ForwardButton), 0);
            ((Button)LastControl).GumpOverID = 5541;
            // quit button
            AddControl(new Button(this, 554, 2, 5513, 5515, ButtonTypes.Activate, 0, (int)Buttons.QuitButton));
            ((Button)LastControl).GumpOverID = 5514;

            IsUncloseableWithRMB = true;
        }

        internal void SaveData(CreateCharacterData data)
        {
            data.HasAppearanceData = true;
            data.Name = Name;
            data.Gender = Gender;
            data.HairStyleID = HairID;
            data.FacialHairStyleID = FacialHairID;
            data.SkinHue = SkinHue;
            data.HairHue = HairHue;
            data.FacialHairHue = FacialHairHue;
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);

            // show different controls based on what gender we're looking at.
            // Also copy over the hair id to facilitate easy switching between male and female appearances.
            if (_gender.Index == 0)
            {
                ActivePage = 1;
                _hairFemale.Index = _hairMale.Index;
            }
            else
            {
                ActivePage = 2;
                _hairMale.Index = _hairFemale.Index;
            }
            // update the paperdoll
            _paperdoll.Gender = _gender.Index;
            _paperdoll.SetSlotHue(PaperdollLargeUninteractable.EquipSlots.Body, SkinHue);
            _paperdoll.SetSlotEquipment(PaperdollLargeUninteractable.EquipSlots.Hair, HairID);
            _paperdoll.SetSlotHue(PaperdollLargeUninteractable.EquipSlots.Hair, HairHue);
            _paperdoll.SetSlotEquipment(PaperdollLargeUninteractable.EquipSlots.FacialHair, FacialHairID);
            _paperdoll.SetSlotHue(PaperdollLargeUninteractable.EquipSlots.FacialHair, FacialHairHue);
        }

        public override void OnButtonClick(int buttonID)
        {
            switch ((Buttons)buttonID)
            {
                case Buttons.BackButton:
                    _onBackward();
                    break;
                case Buttons.ForwardButton:
                    _onForward();
                    break;
                case Buttons.QuitButton:
                    Service.Get<UltimaGame>().Quit();
                    break;
            }
        }
    }
}
