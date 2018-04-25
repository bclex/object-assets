namespace OA.Ultima.FilePacks.Records
{
    public class ITEMRecord : Record
    {
        public int ItemId;

        public string Name
        {
            get { return $"obj{ItemId}"; }
        }
    }
}