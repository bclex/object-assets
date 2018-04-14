using System;
using UnityEngine;

namespace OA.Ultima.World.WorldViews
{
    public class IsometricLighting
    {
        int _lightLevelPersonal = 9;
        int _lightLevelOverall = 9;
        float _lightDirection = 4.12f;
        float _lightHeight = -0.75f;

        public IsometricLighting()
        {
            RecalculateLightningValues();
        }

        public int PersonalLightning
        {
            set { _lightLevelPersonal = value; RecalculateLightningValues(); }
            get { return _lightLevelPersonal; }
        }

        public int OverallLightning
        {
            set { _lightLevelOverall = value; RecalculateLightningValues(); }
            get { return _lightLevelOverall; }
        }

        public float LightDirection
        {
            set { _lightDirection = value; RecalculateLightningValues(); }
            get { return _lightDirection; }
        }

        public float LightHeight
        {
            set { _lightHeight = value; RecalculateLightningValues(); }
            get { return _lightHeight; }
        }

        private void RecalculateLightningValues()
        {
            var light = Math.Min(30 - OverallLightning + PersonalLightning, 30f);
            light = Math.Max(light, 0);
            IsometricLightLevel = light / 30; // bring it between 0-1

            // i'd use a fixed lightning direction for now - maybe enable this effect with a custom packet?
            _lightDirection = 1.2f;
            IsometricLightDirection = Vector3.Normalize(new Vector3((float)Math.Cos(_lightDirection), (float)Math.Sin(_lightDirection), 1f));
        }

        public float IsometricLightLevel { get; private set; }

        public Vector3 IsometricLightDirection { get; private set; }
    }
}
