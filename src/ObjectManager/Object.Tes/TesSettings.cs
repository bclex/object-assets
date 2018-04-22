using OA.Configuration;
using OA.Tes.Configuration;

namespace OA.Tes
{
    public class TesSettings : BaseSettings
    {
        static readonly TesSettings _instance = new TesSettings();
        readonly TesGameSettings _tesGame;

        TesSettings()
        {
            _tesGame = CreateOrOpenSection<TesGameSettings>();
        }

        public static TesGameSettings TesGame => _instance._tesGame;
    }
}
