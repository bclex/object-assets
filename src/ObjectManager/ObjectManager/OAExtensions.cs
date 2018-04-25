using System;
using System.Threading.Tasks;

namespace OA
{
    public static class OAExtensions
    {
        public static Task<IAssetPack> GetAssetPack(this IAssetManager source, string uri) { return source.GetAssetPack(new Uri(uri)); }
        public static Task<IDataPack> GetDataPack(this IAssetManager source, string uri) { return source.GetDataPack(new Uri(uri)); }
    }
}