using OA.Core;
using OA.Core.UI;
using OA.Ultima.Core.Graphics;
using OA.Ultima.UI.Controls;
using OA.Ultima.World;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Ultima.UI
{
    /// <summary>
    /// The base class that encapsulates Gump functionality. All Gumps should inherit from this class or a child thereof.
    /// </summary>
    public class Gump : AControl
    {
        /// <summary>
        /// If this is true, SaveGump() will be called when the World model is stopped, and LoadGump() will be called when the World model restarts.
        /// </summary>
        public bool SaveOnWorldStop { get; protected set; }

        /// <summary>
        /// If true, gump will not be moved by mouse movement, even if IsMoveable is true.
        /// </summary>
        public bool BlockMovement { get; set; }

        public override bool IsMoveable
        {
            get { return !BlockMovement && base.IsMoveable; }
            set { base.IsMoveable = value; }
        }

        public Gump(int localID, int gumpTypeID)
            : base(null)
        {
            GumpLocalID = localID;
            GumpServerTypeID = gumpTypeID;
        }

        public Gump(int localID, int gumpTypeID, string[] pieces, string[] textlines)
            : this(localID, gumpTypeID)
        {
            // Add any gump pieces that have been given to the gump...
            GumpBuilder.BuildGump(this, pieces, textlines);
        }

        public override void Dispose()
        {
            SavePosition();
            base.Dispose();
        }

        public override string ToString()
        {
            return GetType().ToString();
        }

        public override void Update(double totalMS, double frameMS)
        {
            // If page = 0, then we've just created this page. We initialize activepage to 1.
            // This triggers the additional functionality in Control.ActivePage.Set().
            if (ActivePage == 0)
                ActivePage = 1;

            // Update the Controls...
            base.Update(totalMS, frameMS);

            CheckRestoreSavedPosition();
        }

        public override void OnButtonClick(int buttonID)
        {
            if (GumpLocalID != 0)
            {
                if (buttonID == 0) // cancel
                {
                    var world = Service.Get<WorldModel>();
                    world.Client.SendGumpMenuSelect(GumpLocalID, GumpServerTypeID, buttonID, null, null);
                }
                else
                {
                    var switchIDs = new List<int>();
                    foreach (var control in Children)
                    {
                        if (control is CheckBox && (control as CheckBox).IsChecked)
                            switchIDs.Add(control.GumpLocalID);
                        else if (control is RadioButton && (control as RadioButton).IsChecked)
                            switchIDs.Add(control.GumpLocalID);
                    }
                    var textEntries = new List<Tuple<short, string>>();
                    foreach (var control in Children)
                        if (control is TextEntry)
                            textEntries.Add(new Tuple<short, string>((short)control.GumpLocalID, (control as TextEntry).Text));
                    var world = Service.Get<WorldModel>();
                    world.Client.SendGumpMenuSelect(GumpLocalID, GumpServerTypeID, buttonID, switchIDs.ToArray(), textEntries.ToArray());
                }
                Dispose();
            }
        }

        protected override void CloseWithRightMouseButton()
        {
            if (IsUncloseableWithRMB)
                return;
            // send cancel message for server gump
            if (GumpServerTypeID != 0)
                OnButtonClick(0);
            base.CloseWithRightMouseButton();
        }

        public override void ChangePage(int pageIndex)
        {
            // For a gump, Page is the page that is drawing.
            ActivePage = pageIndex;
        }

        protected string GetTextEntry(int entryID)
        {
            foreach (AControl c in Children)
                if (c.GetType() == typeof(TextEntry))
                {
                    var g = (TextEntry)c;
                    if (g.EntryID == entryID)
                        return g.Text;
                }
            return string.Empty;
        }

        protected override void OnMove()
        {
            var sb = Service.Get<SpriteBatchUI>();
            var position = Position;

            var halfWidth = Width / 2;
            var halfHeight = Height / 2;

            if (X < -halfWidth)
                position.x = -halfWidth;
            if (Y < -halfHeight)
                position.y = -halfHeight;
            if (X > sb.GraphicsDevice.Viewport.Width - halfWidth)
                position.x = sb.GraphicsDevice.Viewport.Width - halfWidth;
            if (Y > sb.GraphicsDevice.Viewport.Height - halfHeight)
                position.y = sb.GraphicsDevice.Viewport.Height - halfHeight;

            Position = position;
        }

        #region Position Save and Restore
        bool _willSavePosition;
        bool _willOffsetNextPosition;
        string _savePositionName;
        bool _hasRestoredPosition;

        static Vector2Int _savePositionOffsetAmount = new Vector2Int(24, 24);

        protected void SetSavePositionName(string positionName, bool offsetNext = false)
        {
            if (positionName != null)
            {
                _willSavePosition = true;
                _savePositionName = positionName;
                _willOffsetNextPosition = offsetNext;
            }
        }

        private void CheckRestoreSavedPosition()
        {
            if (!_hasRestoredPosition && _willSavePosition && _savePositionName != null)
            {
                var gumpPosition = UltimaGameSettings.Gumps.GetLastPosition(_savePositionName, Position);
                Position = gumpPosition;
                if (_willOffsetNextPosition)
                    SavePosition();
            }
            _hasRestoredPosition = true;
        }

        private void SavePosition()
        {
            if (_willSavePosition && _savePositionName != null)
            {
                var savePosition = Position;
                if (_willOffsetNextPosition)
                {
                    savePosition.x += _savePositionOffsetAmount.x;
                    savePosition.y += _savePositionOffsetAmount.y;
                }
                UltimaGameSettings.Gumps.SetLastPosition(_savePositionName, savePosition);

            }
        }
        #endregion

        /// <summary>
        /// Called when a gump asks to be restored on login. Should return a dictionary of data needed to restore the gump. Return false to not save this gump.
        /// </summary>
        /// <returns></returns>
        public virtual bool SaveGump(out Dictionary<string, object> data)
        {
            data = null;
            return false;
        }

        /// <summary>
        /// Called to restore a gump that asked to be restored on login.
        /// </summary>
        /// <param name="data">A dictionary of data needed to restore the gump.</param>
        /// <returns>Return false to cancel restoring this gump.</returns>
        public virtual bool RestoreGump(Dictionary<string, object> data)
        {
            return false;
        }
    }
}
