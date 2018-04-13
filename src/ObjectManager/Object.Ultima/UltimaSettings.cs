using OA.Configuration;
using OA.Ultima.Configuration;

namespace OA.Ultima
{
    public class UltimaSettings : BaseSettings
    {
        static readonly UltimaSettings _instance = new UltimaSettings();
        readonly UltimaOnlineSettings _ultimaOnline;

        UltimaSettings()
        {
            _ultimaOnline = CreateOrOpenSection<UltimaOnlineSettings>();
        }

        public static UltimaOnlineSettings UltimaOnline => _instance._ultimaOnline;
    }
}
