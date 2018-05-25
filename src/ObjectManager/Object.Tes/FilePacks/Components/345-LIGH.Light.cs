using UnityEngine;
using System.Collections;
using OA.Tes.FilePacks.Records;
using OA.Core;
using OA.Effects;

namespace OA.Tes.FilePacks.Components
{
    public class LIGHComponent : BASEComponent
    {
        [System.Serializable]
        public class LightData
        {
            public Light lightComponent;
            public enum LightFlags
            {
                Dynamic = 0x0001,
                CanCarry = 0x0002,
                Negative = 0x0004,
                Flicker = 0x0008,
                Fire = 0x0010,
                OffDefault = 0x0020,
                FlickerSlow = 0x0040,
                Pulse = 0x0080,
                PulseSlow = 0x0100
            }

            public int flags;
        }

        public LightData lightData = null;

        void Start()
        {
            var LIGH = (LIGHRecord)record;
            lightData = new LightData
            {
                lightComponent = gameObject.GetComponentInChildren<Light>(true)
            };
            if (LIGH.FULL != null)
                objData.name = LIGH.FULL.Value.Value;
            objData.interactionPrefix = "Take ";
            lightData.flags = LIGH.DATA.Flags;
            if (Utils.ContainsBitFlags(lightData.flags, (int)LightData.LightFlags.CanCarry))
            {
                gameObject.AddComponent<BoxCollider>().size *= 0.5f; //very weak-- adding a box collider to light objects so we can interact with them
                if (TesSettings.Game.KinematicRigidbodies)
                    gameObject.AddComponent<Rigidbody>().isKinematic = true;
            }
            StartCoroutine(ConfigureLightComponent());
        }

        public IEnumerator ConfigureLightComponent()
        {
            var time = 0f;
            //wait until we have found the light component. this will typically be the frame /after/ object creation as the light component is added after this component is created
            while (lightData.lightComponent == null && time < 5f)
            {
                lightData.lightComponent = gameObject.GetComponentInChildren<Light>(true);
                yield return new WaitForEndOfFrame();
                time += Time.deltaTime;
            }
            if (lightData.lightComponent != null) //if we have found the light component by the end of the loop
            {
                // Only disable the light based on flags if the light component hasn't already been disabled due to settings.
                if (lightData.lightComponent.enabled)
                    lightData.lightComponent.enabled = !Utils.ContainsBitFlags(lightData.flags, (int)LightData.LightFlags.OffDefault);
                var flicker = Utils.ContainsBitFlags(lightData.flags, (int)LightData.LightFlags.Flicker);
                var flickerSlow = Utils.ContainsBitFlags(lightData.flags, (int)LightData.LightFlags.FlickerSlow);
                var pulse = Utils.ContainsBitFlags(lightData.flags, (int)LightData.LightFlags.Pulse);
                var pulseSlow = Utils.ContainsBitFlags(lightData.flags, (int)LightData.LightFlags.PulseSlow);
                var fire = Utils.ContainsBitFlags(lightData.flags, (int)LightData.LightFlags.Fire);
                var animated = flicker || flickerSlow || pulse || pulseSlow || fire;
                if (animated && TesSettings.Game.AnimateLights)
                {
                    var lightAnim = lightData.lightComponent.gameObject.AddComponent<LightAnim>();
                    if (flicker) lightAnim.mode = LightAnimMode.Flicker;
                    if (flickerSlow) lightAnim.mode = LightAnimMode.FlickerSlow;
                    if (pulse) lightAnim.mode = LightAnimMode.Pulse;
                    if (pulseSlow) lightAnim.mode = LightAnimMode.PulseSlow;
                    if (fire) lightAnim.mode = LightAnimMode.Fire;
                }
            }
            else Debug.Log("Light Record Object Created Without Light Component. Search Timed Out.");
        }
    }
}