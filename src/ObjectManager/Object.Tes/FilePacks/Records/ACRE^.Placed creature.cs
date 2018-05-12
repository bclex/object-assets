using System;

namespace OA.Tes.FilePacks.Records
{
    public class ACRERecord : Record
    {
        public override Field CreateField(string type) => throw new NotImplementedException();

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}