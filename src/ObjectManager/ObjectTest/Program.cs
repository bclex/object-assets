using OA.Configuration;
using OA.Tes;

namespace OA
{
    public static class Program
    {
        public static void Main()
        {
            BaseSettings.Game.MaterialType = MaterialType.None;
            ObjectTestDataPack.Start();
        }
    }
}