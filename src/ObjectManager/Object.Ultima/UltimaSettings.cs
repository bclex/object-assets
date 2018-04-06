using OA.Configuration;
using OA.Ultima.Configuration;

namespace OA.Ultima
{
    public class UltimaSettings : BaseSettings
    {
        readonly UltimaOnlineSettings _ultimaOnline;

        UltimaSettings()
        {
            _ultimaOnline = CreateOrOpenSection<UltimaOnlineSettings>();
        }

        public static UltimaOnlineSettings UltimaOnline => null;// _instance._ultimaOnline;
    }
}
