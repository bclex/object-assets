using OA.Tes.FilePacks;
using OA.Tes.Formats;
using System.IO;
using UnityEngine;

namespace OA.Tes
{
    public class TesAssetPack : BsaFile, IAssetPack
    {
        TextureManager _textureManager;
        MaterialManager _materialManager;
        NifManager _nifManager;
        string _directory;
        string _webPath;

        public TesAssetPack(string filePath, string webPath)
            : base(filePath != null && File.Exists(filePath) ? filePath : null)
        {
            _directory = filePath != null && Directory.Exists(filePath) ? filePath : null;
            _webPath = webPath;
            _textureManager = new TextureManager(this);
            _materialManager = new MaterialManager(_textureManager);
            var markerLayer = 0; // LayerMask.NameToLayer("Marker");
            _nifManager = new NifManager(this, _materialManager, markerLayer);
        }

        public Texture2D LoadTexture(string path, bool flipVertically = false)
        {
            return _textureManager.LoadTexture(path, flipVertically);
        }

        public GameObject CreateObject(string path)
        {
            return _nifManager.InstantiateNIF(path);
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
