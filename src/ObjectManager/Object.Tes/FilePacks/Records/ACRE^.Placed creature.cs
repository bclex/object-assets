using System;

namespace OA.Tes.FilePacks.Records
{
    public class ACRERecord : Record
    {
        public override SubRecord CreateUninitializedSubRecord(string subRecordName) => throw new NotImplementedException();

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, GameId gameId) => throw new NotImplementedException();
    }
}