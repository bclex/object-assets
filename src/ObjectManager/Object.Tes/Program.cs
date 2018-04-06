using System;

namespace OA.Tes
{
    public static class Program
    {
        public static void Main()
        {
            var dataPath =
                @"C:\Program Files (x86)\Steam\steamapps\common\Fallout 4 VR\Data";
            //@"C:\Program Files (x86)\Steam\steamapps\common\Morrowind\Data Files";
            using (var r = new MorrowindDataReader(dataPath, "Fallout4"))
            {
                Console.WriteLine("Loaded");
                //var engine = new MorrowindEngine(r);
                //engine.TestAllCells("results.txt");
                //var text = r.LoadTextureAsync("tx_cursor").Result;
            }
        }
    }
}