using UnityEngine;

namespace OA.Core.UI.Html
{
    public class HtmlImage
    {
        public RectInt Area;
        public Texture2D Texture;
        public Texture2D TextureOver;
        public Texture2D TextureDown;
        public int LinkIndex = -1;

        public HtmlImage(RectInt area, Texture2D image)
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
