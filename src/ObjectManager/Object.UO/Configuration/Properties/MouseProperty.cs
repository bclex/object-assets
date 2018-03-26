using OA.Ultima.Core.ComponentModel;
using UnityEngine.Experimental.UIElements;

namespace OA.Ultima.Configuration.Properties
{
    public class MouseProperty : NotifyPropertyChangedBase
    {
        MouseButton _interactionButton = MouseButton.LeftMouse;
        MouseButton _movementButton = MouseButton.RightMouse;
        bool _isEnabled = true;
        float _clickAndPickUpMS = 800f; // this is close to what the legacy client uses.
        float _doubleClickMS = 400f;

        public MouseProperty() { }

        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetProperty(ref _isEnabled, value); }
        }

        public MouseButton MovementButton
        {
            get { return _movementButton; }
            set { SetProperty(ref _movementButton, value); }
        }

        public MouseButton InteractionButton
        {
            get { return _interactionButton; }
            set { SetProperty(ref _interactionButton, value); }
        }

        public float ClickAndPickupMS
        {
            get { return _clickAndPickUpMS; }
            set { SetProperty(ref _clickAndPickUpMS, Clamp(value, 0, 2000)); }
        }

        public float DoubleClickMS
        {
            get { return _doubleClickMS; }
            set { SetProperty(ref _doubleClickMS, Clamp(value, 0, 2000)); }
        }

        
    }
}