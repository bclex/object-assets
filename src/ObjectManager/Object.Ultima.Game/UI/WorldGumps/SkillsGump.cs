using OA.Core;
using OA.Core.Input;
using OA.Ultima.Player;
using OA.Ultima.UI.Controls;
using OA.Ultima.World;
using System.Text;

namespace OA.Ultima.UI.WorldGumps
{
    class SkillsGump : Gump
    {
        ExpandableScroll _background;
        HtmlGumpling _skillsHtml;
        bool _mustUpdateSkills = true;
        WorldModel _world;

        public SkillsGump()
            : base(0, 0)
        {
            IsMoveable = true;

            _world = Service.Get<WorldModel>();

            AddControl(_background = new ExpandableScroll(this, 0, 0, 200));
            _background.TitleGumpID = 0x834;

            AddControl(_skillsHtml = new HtmlGumpling(this, 32, 32, 240, 200 - 92, 0, 2, string.Empty));
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("skills");
            PlayerState.Skills.OnSkillChanged += OnSkillChanged;
        }

        public override void Dispose()
        {
            PlayerState.Skills.OnSkillChanged -= OnSkillChanged;
            base.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
            if (_mustUpdateSkills)
            {
                InitializeSkillsList();
                _mustUpdateSkills = false;
            }
            _skillsHtml.Height = Height - 92;
        }

        public override void OnHtmlInputEvent(string href, MouseEvent e)
        {
            if (e == MouseEvent.Click)
            {
                if (href.Substring(0, 6) == "skill=" || href.Substring(0, 9) == "skillbtn=")
                {
                    if (!int.TryParse(href.Substring(href.IndexOf('=') + 1), out int skillIndex))
                        return;
                    _world.Interaction.UseSkill(skillIndex);
                }
                else if (href.Substring(0, 10) == "skilllock=")
                {
                    if (!int.TryParse(href.Substring(10), out int skillIndex))
                        return;
                    _world.Interaction.ChangeSkillLock(PlayerState.Skills.SkillEntryByIndex(skillIndex));
                }
            }
            else if (e == MouseEvent.DragBegin)
            {
                if (href.Length >= 9 && href.Substring(0, 9) == "skillbtn=")
                {
                    if (!int.TryParse(href.Substring(9), out int skillIndex))
                        return;
                    var skill = PlayerState.Skills.SkillEntryByIndex(skillIndex);
                    var input = Service.Get<IInputService>();
                    var gump = new UseSkillButtonGump(skill);
                    UserInterface.AddControl(gump, input.MousePosition.X - 60, input.MousePosition.Y - 20);
                    UserInterface.AttemptDragControl(gump, input.MousePosition, true);
                }
            }
        }

        private void InitializeSkillsList()
        {
            var sb = new StringBuilder();
            foreach (var pair in PlayerState.Skills.List)
            {
                var skill = pair.Value;
                sb.Append(string.Format(skill.HasUseButton ? kSkillName_HasUseButton : kSkillName_NoUseButton, skill.Index, skill.Name));
                sb.Append(string.Format(kSkillValues[skill.LockType], skill.Value, skill.Index));
            }
            _skillsHtml.Text = sb.ToString();
        }

        private void OnSkillChanged(SkillEntry entry)
        {
            _mustUpdateSkills = true;
        }

        // 0 = skill index, 1 = skill name
        const string kSkillName_HasUseButton =
            "<left><a href='skillbtn={0}'><gumpimg src='2103' hoversrc='2104' activesrc='2103'/></a> " +
            "<a href='skill={0}' color='#5b4f29' hovercolor='#857951' activecolor='#402708' style='text-decoration=none'>{1}</a></left>";
        const string kSkillName_NoUseButton = "<left>   <medium color=#50422D>{1}</medium></left>";
        // 0 = skill value
        static string[] kSkillValues = {
            "<right><medium color=#50422D>{0:0.0}</medium><a href='skilllock={1}'><gumpimg src='2436'/></a> </right><br/>",
            "<right><medium color=#50422D>{0:0.0}</medium><a href='skilllock={1}'><gumpimg src='2438'/></a> </right><br/>",
            "<right><medium color=#50422D>{0:0.0}</medium><a href='skilllock={1}'><gumpimg src='2092'/></a> </right><br/>" };
    }
}
