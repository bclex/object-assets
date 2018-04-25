using UnityEngine;

namespace OA.Core.UI.Html
{
    public class HtmlImage
    {
        public RectInt Area;
        public Texture2DInfo Texture;
        public Texture2DInfo TextureOver;
        public Texture2DInfo TextureDown;
        public int LinkIndex = -1;

        public HtmlImage(RectInt area, Texture2DInfo image)
        {
            Area = area;
            Texture = image;
        }

        public void Dispose()
        {
            Texture = null;
            TextureOver = null;
            TextureDown = null;
        }
    }
}
