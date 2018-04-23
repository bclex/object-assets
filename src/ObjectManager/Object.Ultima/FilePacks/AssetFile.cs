using System;

namespace OA.Ultima.FilePacks
{
    public partial class AssetFile : IDisposable
    {
        public AssetFile(string searchPath)
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