using OA.Core;
using OA.Core.UI;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Core.Network;
using OA.Ultima.Network.Client;
using OA.Ultima.Player;
using OA.Ultima.UI.Controls;
using OA.Ultima.World;
using OA.Ultima.World.Entities.Mobiles;
using UnityEngine;

namespace OA.Ultima.UI.WorldGumps
{
    class StatusGump : Gump
    {
        public static void Toggle(Serial serial)
        {
            var ui = Service.Get<UserInterfaceService>();
            if (ui.GetControl<StatusGump>() == null)
            {
                var client = Service.Get<INetworkClient>();
                client.Send(new MobileQueryPacket(MobileQueryPacket.StatusType.BasicStatus, serial));
                ui.AddControl(new StatusGump(), 200, 400);
            }
            else ui.RemoveControl<StatusGump>();
        }

        Mobile _mobile = WorldModel.Entities.GetPlayerEntity();
        double _refreshTime;

        TextLabelAscii[] _labels = new TextLabelAscii[(int)MobileStats.Max];

        private enum MobileStats
        {
            Name,
            Strength,
            Dexterity,
            Intelligence,
            HealthCurrent,
            HealthMax,
            StaminaCurrent,
            StaminaMax,
            ManaCurrent,
            ManaMax,
            Followers,
            WeightCurrent,
            WeightMax,
            StatCap,
            Luck,
            Gold,
            AR,
            RF,
            RC,
            RP,
            RE,
            Damage,
            Sex,
            Max
        }

        public StatusGump()
            : base(0, 0)
        {
            IsMoveable = true;
            if (PlayerState.ClientFeatures.AOS)
            {
                AddControl(new GumpPic(this, 0, 0, 0x2A6C, 0));

                _labels[(int)MobileStats.Name] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 54, 48, 1, 997, string.Format("<center>{0}", _mobile.Name))); // center doesn't work because textlabelascii shrinks to fit.
                _labels[(int)MobileStats.Strength] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 88, 76, 1, 997, _mobile.Strength.ToString()));
                _labels[(int)MobileStats.Dexterity] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 88, 104, 1, 997, _mobile.Dexterity.ToString()));
                _labels[(int)MobileStats.Intelligence] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 88, 132, 1, 997, _mobile.Intelligence.ToString()));

                _labels[(int)MobileStats.HealthCurrent] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 147, 71, 1, 997, _mobile.Health.Current.ToString()));
                _labels[(int)MobileStats.HealthMax] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 147, 82, 1, 997, _mobile.Health.Max.ToString()));

                _labels[(int)MobileStats.StaminaCurrent] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 147, 99, 1, 997, _mobile.Stamina.Current.ToString()));
                _labels[(int)MobileStats.StaminaMax] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 147, 110, 1, 997, _mobile.Stamina.Max.ToString()));

                _labels[(int)MobileStats.ManaCurrent] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 148, 127, 1, 997, _mobile.Mana.Current.ToString()));
                _labels[(int)MobileStats.ManaMax] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 148, 138, 1, 997, _mobile.Mana.Max.ToString()));

                _labels[(int)MobileStats.Followers] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 289, 132, 1, 997, ConcatCurrentMax(_mobile.Followers.Current, _mobile.Followers.Max)));

                _labels[(int)MobileStats.WeightCurrent] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 212, 127, 1, 997, _mobile.Weight.Current.ToString()));
                _labels[(int)MobileStats.WeightMax] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 212, 138, 1, 997, _mobile.Weight.Max.ToString()));

                _labels[(int)MobileStats.StatCap] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 219, 74, 1, 997, _mobile.StatCap.ToString()));
                _labels[(int)MobileStats.Luck] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 218, 102, 1, 997, _mobile.Luck.ToString()));
                _labels[(int)MobileStats.Gold] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 282, 102, 1, 997, _mobile.Gold.ToString()));

                _labels[(int)MobileStats.AR] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 352, 73, 1, 997, _mobile.ArmorRating.ToString()));
                _labels[(int)MobileStats.RF] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 352, 90, 1, 997, _mobile.ResistFire.ToString()));
                _labels[(int)MobileStats.RC] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 352, 105, 1, 997, _mobile.ResistCold.ToString()));
                _labels[(int)MobileStats.RP] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 352, 119, 1, 997, _mobile.ResistPoison.ToString()));
                _labels[(int)MobileStats.RE] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 352, 135, 1, 997, _mobile.ResistEnergy.ToString()));

                _labels[(int)MobileStats.Damage] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 277, 75, 1, 997, ConcatCurrentMax(_mobile.DamageMin, _mobile.DamageMax)));
            }
            else
            {
                AddControl(new GumpPic(this, 0, 0, 0x802, 0));

                _labels[(int)MobileStats.Name] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 84, 37, 6, 0, _mobile.Name));
                _labels[(int)MobileStats.Strength] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 84, 57, 6, 0, _mobile.Strength.ToString()));
                _labels[(int)MobileStats.Dexterity] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 84, 69, 6, 0, _mobile.Dexterity.ToString()));
                _labels[(int)MobileStats.Intelligence] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 84, 81, 6, 0, _mobile.Intelligence.ToString()));
                _labels[(int)MobileStats.Sex] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 84, 93, 6, 0, _mobile.Flags.IsFemale ? "F" : "M"));
                _labels[(int)MobileStats.AR] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 84, 105, 6, 0, _mobile.ArmorRating.ToString()));

                _labels[(int)MobileStats.HealthCurrent] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 172, 57, 6, 0, _mobile.Health.Current.ToString() + '/' + _mobile.Health.Max.ToString()));
                _labels[(int)MobileStats.ManaCurrent] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 172, 69, 6, 0, _mobile.Mana.Current.ToString() + '/' + _mobile.Mana.Max.ToString()));
                _labels[(int)MobileStats.StaminaCurrent] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 172, 81, 6, 0, _mobile.Stamina.Current.ToString() + '/' + _mobile.Stamina.Max.ToString()));
                _labels[(int)MobileStats.Gold] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 172, 93, 6, 0, _mobile.Gold.ToString()));
                _labels[(int)MobileStats.WeightCurrent] = (TextLabelAscii)AddControl(new TextLabelAscii(this, 172, 105, 6, 0, _mobile.Weight.Current.ToString() + '/' + _mobile.Weight.Max.ToString()));
            }
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("status");
            base.OnInitialize();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (_refreshTime + 0.5d < totalMS) //need to update
            {
                _refreshTime = totalMS;
                // we can just set these without checking if they've changed.
                // The label will only update if the value has changed.
                if (PlayerState.ClientFeatures.AOS)
                {
                    _labels[(int)MobileStats.Name].Text = string.Format("<center>{0}", _mobile.Name);
                    _labels[(int)MobileStats.Strength].Text = _mobile.Strength.ToString();
                    _labels[(int)MobileStats.Dexterity].Text = _mobile.Dexterity.ToString();
                    _labels[(int)MobileStats.Intelligence].Text = _mobile.Intelligence.ToString();

                    _labels[(int)MobileStats.HealthCurrent].Text = _mobile.Health.Current.ToString();
                    _labels[(int)MobileStats.HealthMax].Text = _mobile.Health.Max.ToString();

                    _labels[(int)MobileStats.StaminaCurrent].Text = _mobile.Stamina.Current.ToString();
                    _labels[(int)MobileStats.StaminaMax].Text = _mobile.Stamina.Max.ToString();

                    _labels[(int)MobileStats.ManaCurrent].Text = _mobile.Mana.Current.ToString();
                    _labels[(int)MobileStats.ManaMax].Text = _mobile.Mana.Max.ToString();

                    _labels[(int)MobileStats.Followers].Text = ConcatCurrentMax(_mobile.Followers.Current, _mobile.Followers.Max);

                    _labels[(int)MobileStats.WeightCurrent].Text = _mobile.Weight.Current.ToString();
                    _labels[(int)MobileStats.WeightMax].Text = _mobile.Weight.Max.ToString();

                    _labels[(int)MobileStats.StatCap].Text = _mobile.StatCap.ToString();
                    _labels[(int)MobileStats.Luck].Text = _mobile.Luck.ToString();
                    _labels[(int)MobileStats.Gold].Text = _mobile.Gold.ToString();

                    _labels[(int)MobileStats.AR].Text = _mobile.ArmorRating.ToString();
                    _labels[(int)MobileStats.RF].Text = _mobile.ResistFire.ToString();
                    _labels[(int)MobileStats.RC].Text = _mobile.ResistCold.ToString();
                    _labels[(int)MobileStats.RP].Text = _mobile.ResistPoison.ToString();
                    _labels[(int)MobileStats.RE].Text = _mobile.ResistEnergy.ToString();

                    _labels[(int)MobileStats.Damage].Text = ConcatCurrentMax(_mobile.DamageMin, _mobile.DamageMax);
                }
                else
                {
                    _labels[(int)MobileStats.Name].Text = _mobile.Name;
                    _labels[(int)MobileStats.Strength].Text = _mobile.Strength.ToString();
                    _labels[(int)MobileStats.Dexterity].Text = _mobile.Dexterity.ToString();
                    _labels[(int)MobileStats.Intelligence].Text = _mobile.Intelligence.ToString();

                    _labels[(int)MobileStats.HealthCurrent].Text = ConcatCurrentMax(_mobile.Health.Current, _mobile.Health.Max);
                    _labels[(int)MobileStats.StaminaCurrent].Text = ConcatCurrentMax(_mobile.Stamina.Current, _mobile.Stamina.Max);
                    _labels[(int)MobileStats.ManaCurrent].Text = ConcatCurrentMax(_mobile.Mana.Current, _mobile.Mana.Max);

                    _labels[(int)MobileStats.WeightCurrent].Text = _mobile.Weight.Current.ToString();

                    _labels[(int)MobileStats.Gold].Text = _mobile.Gold.ToString();

                    _labels[(int)MobileStats.AR].Text = _mobile.ArmorRating.ToString();

                    _labels[(int)MobileStats.Sex].Text = _mobile.Flags.IsFemale ? "F" : "M";
                }
            }

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            base.Draw(spriteBatch, position, frameMS);
        }

        private string ConcatCurrentMax(int min, int max)
        {
            return string.Format("{0}/{1}", min, max);
        }
    }
}