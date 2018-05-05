using OA.Core;
using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Resources;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    /// <summary>
    /// A checkbox control.
    /// </summary>
    class CheckBox : AControl
    {
        Texture2DInfo _inactive, _active;
        bool isChecked;

        public bool IsChecked
        {
            get { return isChecked; }
            set { isChecked = value; }
        }

        CheckBox(AControl parent)
            : base(parent)
        {
            HandlesMouseInput = true;
        }

        public CheckBox(AControl parent, string[] arguements, string[] lines)
            : this(parent)
        {
            var x = int.Parse(arguements[1]);
            var y = int.Parse(arguements[2]);
            var inactiveID = int.Parse(arguements[3]);
            var activeID = int.Parse(arguements[4]);
            var initialState = int.Parse(arguements[5]) == 1;
            var switchID = int.Parse(arguements[6]);
            BuildGumpling(x, y, inactiveID, activeID, initialState, switchID);
        }

        public CheckBox(AControl parent, int x, int y, int inactiveID, int activeID, bool initialState, int switchID)
            : this(parent)
        {
            BuildGumpling(x, y, inactiveID, activeID, initialState, switchID);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            base.Draw(spriteBatch, position, frameMS);
            if (IsChecked && _active != null)
                spriteBatch.Draw2D(_active, new Vector3(position.x, position.y, 0), Vector3.zero);
            else if (!IsChecked && _inactive != null)
                spriteBatch.Draw2D(_inactive, new Vector3(position.x, position.y, 0), Vector3.zero);
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            IsChecked = !IsChecked;
        }

        void BuildGumpling(int x, int y, int inactiveID, int activeID, bool initialState, int switchID)
        {
            var provider = Service.Get<IResourceProvider>();
            _inactive = provider.GetUITexture(inactiveID);
            _active = provider.GetUITexture(activeID);
            Position = new Vector2Int(x, y);
            Size = new Vector2Int(_inactive.Width, _inactive.Height);
            IsChecked = initialState;
            GumpLocalID = switchID;
        }
    }
}