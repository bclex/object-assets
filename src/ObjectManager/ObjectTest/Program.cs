using OA.Configuration;
using OA.Ultima;

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