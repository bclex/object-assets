using OA.Configuration;
using OA.Tes.Configuration;

namespace OA.Tes
{
    public class TesSettings : BaseSettings
    {
        static readonly TesSettings _instance = new TesSettings();
        readonly TesRenderSettings _tesRender;
        readonly XRSettings _xr;

        TesSettings()
        {
            _tesRender = CreateOrOpenSection<TesRenderSettings>();
            _xr = CreateOrOpenSection<XRSettings>();
        }

        public static TesRenderSettings TesRender => _instance._tesRender;
        public static XRSettings XR => _instance._xr;
    }
}
