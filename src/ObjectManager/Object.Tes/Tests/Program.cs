using OA.Tes.FilePacks;
using OA.Tes.IO;
using System;

namespace OA.Tes
{
    public static class Program
    {
        public static void Main()
        {
            //@"C:\Program Files (x86)\Steam\steamapps\common\Fallout 4 VR\Data";
#if false
            TesSettings.TesRender.DataDirectory = @"C:\Program Files (x86)\Steam\steamapps\common\Morrowind\Data Files";
            TesSettings.TesRender.GameId = "Morrowind";
            var dataPath = FileManager.GetFilePath("Morrowind.esm", GameId.Morrowind);
            var dataPaths2 = FileManager.GetFilePaths("Morrowind.bsa", GameId.Morrowind);
#else
            var dataPath = FileManager.GetFilePath("Fallout4.esm", GameId.Fallout4VR);
            var dataPaths2 = FileManager.GetFilePaths("Fallout4.bsa", GameId.Fallout4VR);
#endif
            using (var esmFile = new EsmFile(dataPath, GameId.Fallout4VR))
            using (var bsaFile = new BsaMultiFile(dataPaths2))
            {
                Console.WriteLine("Loaded");
                var text = bsaFile.LoadTextureInfoAsync("tx_cursor").Result;
            }
        }
    }
}