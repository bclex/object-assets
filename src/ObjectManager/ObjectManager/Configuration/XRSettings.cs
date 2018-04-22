namespace OA.Configuration
{
    public sealed class XRSettings : ASettingsSection
    {
        // VR
        bool _followHeadDirection = false;
        public bool FollowHeadDirection
        {
            get { return _followHeadDirection; }
            set { SetProperty(ref _followHeadDirection, value); }
        }
        bool _directModePreview = true;
        public bool DirectModePreview
        {
            get { return _directModePreview; }
            set { SetProperty(ref _directModePreview, value); }
        }
        bool _roomScale = true;
        public bool RoomScale
        {
            get { return _roomScale; }
            set { SetProperty(ref _roomScale, value); }
        }
        bool _forceControllers = false;
        public bool ForceControllers
        {
            get { return _forceControllers; }
            set { SetProperty(ref _forceControllers, value); }
        }
        bool _useXRVignette = false;
        public bool UseXRVignette
        {
            get { return _useXRVignette; }
            set { SetProperty(ref _useXRVignette, value); }
        }
        float _renderScale = 1.0f;
        public float RenderScale
        {
            get { return _renderScale; }
            set { SetProperty(ref _renderScale, value); }
        }
    }
}