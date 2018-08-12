using OA.Configuration;
//using Test = OA.Tes.ObjectTestDataPack;
using Test = OA.Tes.ObjectTestObject;

namespace OA
{
    public static class Program
    {
        public static void Main()
        {
            BaseSettings.Game.MaterialType = MaterialType.None;
            Test.Start();
        }
    }
}