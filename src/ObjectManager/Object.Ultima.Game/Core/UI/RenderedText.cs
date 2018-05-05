using OA.Core.UI.Html;
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
        Texture2DInfo _texture;
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

        public Texture2DInfo Texture
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
            Draw(sb, new RectInt(position.x, position.y, Width, Height), 0, 0, hueVector);
        }

        public void Draw(SpriteBatchUI sb, RectInt destRectangle, int xScroll, int yScroll, Vector3? hueVector = null)
        {
            if (string.IsNullOrEmpty(Text))
                return;
            var sourceRectangle = new RectInt();
            if (xScroll > Width || xScroll < -MaxWidth || yScroll > Height || yScroll < -Height)
                return;
            sourceRectangle.x = xScroll;
            sourceRectangle.y = yScroll;
            var maxX = sourceRectangle.x + destRectangle.width;
            if (maxX <= Width)
                sourceRectangle.width = destRectangle.width;
            else
            {
                sourceRectangle.width = Width - sourceRectangle.x;
                destRectangle.width = sourceRectangle.width;
            }
            var maxY = sourceRectangle.y + destRectangle.height;
            if (maxY <= Height)
                sourceRectangle.height = destRectangle.height;
            else
            {
                sourceRectangle.height = Height - sourceRectangle.y;
                destRectangle.height = sourceRectangle.height;
            }
            sb.Draw2D(Texture, destRectangle, sourceRectangle, hueVector.HasValue ? hueVector.Value : Vector3.zero);
            for (var i = 0; i < _document.Links.Count; i++)
            {
                var link = _document.Links[i];
                if (ClipRectangle(new Vector2Int(xScroll, yScroll), link.Area, destRectangle, out Vector2Int pos, out RectInt srcRect))
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
                        sb.Draw2D(Texture, new Vector3(pos.x, pos.y, 0), srcRect, Utility.GetHueVector(linkHue));
                    }
            }
            for (var i = 0; i < _document.Images.Count; i++)
            {
                var img = _document.Images[i];
                if (ClipRectangle(new Vector2Int(xScroll, yScroll), img.Area, destRectangle, out Vector2Int position, out RectInt srcRect))
                {
                    var srcImage = new RectInt(srcRect.x - img.Area.X, srcRect.y - img.Area.Y, srcRect.width, srcRect.height);
                    Texture2DInfo texture = null;
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
                    if (srcImage.width > texture.Width)
                        srcImage.width = texture.Width;
                    if (srcImage.height > texture.Height)
                        srcImage.height = texture.Height;
                    sb.Draw2D(texture, new Vector3(position.x, position.y, 0), srcImage, Utility.GetHueVector(0, false, false, true));
                }
            }
        }

        bool ClipRectangle(Vector2Int offset, RectInt srcRect, RectInt clipTo, out Vector2Int posClipped, out RectInt srcClipped)
        {
            posClipped = new Vector2Int(clipTo.x + srcRect.x - offset.x, clipTo.y + srcRect.y - offset.y);
            srcClipped = srcRect;
            var dstClipped = srcRect;
            dstClipped.x += clipTo.x - offset.x;
            dstClipped.y += clipTo.y - offset.y;
            if (dstClipped.yMax < clipTo.y)
                return false;
            if (dstClipped.y < clipTo.y)
            {
                srcClipped.y += (clipTo.y - dstClipped.y);
                srcClipped.height -= (clipTo.y - dstClipped.y);
                posClipped.y += (clipTo.y - dstClipped.y);
            }
            if (dstClipped.y > clipTo.yMax)
                return false;
            if (dstClipped.yMax > clipTo.yMax)
                srcClipped.height += (clipTo.yMax - dstClipped.yMax);
            if (dstClipped.xMax < clipTo.x)
                return false;
            if (dstClipped.x < clipTo.x)
            {
                srcClipped.x += (clipTo.x - dstClipped.x);
                srcClipped.width -= (clipTo.x - dstClipped.x);
                posClipped.x += (clipTo.x - dstClipped.x);
            }
            if (dstClipped.x > clipTo.xMax)
                return false;
            if (dstClipped.xMax > clipTo.xMax)
                srcClipped.width += (clipTo.xMax - dstClipped.xMax);
            return true;
        }
    }
}
