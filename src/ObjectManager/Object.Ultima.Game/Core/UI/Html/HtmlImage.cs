namespace OA.Core.UI.Html
{
    public class HtmlImage
    {
        public Rectangle Area;
        public Texture2D Texture;
        public Texture2D TextureOver;
        public Texture2D TextureDown;
        public int LinkIndex = -1;

        public HtmlImage(Rectangle area, Texture2D image)
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
