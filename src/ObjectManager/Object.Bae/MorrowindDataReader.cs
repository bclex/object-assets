using OA.Bae.Esm;
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
        public ESMFile MorrowindESMFile;
        public BSAFile MorrowindBSAFile;
        public ESMFile BloodmoonESMFile;
        public BSAFile BloodmoonBSAFile;
        public ESMFile TribunalESMFile;
        public BSAFile TribunalBSAFile;

        public MorrowindDataReader(string MorrowindFilePath)
        {
            MorrowindESMFile = new ESMFile(MorrowindFilePath + "/Morrowind.esm");
            MorrowindBSAFile = new BSAFile(MorrowindFilePath + "/Morrowind.bsa");
            /*BloodmoonESMFile = new ESMFile(MorrowindFilePath + "/Bloodmoon.esm");
			BloodmoonBSAFile = new BSAFile(MorrowindFilePath + "/Bloodmoon.bsa");
			TribunalESMFile = new ESMFile(MorrowindFilePath + "/Tribunal.esm");
			TribunalBSAFile = new BSAFile(MorrowindFilePath + "/Tribunal.bsa");*/
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
            /*TribunalBSAFile.Close();
			TribunalESMFile.Close();
			BloodmoonBSAFile.Close();
			BloodmoonESMFile.Close();*/
            MorrowindBSAFile.Close();
            MorrowindESMFile.Close();
        }

        public Task<Texture2DInfo> LoadTextureAsync(string texturePath)
        {
            var filePath = FindTexture(texturePath);
            if (filePath != null)
            {
                var fileData = MorrowindBSAFile.LoadFileData(filePath);
                return Task.Run(() =>
                {
                    var fileExtension = Path.GetExtension(filePath);
                    if (fileExtension?.ToLower() == ".dds") return DdsReader.LoadDDSTexture(new MemoryStream(fileData));
                    else throw new NotSupportedException($"Unsupported texture type: {fileExtension}");
                });
            }
            else
            {
                Debug.LogWarning("Could not find file \"" + texturePath + "\" in a BSA file.");
                return Task.FromResult<Texture2DInfo>(null);
            }
        }

        public Task<NiFile> LoadNifAsync(string filePath)
        {
            var fileData = MorrowindBSAFile.LoadFileData(filePath);
            return Task.Run(() =>
            {
                var file = new NiFile(Path.GetFileNameWithoutExtension(filePath));
                file.Deserialize(new UnityBinaryReader(new MemoryStream(fileData)));
                return file;
            });
        }

        public LTEXRecord FindLTEXRecord(int index)
        {
            var records = MorrowindESMFile.GetRecordsOfType<LTEXRecord>();
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
            MorrowindESMFile.LANDRecordsByIndices.TryGetValue(cellIndices, out land);
            return land;
        }

        public CELLRecord FindExteriorCellRecord(Vector2i cellIndices)
        {
            CELLRecord cell;
            MorrowindESMFile.exteriorCELLRecordsByIndices.TryGetValue(cellIndices, out cell);
            return cell;
        }

        public CELLRecord FindInteriorCellRecord(string cellName)
        {
            var records = MorrowindESMFile.GetRecordsOfType<CELLRecord>();
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
            var records = MorrowindESMFile.GetRecordsOfType<CELLRecord>();
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
            if (MorrowindBSAFile.ContainsFile(filePath))
                return filePath;
            filePath = textureNameInTexturesDir + ".tga";
            if (MorrowindBSAFile.ContainsFile(filePath))
                return filePath;
            var texturePathWithoutExtension = Path.GetDirectoryName(texturePath) + '/' + textureName;
            filePath = texturePathWithoutExtension + ".dds";
            if (MorrowindBSAFile.ContainsFile(filePath))
                return filePath;
            filePath = texturePathWithoutExtension + ".tga";
            if (MorrowindBSAFile.ContainsFile(filePath))
                return filePath;
            // Could not find the file.
            return null;
        }
    }
}