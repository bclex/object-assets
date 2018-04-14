using OA.Core;
using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Data;
using OA.Ultima.UI.Controls;
using OA.Ultima.World;
using UnityEngine;

namespace OA.Ultima.UI.WorldGumps
{
    public class UseSpellButtonGump : Gump
    {
        // private variables
        SpellDefinition _spell;
        GumpPic _spellButton;
        // services
        readonly WorldModel _world;

        public UseSpellButtonGump(SpellDefinition spell)
            : base(spell.ID, 0)
        {
            while (UserInterface.GetControl<UseSpellButtonGump>(spell.ID) != null)
                UserInterface.GetControl<UseSpellButtonGump>(spell.ID).Dispose();
            _spell = spell;
            _world = Service.Get<WorldModel>();
            IsMoveable = true;
            HandlesMouseInput = true;
            _spellButton = (GumpPic)AddControl(new GumpPic(this, 0, 0, spell.GumpIconSmallID, 0));
            _spellButton.HandlesMouseInput = true;
            _spellButton.MouseDoubleClickEvent += EventMouseDoubleClick;
        }

        public override void Dispose()
        {
            _spellButton.MouseDoubleClickEvent -= EventMouseDoubleClick;
            base.Dispose();
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            base.Draw(spriteBatch, position, frameMS);
        }

        private void EventMouseDoubleClick(AControl sender, int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;
            _world.Interaction.CastSpell(_spell.ID);
        }
    }
}