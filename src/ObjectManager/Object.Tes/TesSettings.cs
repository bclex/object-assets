using OA.Configuration;
using OA.Tes.Configuration;

namespace OA.Tes
{
    public class TesSettings : BaseSettings
    {
        static readonly TesSettings _instance = new TesSettings();
        readonly TesRenderSettings _tesRender;

        TesSettings()
        {
            _tesRender = CreateOrOpenSection<TesRenderSettings>();
        }

        public static TesRenderSettings TesRender => _instance._tesRender;
    }
}
