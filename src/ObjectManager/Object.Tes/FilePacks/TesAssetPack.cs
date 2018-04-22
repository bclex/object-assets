using OA.Tes.FilePacks;
using OA.Tes.Formats;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace OA.Tes.FilePacks
{
    public class TesAssetPack : BsaMultiFile, IAssetPack
    {
        TextureManager _textureManager;
        MaterialManager _materialManager;
        NifManager _nifManager;
        string _directory;
        string _webPath;

        public TesAssetPack(string searchPath, string webPath)
            : base(searchPath) //: base(filePath != null && File.Exists(filePath) ? filePath : null)
        {
            //_directory = searchPath != null && Directory.Exists(searchPath) ? searchPath : null;
            _directory = null;
            _webPath = webPath;
            _textureManager = new TextureManager(this);
            _materialManager = new MaterialManager(_textureManager);
            var markerLayer = 0; // LayerMask.NameToLayer("Marker");
            _nifManager = new NifManager(this, _materialManager, markerLayer);
        }

        public Texture2D LoadTexture(string texturePath, bool flipVertically = false)
        {
            return _textureManager.LoadTexture(texturePath, flipVertically);
        }

        public void PreloadTextureAsync(string texturePath)
        {
            _textureManager.PreloadTextureFileAsync(texturePath);
        }

        public GameObject CreateObject(string filePath)
        {
            return _nifManager.InstantiateNif(filePath);
        }

        public void PreloadObjectAsync(string filePath)
        {
            _nifManager.PreloadNifFileAsync(filePath);
        }

        public override bool ContainsFile(string filePath)
        {
            if (_directory == null && _webPath == null)
                return base.ContainsFile(filePath);
            if (_directory != null)
            {
                var path = Path.Combine(_directory, filePath.Replace("/", @"\"));
                var r = File.Exists(path);
                return r;
            }
            return false;
        }

        public override byte[] LoadFileData(string filePath)
        {
            if (_directory == null && _webPath == null)
                return base.LoadFileData(filePath);
            if (_directory != null)
            {
                var path = Path.Combine(_directory, filePath);
                return File.ReadAllBytes(path);
            }
            return null;
        }
    }
}
