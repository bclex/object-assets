//using OA.Tes.FilePacks;
//using OA.Tes.Formats;
//using OA.Core;
//using System;
//using System.IO;
//using System.Threading.Tasks;
//using UnityEngine;
//using OA.Tes.FilePacks.Tes3;
//using OA.Tes.Core;
//using OA.Common.Formats;

//namespace OA.Tes
//{
//    public class MorrowindDataReader : IDisposable
//    {
//        public EsmFile ESMFile;
//        public BsaFile BSAFile;

//        public MorrowindDataReader(string dataPath, string name)
//        {
//            ESMFile = new EsmFile(dataPath + "/" + name + ".esm", GameId.Fallout4);
//            BSAFile = new BsaFile(dataPath + "/" + name + ".bsa");
//        }

//        void IDisposable.Dispose()
//        {
//            Close();
//        }

//        ~MorrowindDataReader()
//        {
//            Close();
//        }

//        public void Close()
//        {
//            BSAFile.Close();
//            ESMFile.Close();
//        }
//}