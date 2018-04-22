using System;

namespace OA.HttpCache
{
    public static class HttpCacheManager
    {
        public static object Get(Uri uri)
        {
            return null;
        }
    }
}


//public static class ResourcesEx
//{
//    public static Object LoadFromUri(string address) { return LoadFromUri(new Uri(address)); }
//    public static Object LoadFromUri(Uri address)
//    {
//        using (var client = new WebClient())
//        {
//            using (var stream = client.OpenRead(address))
//            using (var reader = new StreamReader(stream))
//            {
//                var result = reader.ReadToEnd();
//                Console.WriteLine(result);
//            }
//        }
//        return GameObject.Find("Cube00");
//    }
//}