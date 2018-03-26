using OA.Ultima.Core.ComponentModel;

namespace OA.Ultima.Configuration.Properties
{
    /// <summary>
    /// A class that describes a resolution width height pair.
    /// </summary>
    public class ResolutionProperty : NotifyPropertyChangedBase
    {
        int _height;
        int _width;

        public ResolutionProperty()
        {
            Width = 800;
            Height = 600;
        }

        public ResolutionProperty(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value); }
        }

        public int Width
        {
            get { return _width; }
            set { SetProperty(ref _width, value); }
        }

        public override string ToString()
        {
            return string.Format("{0}x{1}", _width, _height);
        }
    }
}