using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using UnityEngine;

namespace OA.Core.UI
{
    /// <summary>
    /// Displays a given string of html text.
    /// </summary>
    class RenderedText
    {
        const int DefaultRenderedTextWidth = 200;

        string _text;
        HtmlDocument _document;
        bool _mustRender;
        Texture2D _texture;
        bool _collapseContent;
        int _maxWidth;

        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _mustRender = true;
                    _text = value;
                    _document?.SetHtml(_text, MaxWidth, _collapseContent);
                }
            }
        }

        public int MaxWidth
        {
            get { return _maxWidth; }
            set
            {
                if (value <= 0)
                    value = DefaultRenderedTextWidth;
                if (_maxWidth != value)
                {
                    _mustRender = true;
                    _maxWidth = value;
                    _document?.SetHtml(_text, MaxWidth, _collapseContent);
                }
            }
        }

        public int Width
        {
            get
            {
                if (string.IsNullOrEmpty(Text))
                    return 0;
                return _document.Width;
            }
        }

        public int Height
        {
            get
            {
                if (string.IsNullOrEmpty(Text))
                    return 0;
                return _document.Height;
            }
        }

        public int MouseOverRegionID { get; set; } // not set by anything...


        public bool IsMouseDown { get; set; }// only used by HtmlGumpling

        public HtmlLinkList Regions => _document.Links;

        public Texture2D Texture
        {
            get
            {
                if (_mustRender)
                {
                    _texture = _document.Render();
                    _mustRender = false;
                }
                return _texture;
            }
        }

        public HtmlDocument Document => _document; // TODO: Remove this. Should not be publicly accessibly.

        public RenderedText(string text, int maxWidth = DefaultRenderedTextWidth, bool collapseContent = false)
        {
            Text = text;
            MaxWidth = maxWidth;
            _collapseContent = collapseContent;
            _document = new HtmlDocument(Text, MaxWidth, _collapseContent);
            _mustRender = true;
        }

        // ============================================================================================================
        // Draw methods
        // ============================================================================================================

        public void Draw(SpriteBatchUI sb, Vector2Int position, Vector3? hueVector = null)
        {
            Draw(sb, new RectInt(position.X, position.Y, Width, Height), 0, 0, hueVector);
        }

        public void Draw(SpriteBatchUI sb, RectInt destRectangle, int xScroll, int yScroll, Vector3? hueVector = null)
        {
            if (string.IsNullOrEmpty(Text))
                return;
            RectInt sourceRectangle;
            if (xScroll > Width || xScroll < -MaxWidth || yScroll > Height || yScroll < -Height)
                return;
            sourceRectangle.X = xScroll;
            sourceRectangle.Y = yScroll;
            var maxX = sourceRectangle.X + destRectangle.Width;
            if (maxX <= Width)
                sourceRectangle.Width = destRectangle.Width;
            else
            {
                sourceRectangle.Width = Width - sourceRectangle.X;
                destRectangle.Width = sourceRectangle.Width;
            }
            var maxY = sourceRectangle.Y + destRectangle.Height;
            if (maxY <= Height)
                sourceRectangle.Height = destRectangle.Height;
            else
            {
                sourceRectangle.Height = Height - sourceRectangle.Y;
                destRectangle.Height = sourceRectangle.Height;
            }
            sb.Draw2D(Texture, destRectangle, sourceRectangle, hueVector.HasValue ? hueVector.Value : Vector3.Zero);
            for (var i = 0; i < _document.Links.Count; i++)
            {
                var link = _document.Links[i];
                Vector2Int pos;
                RectInt srcRect;
                if (ClipRectangle(new Vector2Int(xScroll, yScroll), link.Area, destRectangle, out pos, out srcRect))
                    // only draw the font in a different color if this is a HREF region.
                    // otherwise it is a dummy region used to notify images that they are
                    // being mouse overed.
                    if (link.HREF != null)
                    {
                        var linkHue = 0;
                        if (link.Index == MouseOverRegionID)
                        {
                            if (IsMouseDown) linkHue = link.Style.ActiveColorHue;
                            else linkHue = link.Style.HoverColorHue;
                        }
                        else linkHue = link.Style.ColorHue;
                        sb.Draw2D(Texture, new Vector3(pos.X, pos.Y, 0), srcRect, Utility.GetHueVector(linkHue));
                    }
            }
            for (var i = 0; i < _document.Images.Count; i++)
            {
                var img = _document.Images[i];
                Vector2Int position;
                RectInt srcRect;
                if (ClipRectangle(new Vector2Int(xScroll, yScroll), img.Area, destRectangle, out position, out srcRect))
                {
                    var srcImage = new RectInt(srcRect.X - img.Area.X, srcRect.Y - img.Area.Y, srcRect.Width, srcRect.Height);
                    Texture2D texture = null;
                    // is the mouse over this image?
                    if (img.LinkIndex != -1 && img.LinkIndex == MouseOverRegionID)
                    {
                        if (IsMouseDown)
                            texture = img.TextureDown;
                        if (texture == null)
                            texture = img.TextureOver;
                        if (texture == null)
                            texture = img.Texture;
                    }
                    if (texture == null)
                        texture = img.Texture;
                    if (srcImage.Width > texture.Width)
                        srcImage.Width = texture.Width;
                    if (srcImage.Height > texture.Height)
                        srcImage.Height = texture.Height;
                    sb.Draw2D(texture, new Vector3(position.X, position.Y, 0), srcImage, Utility.GetHueVector(0, false, false, true));
                }
            }
        }

        bool ClipRectangle(Vector2Int offset, RectInt srcRect, RectInt clipTo, out Vector2Int posClipped, out RectInt srcClipped)
        {
            posClipped = new Vector2Int(clipTo.X + srcRect.X - offset.X, clipTo.Y + srcRect.Y - offset.Y);
            srcClipped = srcRect;
            RectInt dstClipped = srcRect;
            dstClipped.X += clipTo.X - offset.X;
            dstClipped.Y += clipTo.Y - offset.Y;
            if (dstClipped.Bottom < clipTo.Top)
                return false;
            if (dstClipped.Top < clipTo.Top)
            {
                srcClipped.Y += (clipTo.Top - dstClipped.Top);
                srcClipped.Height -= (clipTo.Top - dstClipped.Top);
                posClipped.Y += (clipTo.Top - dstClipped.Top);
            }
            if (dstClipped.Top > clipTo.Bottom)
                return false;
            if (dstClipped.Bottom > clipTo.Bottom)
                srcClipped.Height += (clipTo.Bottom - dstClipped.Bottom);
            if (dstClipped.Right < clipTo.Left)
                return false;
            if (dstClipped.Left < clipTo.Left)
            {
                srcClipped.X += (clipTo.Left - dstClipped.Left);
                srcClipped.Width -= (clipTo.Left - dstClipped.Left);
                posClipped.X += (clipTo.Left - dstClipped.Left);
            }
            if (dstClipped.Left > clipTo.Right)
                return false;
            if (dstClipped.Right > clipTo.Right)
                srcClipped.Width += (clipTo.Right - dstClipped.Right);
            return true;
        }
    }
}
