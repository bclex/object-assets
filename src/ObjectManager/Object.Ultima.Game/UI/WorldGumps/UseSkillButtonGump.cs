using OA.Core;
using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Player;
using OA.Ultima.UI.Controls;
using OA.Ultima.World;
using UnityEngine;

namespace OA.Ultima.UI.WorldGumps
{
    public class UseSkillButtonGump : Gump
    {
        // private variables
        readonly SkillEntry _skill;
        ResizePic[] _bg;
        HtmlGumpling _caption;
        bool _isMouseDown;
        // services
        readonly WorldModel _world;

        public UseSkillButtonGump(SkillEntry skill)
            : base(skill.ID, 0)
        {
            while (UserInterface.GetControl<UseSkillButtonGump>(skill.ID) != null)
                UserInterface.GetControl<UseSkillButtonGump>(skill.ID).Dispose();
            _skill = skill;
            _world = Service.Get<WorldModel>();
            IsMoveable = true;
            HandlesMouseInput = true;
            _bg = new ResizePic[3];
            _bg[0] = (ResizePic)AddControl(new ResizePic(this, 0, 0, 0x24B8, 120, 40));
            _bg[1] = (ResizePic)AddControl(new ResizePic(this, 0, 0, 0x24EA, 120, 40));
            _bg[2] = (ResizePic)AddControl(new ResizePic(this, 0, 0, 0x251C, 120, 40));
            _caption = (HtmlGumpling)AddControl(new HtmlGumpling(this, 0, 10, 120, 20, 0, 0, "<center>" + _skill.Name));
            for (var i = 0; i < 3; i++)
            {
                _bg[i].MouseDownEvent += EventMouseDown;
                _bg[i].MouseUpEvent += EventMouseUp;
                _bg[i].MouseClickEvent += EventMouseClick;
            }
        }

        public override void Dispose()
        {
            for (var i = 0; i < 3; i++)
            {
                _bg[i].MouseDownEvent -= EventMouseDown;
                _bg[i].MouseUpEvent -= EventMouseUp;
                _bg[i].MouseClickEvent -= EventMouseClick;
            }
            base.Dispose();
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            var isMouseOver = (_bg[0].IsMouseOver || _bg[1].IsMouseOver || _bg[2].IsMouseOver);
            if (_isMouseDown)
            {
                _bg[0].IsVisible = false;
                _bg[1].IsVisible = false;
                _bg[2].IsVisible = true;

            }
            else if (isMouseOver)
            {
                _bg[0].IsVisible = false;
                _bg[1].IsVisible = true;
                _bg[2].IsVisible = false;
            }
            else
            {
                _bg[0].IsVisible = true;
                _bg[1].IsVisible = false;
                _bg[2].IsVisible = false;
            }

            if (_isMouseDown)
                _caption.Position = new Vector2Int(_caption.Position.X, _caption.Position.Y + 1);

            base.Draw(spriteBatch, position, frameMS);

            if (_isMouseDown)
                _caption.Position = new Vector2Int(_caption.Position.X, _caption.Position.Y - 1);
        }

        private void EventMouseDown(AControl sender, int x, int y, MouseButton button)
        {
            OnMouseDown(x, y, button);
        }

        private void EventMouseUp(AControl sender, int x, int y, MouseButton button)
        {
            OnMouseUp(x, y, button);
        }

        private void EventMouseClick(AControl sender, int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
                OnMouseClick(x, y, button);
        }

        protected override void OnMouseDown(int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;
            _isMouseDown = true;
        }

        protected override void OnMouseUp(int x, int y, MouseButton button)
        {
            _isMouseDown = false;
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            if (button != MouseButton.Left)
                return;
            _world.Interaction.UseSkill(_skill.Index);
        }
    }
}