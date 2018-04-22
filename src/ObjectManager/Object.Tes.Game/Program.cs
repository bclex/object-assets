using OA.Tes.FilePacks;
using OA.Tes.IO;
using System;

namespace OA.Tes
{
    public static class Program
    {
        public static void Main()
        {
            //var dataPath = FileManager.GetFilePath("Fallout4.esm", GameId.Fallout4);
            //var dataPath2 = FileManager.GetFilePath("Fallout4.bsa", GameId.Fallout4);
            TesSettings.Game.DataDirectory = @"C:\Program Files (x86)\Steam\steamapps\common\Morrowind\Data Files";
            TesSettings.Game.GameId = "Morrowind";
            var dataPath = FileManager.GetFilePath("Morrowind.esm", GameId.Morrowind);
            var dataPath2 = FileManager.GetFilePath("Morrowind.bsa", GameId.Morrowind);
            //using (var esmFile = new TesDataFile(dataPath, GameId.Morrowind))
            //using (var bsaFile = new TesAssetFile(dataPath2))
            //{
            //    Console.WriteLine("Loaded");
            //    var engine = new TesEngine(esmFile, bsaFile, false);
            //    engine.TestAllCells("results.txt");
            //}
        }
    }
}