namespace OA.Bae
{
    public class BaeSettings
    {
        //[Header("Global")]
        //public string dataPath;
        //public string alternativeDataPath;
        public static bool useKinematicRigidbodies = true;
        //public bool playMusic = false;
        //public bool enableLog = false;
        //public Water.WaterMode waterQuality = Water.WaterMode.Simple;

        //[Header("Rendering")]
        public static MaterialType materialType = MaterialType.BumpedDiffuse;
        //public RenderingPath renderPath = RenderingPath.Forward;
        //public float cameraFarClip = 500.0f;

        //[Header("Lighting")]
        //public float ambientIntensity = 1.5f;
        //public bool renderSunShadows = false;
        public static bool renderLightShadows = false;
        public static bool renderExteriorCellLights = false;
        public static bool animateLights = false;
        //public bool dayNightCycle = false;
        public static bool generateNormalMap = true;
        public static float normalGeneratorIntensity = 0.75f;

        //[Header("Effects")]
        //public PostProcessingQuality postProcessingQuality = PostProcessingQuality.High;
        //public PostProcessLayer.Antialiasing antiAliasing = PostProcessLayer.Antialiasing.TemporalAntialiasing;
        //public bool waterBackSideTransparent = false;

        //[Header("VR")]
        //public bool followHeadDirection = false;
        //public bool directModePreview = true;
        //public bool roomScale = true;
        //public bool forceControllers = false;
        //public bool useXRVignette = false;
        //public float renderScale = 1.0f;

        //[Header("UI")]
        //public UIManager UIManager;
        //public Sprite UIBackgroundImg;
        //public Sprite UICheckmarkImg;
        //public Sprite UIDropdownArrowImg;
        //public Sprite UIInputFieldBackgroundImg;
        //public Sprite UIKnobImg;
        //public Sprite UIMaskImg;
        //public Sprite UISpriteImg;

        //[Header("Prefabs")]
        //public GameObject playerPrefab;
        //public GameObject waterPrefab;

        //[Header("Debug")]
        public static bool creaturesEnabled = false;
        public static bool npcsEnabled = false;
    }
}