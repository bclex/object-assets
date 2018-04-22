using OA.Core.UI.Html.Elements;
using OA.Core.UI.Html.Styles;
using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using UnityEngine;

namespace OA.Core.UI.Html
{
    class HtmlRenderer
    {
        /// <summary>
        /// Renders all the elements in the root branch. At the same time, also sets areas for regions and href links.
        /// TODO: code for setting areas for regions / hrefs belongs in layout code in HtmlDocument.
        /// </summary>
        public Texture2D Render(BlockElement root, int ascender, HtmlLinkList links)
        {
            var sb = Service.Get<SpriteBatchUI>();
            if (root == null || root.Width == 0 || root.Height == 0) // empty text string
                return new Texture2D(1, 1);
            var pixels = new uint[root.Width * root.Height];
            if (root.Err_Cant_Fit_Children)
            {
                for (var i = 0; i < pixels.Length; i++)
                    pixels[i] = 0xffffff00;
                Utils.Error("Err: Block can't fit children.");
            }
            else
            {
                unsafe
                {
                    fixed (uint* ptr = pixels)
                    {
                        DoRenderBlock(root, ascender, links, ptr, root.Width, root.Height);
                    }
                }
            }

            var texture = new Texture2D(root.Width, root.Height, TextureFormat.RGBA32, false);
            //texture.SetData(pixels);
            return texture;
        }

        unsafe void DoRenderBlock(BlockElement root, int ascender, HtmlLinkList links, uint* ptr, int width, int height)
        {
            foreach (var e in root.Children)
            {
                var x = e.Layout_X;
                var y = e.Layout_Y - ascender; // ascender is always negative.
                StyleState style = e.Style;
                if (e is CharacterElement)
                {
                    var font = style.Font;
                    var character = font.GetCharacter((e as CharacterElement).Character);
                    // HREF links should be colored white, because we will hue them at runtime.
                    var color = style.IsHREF ? 0xFFFFFFFF : Utility.UintFromColor(style.Color);
                    character.WriteToBuffer(ptr, x, y, width, height, font.Baseline, style.IsBold, style.IsItalic, style.IsUnderlined, style.DrawOutline, color, 0xFF000008);
                    // offset y by ascender for links...
                    if (character.YOffset < 0)
                    {
                        y += character.YOffset;
                        height -= character.YOffset;
                    }
                }
                else if (e is ImageElement)
                {
                    var image = (e as ImageElement);
                    image.AssociatedImage.Area = new RectInt(x, y, image.Width, image.Height);
                    if (style.IsHREF)
                    {
                        links.AddLink(style, new RectInt(x, y, e.Width, e.Height));
                        image.AssociatedImage.LinkIndex = links.Count;
                    }
                }
                else if (e is BlockElement)
                    DoRenderBlock(e as BlockElement, ascender, links, ptr, width, height);
                // set href link regions
                if (style.IsHREF)
                    links.AddLink(style, new RectInt(x, y, e.Width, e.Height));
            }
        }
    }
}
