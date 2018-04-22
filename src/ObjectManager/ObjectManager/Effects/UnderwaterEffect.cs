using UnityEngine;

namespace OA.Effects
{
    public class UnderwaterEffect : MonoBehaviour
    {
        bool _defaultFog;
        Color _defaultFogColor;
        float _defaultFogDensity;
        Material _defaultSkybox = null;
        bool _isUnderwater = false;
        Transform _transform = null;

        [SerializeField]
        float underwaterLevel = 0.0f;
        [SerializeField]
        Color ambientColor = Color.white;
        [SerializeField]
        Color fogColor = new Color(0, 0.4f, 0.7f, 0.6f);
        [SerializeField]
        float fogDensity = 0.04f;

        public float Level
        {
            get { return underwaterLevel; }
            set { underwaterLevel = value; }
        }

        void Start()
        {
            _defaultFog = RenderSettings.fog;
            _defaultFogColor = RenderSettings.fogColor;
            _defaultFogDensity = RenderSettings.fogDensity;
            _defaultSkybox = RenderSettings.skybox;
            _transform = GetComponent<Transform>();
        }

        void Update()
        {
            if (_transform.position.y < underwaterLevel && !_isUnderwater)
                SetEffectEnabled(true);
            else if (_transform.position.y > underwaterLevel && _isUnderwater)
                SetEffectEnabled(false);
        }

        public void SetEffectEnabled(bool enabled)
        {
            _isUnderwater = enabled;
            RenderSettings.fog = enabled;
            RenderSettings.fogColor = enabled ? fogColor : _defaultFogColor;
            RenderSettings.fogDensity = enabled ? fogDensity : _defaultFogDensity;
            RenderSettings.skybox = enabled ? null : _defaultSkybox;
        }
    }
}