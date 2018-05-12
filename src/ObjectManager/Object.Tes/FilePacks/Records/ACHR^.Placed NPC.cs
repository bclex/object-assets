using System;

namespace OA.Tes.FilePacks.Records
{
    public class ACHRRecord : Record
    {
        public override Field CreateField(string type) => throw new NotImplementedException();

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}