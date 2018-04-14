using OA.Core.UI;
using OA.Ultima.Core.Graphics;
using OA.Ultima.UI.Controls;
using OA.Ultima.World.Entities;
using UnityEngine;

namespace OA.Ultima.UI
{
    class Tooltip
    {
        public string Caption { get; protected set; }

        RenderedText _renderedText;

        int _propertyListHash;
        AEntity _entity;

        public Tooltip(string caption)
        {
            _entity = null;
            Caption = caption;
        }

        public Tooltip(AEntity entity)
        {
            _entity = entity;
            _propertyListHash = _entity.PropertyList.Hash;
            Caption = _entity.PropertyList.Properties;
        }

        public void Dispose()
        {
            Caption = null;
        }

        public void Draw(SpriteBatchUI spriteBatch, int x, int y)
        {
            // determine if properties need to be updated.
            if (_entity != null && _propertyListHash != _entity.PropertyList.Hash)
            {
                _propertyListHash = _entity.PropertyList.Hash;
                Caption = _entity.PropertyList.Properties;
            }
            // update text if necessary.
            if (_renderedText == null)
                _renderedText = new RenderedText("<center>" + Caption, 300, true);
            else if (_renderedText.Text != "<center>" + Caption)
            {
                _renderedText = null;
                _renderedText = new RenderedText("<center>" + Caption, 300, true);
            }
            // draw checkered trans underneath.
            spriteBatch.Draw2DTiled(CheckerTrans.CheckeredTransTexture, new RectInt(x - 4, y - 4, _renderedText.Width + 8, _renderedText.Height + 8), Vector3.zero);
            // draw tooltip contents
            _renderedText.Draw(spriteBatch, new Vector2Int(x, y));
        }

        internal void UpdateEntity(AEntity entity)
        {
            if (_entity == null || _entity != entity || _propertyListHash != _entity.PropertyList.Hash)
            {
                _entity = entity;
                _propertyListHash = _entity.PropertyList.Hash;
                Caption = _entity.PropertyList.Properties;
            }
        }

        internal void UpdateCaption(string caption)
        {
            _entity = null;
            Caption = caption;
        }
    }
}
