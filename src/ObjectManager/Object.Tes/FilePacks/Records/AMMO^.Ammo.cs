using System;

namespace OA.Tes.FilePacks.Records
{
    public class AMMORecord : Record
    {
        public override Field CreateField(string type) => throw new NotImplementedException();

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}