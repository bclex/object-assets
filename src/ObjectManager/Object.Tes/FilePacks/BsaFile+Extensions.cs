using OA.Core;
using OA.Formats;
using OA.Tes.Formats;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace OA.Tes.FilePacks
{
    partial class BsaFile
    {
        public Task<Texture2DInfo> LoadTextureAsync(string texturePath)
        {
            var filePath = FindTexture(texturePath);
            if (filePath != null)
            {
                var fileData = LoadFileData(filePath);
                return Task.Run(() =>
                {
                    var fileExtension = Path.GetExtension(filePath);
                    if (fileExtension?.ToLower() == ".dds") return DdsReader.LoadDDSTexture(new MemoryStream(fileData));
                    else throw new NotSupportedException($"Unsupported texture type: {fileExtension}");
                });
            }
            else
            {
                Utils.Warning("Could not find file \"" + texturePath + "\" in a BSA file.");
                return Task.FromResult<Texture2DInfo>(null);
            }
        }

        public Task<NiFile> LoadNifAsync(string filePath)
        {
            var fileData = LoadFileData(filePath);
            return Task.Run(() =>
            {
                var file = new NiFile(Path.GetFileNameWithoutExtension(filePath));
                file.Deserialize(new UnityBinaryReader(new MemoryStream(fileData)));
                return file;
            });
        }

        /// <summary>
        /// Finds the actual path of a texture.
        /// </summary>
        private string FindTexture(string texturePath)
        {
            var textureName = Path.GetFileNameWithoutExtension(texturePath);
            var textureNameInTexturesDir = "textures/" + textureName;
            var filePath = textureNameInTexturesDir + ".dds";
            if (ContainsFile(filePath))
                return filePath;
            //filePath = textureNameInTexturesDir + ".tga";
            //if (ContainsFile(filePath))
            //    return filePath;
            var texturePathWithoutExtension = Path.GetDirectoryName(texturePath) + '/' + textureName;
            filePath = texturePathWithoutExtension + ".dds";
            if (ContainsFile(filePath))
                return filePath;
            //filePath = texturePathWithoutExtension + ".tga";
            //if (ContainsFile(filePath))
            //    return filePath;
            // Could not find the file.
            return null;
        }
    }
}