using ValveResourceFormat;

namespace Object.Vlv
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
