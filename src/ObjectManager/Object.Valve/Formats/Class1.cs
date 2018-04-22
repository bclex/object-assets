using ValveResourceFormat;

namespace OA.Valve.Formats
{
    public class Class1
    {
        public void Test()
        {
            using (var r = new Resource())
            {
                r.Read("");
            }
        }
    }
}
