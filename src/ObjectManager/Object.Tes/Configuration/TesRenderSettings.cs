using OA.Configuration;

namespace OA.Tes.Configuration
{
    public sealed class TesRenderSettings : ASettingsSection
    {
        string _dataDirectory;
        MaterialType _materialType;
        bool _kinematicRigidbodies;
        bool _creaturesEnabled;
        bool _npcsEnabled;
        bool _generateNormalMap;
        float _normalGeneratorIntensity;
        bool _renderLightShadows;
        bool _renderExteriorCellLights;

        public TesRenderSettings()
        {
            _kinematicRigidbodies = true;
            _generateNormalMap = true;
            _normalGeneratorIntensity = 0.75f;
            _materialType = MaterialType.BumpedDiffuse;
        }
        
        /// <summary>
        /// The directory where the Ultima Online resource files and executable are located.
        /// </summary>
        public string DataDirectory
        {
            get { return _dataDirectory; }
            set { SetProperty(ref _dataDirectory, value); }
        }

        public MaterialType MaterialType
        {
            get { return _materialType; }
            set { SetProperty(ref _materialType, value); }
        }

        public bool KinematicRigidbodies
        {
            get { return _kinematicRigidbodies; }
            set { SetProperty(ref _kinematicRigidbodies, value); }
        }

        public bool CreaturesEnabled
        {
            get { return _creaturesEnabled; }
            set { SetProperty(ref _creaturesEnabled, value); }
        }

        public bool NpcsEnabled
        {
            get { return _npcsEnabled; }
            set { SetProperty(ref _npcsEnabled, value); }
        }

        public bool GenerateNormalMap
        {
            get { return _generateNormalMap; }
            set { SetProperty(ref _generateNormalMap, value); }
        }

        public float NormalGeneratorIntensity
        {
            get { return _normalGeneratorIntensity; }
            set { SetProperty(ref _normalGeneratorIntensity, value); }
        }

        public bool RenderLightShadows
        {
            get { return _renderLightShadows; }
            set { SetProperty(ref _renderLightShadows, value); }
        }

        public bool RenderExteriorCellLights
        {
            get { return _renderExteriorCellLights; }
            set { SetProperty(ref _renderExteriorCellLights, value); }
        }
    }
}