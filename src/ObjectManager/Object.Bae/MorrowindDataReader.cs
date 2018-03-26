using OA.Bae.FilePacks;
using OA.Bae.Formats;
using OA.Core;
using OA.Formats;
using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace OA.Bae
{
    public class MorrowindDataReader : IDisposable
    {
        public EsmFile ESMFile;
        public BsaFile BSAFile;

        public MorrowindDataReader(string dataPath, string name)
        {
            ESMFile = new EsmFile(dataPath + "/" + name + ".esm", GameId.Fallout4);
            BSAFile = new BsaFile(dataPath + "/" + name + ".bsa");
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        ~MorrowindDataReader()
        {
            Close();
        }

        public void Close()
        {
            BSAFile.Close();
            ESMFile.Close();
        }

        public Task<Texture2DInfo> LoadTextureAsync(string texturePath)
        {
            var filePath = FindTexture(texturePath);
            if (filePath != null)
            {
                var fileData = BSAFile.LoadFileData(filePath);
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
            var fileData = BSAFile.LoadFileData(filePath);
            return Task.Run(() =>
            {
                var file = new NiFile(Path.GetFileNameWithoutExtension(filePath));
                file.Deserialize(new UnityBinaryReader(new MemoryStream(fileData)));
                return file;
            });
        }

        public LTEXRecord FindLTEXRecord(int index)
        {
            var records = ESMFile.GetRecordsOfType<LTEXRecord>();
            LTEXRecord ltex = null;
            for (int i = 0, l = records.Count; i < l; i++)
            {
                ltex = (LTEXRecord)records[i];
                if (ltex.INTV.value == index)
                    return ltex;
            }
            return null;
        }

        public LANDRecord FindLANDRecord(Vector2i cellIndices)
        {
            LANDRecord land;
            ESMFile.LANDRecordsByIndices.TryGetValue(cellIndices, out land);
            return land;
        }

        public CELLRecord FindExteriorCellRecord(Vector2i cellIndices)
        {
            CELLRecord cell;
            ESMFile.exteriorCELLRecordsByIndices.TryGetValue(cellIndices, out cell);
            return cell;
        }

        public CELLRecord FindInteriorCellRecord(string cellName)
        {
            var records = ESMFile.GetRecordsOfType<CELLRecord>();
            CELLRecord cell = null;
            for (int i = 0, l = records.Count; i < l; i++)
            {
                cell = (CELLRecord)records[i];
                if (cell.NAME.value == cellName)
                    return cell;
            }
            return null;
        }

        public CELLRecord FindInteriorCellRecord(Vector2i gridCoords)
        {
            var records = ESMFile.GetRecordsOfType<CELLRecord>();
            CELLRecord cell = null;
            for (int i = 0, l = records.Count; i < l; i++)
            {
                cell = (CELLRecord)records[i];
                if (cell.gridCoords.x == gridCoords.x && cell.gridCoords.y == gridCoords.y)
                    return cell;
            }
            return null;
        }

        /// <summary>
        /// Finds the actual path of a texture.
        /// </summary>
        private string FindTexture(string texturePath)
        {
            var textureName = Path.GetFileNameWithoutExtension(texturePath);
            var textureNameInTexturesDir = "textures/" + textureName;
            var filePath = textureNameInTexturesDir + ".dds";
            if (BSAFile.ContainsFile(filePath))
                return filePath;
            filePath = textureNameInTexturesDir + ".tga";
            if (BSAFile.ContainsFile(filePath))
                return filePath;
            var texturePathWithoutExtension = Path.GetDirectoryName(texturePath) + '/' + textureName;
            filePath = texturePathWithoutExtension + ".dds";
            if (BSAFile.ContainsFile(filePath))
                return filePath;
            filePath = texturePathWithoutExtension + ".tga";
            if (BSAFile.ContainsFile(filePath))
                return filePath;
            // Could not find the file.
            return null;
        }
    }
}