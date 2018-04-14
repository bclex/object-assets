namespace OA.Configuration
{
    public sealed class XRSettings : ASettingsSection
    {
        bool _followHeadDirection = false;
        bool _directModePreview = true;
        bool _roomScale = true;
        bool _forceControllers = false;
        bool _useXRVignette = false;
        float _renderScale = 1.0f;

        public bool FollowHeadDirection
        {
            get { return _followHeadDirection; }
            set { SetProperty(ref _followHeadDirection, value); }
        }

        public bool DirectModePreview
        {
            get { return _directModePreview; }
            set { SetProperty(ref _directModePreview, value); }
        }

        public bool RoomScale
        {
            get { return _roomScale; }
            set { SetProperty(ref _roomScale, value); }
        }

        public bool ForceControllers
        {
            get { return _forceControllers; }
            set { SetProperty(ref _forceControllers, value); }
        }

        public bool UseXRVignette
        {
            get { return _useXRVignette; }
            set { SetProperty(ref _useXRVignette, value); }
        }

        public float RenderScale
        {
            get { return _renderScale; }
            set { SetProperty(ref _renderScale, value); }
        }
    }
}