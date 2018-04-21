using OA.Core.UI.Html.Styles;

namespace OA.Core.UI.Html.Elements
{
    public class ImageElement : AElement
    {
        public HtmlImage AssociatedImage { get; set; }

        public int ImgSrc = -1;
        public int ImgSrcOver = -1;
        public int ImgSrcDown = -1;

        int _width, _height;

        public override int Width
        {
            set { _width = value; }
            get
            {
                if (_width != 0)
                    return _width;
                return AssociatedImage.Texture.Width;
            }
        }

        public override int Height
        {
            set { _height = value; }
            get
            {
                if (_height != 0)
                    return _height;
                return AssociatedImage.Texture.Height;
            }
        }

        public ImageTypes ImageType { get; private set; }

        public ImageElement(StyleState style, ImageTypes imageType = ImageTypes.UI)
            : base(style)
        {
            ImageType = imageType;
        }

        public override string ToString()
        {
            return string.Format("<img {0} {1}>", ImgSrc, ImageType.ToString());
        }

        public enum ImageTypes
        {
            UI,
            Item
        }
    }
}
