using OA.Ultima.Resources;
using System;

namespace OA.Ultima.FilePacks
{
    public partial class AssetFile : IDisposable
    {
        //readonly AnimationResource _animationResource = new AnimationResource(null);
        //readonly FontsResource _fontsResource = new FontsResource(null);
        readonly ArtMulResource _artmulResource = new ArtMulResource(null);
        readonly GumpMulResource _gumpMulResource = new GumpMulResource(null);
        readonly TexmapResource _texmapResource = new TexmapResource(null);

        public AssetFile()
        {
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        ~AssetFile()
        {
            Close();
        }

        public void Close()
        {
        }
    }
}