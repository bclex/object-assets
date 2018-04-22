using OA.Configuration;
using UnityEngine;

namespace OA.Tes.Configuration
{
    public sealed class TesRenderSettings : ASettingsSection
    {
        // GLOBAL
        string _gameId;
        public string GameId
        {
            get { return _gameId; }
            set { SetProperty(ref _gameId, value); }
        }
        string _dataDirectory;
        public string DataDirectory
        {
            get { return _dataDirectory; }
            set { SetProperty(ref _dataDirectory, value); }
        }
        bool _kinematicRigidbodies = true;
        public bool KinematicRigidbodies
        {
            get { return _kinematicRigidbodies; }
            set { SetProperty(ref _kinematicRigidbodies, value); }
        }

        // RENDERING
        MaterialType _materialType = MaterialType.BumpedDiffuse;
        public MaterialType MaterialType
        {
            get { return _materialType; }
            set { SetProperty(ref _materialType, value); }
        }
        public RenderingPath _renderPath = RenderingPath.Forward;
        public RenderingPath RrenderPath
        {
            get { return _renderPath; }
            set { SetProperty(ref _renderPath, value); }
        }
        public float _cameraFarClip = 500.0f;
        public float CameraFarClip
        {
            get { return _cameraFarClip; }
            set { SetProperty(ref _cameraFarClip, value); }
        }

        // LIGHTING
        //float _ambientIntensity = 1.5f;
        //public float AmbientIntensity
        //{
        //    get { return _ambientIntensity; }
        //    set { SetProperty(ref _ambientIntensity, value); }
        //}
        //bool _renderSunShadows = false;
        //public bool RenderSunShadows
        //{
        //    get { return _renderSunShadows; }
        //    set { SetProperty(ref _renderSunShadows, value); }
        //}
        bool _renderLightShadows = false;
        public bool RenderLightShadows
        {
            get { return _renderLightShadows; }
            set { SetProperty(ref _renderLightShadows, value); }
        }
        bool _renderExteriorCellLights = false;
        public bool RenderExteriorCellLights
        {
            get { return _renderExteriorCellLights; }
            set { SetProperty(ref _renderExteriorCellLights, value); }
        }
        bool _animateLights = false;
        public bool AnimateLights
        {
            get { return _animateLights; }
            set { SetProperty(ref _animateLights, value); }
        }
        //bool _dayNightCycle = false;
        //public bool DayNightCycle
        //{
        //    get { return _dayNightCycle; }
        //    set { SetProperty(ref _dayNightCycle, value); }
        //}
        bool _generateNormalMap = true;
        public bool GenerateNormalMap
        {
            get { return _generateNormalMap; }
            set { SetProperty(ref _generateNormalMap, value); }
        }
        float _normalGeneratorIntensity = 0.75f;
        public float NormalGeneratorIntensity
        {
            get { return _normalGeneratorIntensity; }
            set { SetProperty(ref _normalGeneratorIntensity, value); }
        }

        // EFFECTS
        // UI
        // PREFABS

        // DEBUG
        bool _creaturesEnabled;
        public bool CreaturesEnabled
        {
            get { return _creaturesEnabled; }
            set { SetProperty(ref _creaturesEnabled, value); }
        }
        bool _npcsEnabled;
        public bool NpcsEnabled
        {
            get { return _npcsEnabled; }
            set { SetProperty(ref _npcsEnabled, value); }
        }
    }
}