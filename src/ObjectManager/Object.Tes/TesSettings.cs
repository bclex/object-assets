using OA.Configuration;
using OA.Tes.Configuration;

namespace OA.Tes
{
    public class TesSettings : BaseSettings
    {
        readonly TesRenderSettings _tes;

        TesSettings()
        {
            _tes = CreateOrOpenSection<TesRenderSettings>();
        }

        public static TesRenderSettings TesRender => null;// _instance._tes;
    }
}
