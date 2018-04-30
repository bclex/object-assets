namespace OA.Ultima.Formats
{
    public class StFile
    {
        public readonly string Name;

        public StFile(string name)
        {
            Name = name;
        }

        public StObject[] Blocks;
    }

    public class StObject
    {
    }
}