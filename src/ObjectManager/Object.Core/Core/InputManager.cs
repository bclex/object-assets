using OA.XR;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace OA.Core
{
    public static class InputManager
    {
        private struct XRButtonMapping
        {
            public XRButton Button { get; set; }
            public bool LeftHand { get; set; }

            public XRButtonMapping(XRButton button, bool left)
            {
                Button = button;
                LeftHand = left;
            }
        }

        static Dictionary<string, XRButtonMapping> _XRMapping = null;

        public static float GetAxis(string axis)
        {
            var result = Input.GetAxis(axis);
            if (XRSettings.enabled)
            {
                var input = XRInput.Instance;
                if (axis == "Horizontal") result += input.GetAxis(XRAxis.ThumbstickX, true);
                else if (axis == "Vertical") result += input.GetAxis(XRAxis.ThumbstickY, true);
                else if (axis == "Mouse X") result += input.GetAxis(XRAxis.ThumbstickX, false);
                else if (axis == "Mouse Y") result += input.GetAxis(XRAxis.ThumbstickY, false);
                // Deadzone
                if (Mathf.Abs(result) < 0.15f)
                    result = 0.0f;
            }
            return result;
        }

        public static bool GetButton(string button)
        {
            var result = Input.GetButtonDown(button);
            if (XRSettings.enabled)
            {
                var input = XRInput.Instance;
                if (_XRMapping == null)
                    InitializeMapping();
                if (_XRMapping.ContainsKey(button))
                {
                    var mapping = _XRMapping[button];
                    result |= input.GetButton(mapping.Button, mapping.LeftHand);
                }
            }
            return result;
        }

        public static bool GetButtonUp(string button)
        {
            var result = Input.GetButtonUp(button);
            if (XRSettings.enabled)
            {
                var input = XRInput.Instance;
                if (_XRMapping == null)
                    InitializeMapping();
                if (_XRMapping.ContainsKey(button))
                {
                    var mapping = _XRMapping[button];
                    result |= input.GetButtonUp(mapping.Button, mapping.LeftHand);
                }
            }
            return result;
        }

        public static bool GetButtonDown(string button)
        {
            var result = Input.GetButtonDown(button);
            if (XRSettings.enabled)
            {
                var input = XRInput.Instance;
                if (_XRMapping == null)
                    InitializeMapping();
                if (_XRMapping.ContainsKey(button))
                {
                    var mapping = _XRMapping[button];
                    result |= input.GetButtonDown(mapping.Button, mapping.LeftHand);
                }
            }
            return result;
        }

        private static void InitializeMapping()
        {
            _XRMapping = new Dictionary<string, XRButtonMapping>()
            {
                { "Jump", new XRButtonMapping(XRButton.Thumbstick, true) },
                { "Light", new XRButtonMapping(XRButton.Thumbstick, false) },
                { "Run", new XRButtonMapping(XRButton.Grip, true) },
                { "Slow", new XRButtonMapping(XRButton.Grip, false) },
                { "Attack", new XRButtonMapping(XRButton.Trigger, false) },
                { "Recenter", new XRButtonMapping(XRButton.Menu, false) },
                { "Use", new XRButtonMapping(XRButton.Trigger, true) },
                { "Menu", new XRButtonMapping(XRButton.Menu, true) }
            };
        }
    }
}