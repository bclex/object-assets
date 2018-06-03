using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class EFSHRecord : Record
    {
        public class DATAField
        {
            public byte Flags;
            public uint MembraneShader_SourceBlendMode;
            public uint MembraneShader_BlendOperation;
            public uint MembraneShader_ZTestFunction;
            public ColorRef FillTextureEffect_Color;
            public float FillTextureEffect_AlphaFadeInTime;
            public float FillTextureEffect_FullAlphaTime;
            public float FillTextureEffect_AlphaFadeOutTime;
            public float FillTextureEffect_PresistentAlphaRatio;
            public float FillTextureEffect_AlphaPulseAmplitude;
            public float FillTextureEffect_AlphaPulseFrequency;
            public float FillTextureEffect_TextureAnimationSpeed_U;
            public float FillTextureEffect_TextureAnimationSpeed_V;
            public float EdgeEffect_FallOff;
            public ColorRef EdgeEffect_Color;
            public float EdgeEffect_AlphaFadeInTime;
            public float EdgeEffect_FullAlphaTime;
            public float EdgeEffect_AlphaFadeOutTime;
            public float EdgeEffect_PresistentAlphaRatio;
            public float EdgeEffect_AlphaPulseAmplitude;
            public float EdgeEffect_AlphaPulseFrequency;
            public float FillTextureEffect_FullAlphaRatio;
            public float EdgeEffect_FullAlphaRatio;
            public uint MembraneShader_DestBlendMode;
            public uint ParticleShader_SourceBlendMode;
            public uint ParticleShader_BlendOperation;
            public uint ParticleShader_ZTestFunction;
            public uint ParticleShader_DestBlendMode;
            public float ParticleShader_ParticleBirthRampUpTime;
            public float ParticleShader_FullParticleBirthTime;
            public float ParticleShader_ParticleBirthRampDownTime;
            public float ParticleShader_FullParticleBirthRatio;
            public float ParticleShader_PersistantParticleBirthRatio;
            public float ParticleShader_ParticleLifetime;
            public float ParticleShader_ParticleLifetime_Delta;
            public float ParticleShader_InitialSpeedAlongNormal;
            public float ParticleShader_AccelerationAlongNormal;
            public float ParticleShader_InitialVelocity1;
            public float ParticleShader_InitialVelocity2;
            public float ParticleShader_InitialVelocity3;
            public float ParticleShader_Acceleration1;
            public float ParticleShader_Acceleration2;
            public float ParticleShader_Acceleration3;
            public float ParticleShader_ScaleKey1;
            public float ParticleShader_ScaleKey2;
            public float ParticleShader_ScaleKey1Time;
            public float ParticleShader_ScaleKey2Time;
            public ColorRef ColorKey1_Color;
            public ColorRef ColorKey2_Color;
            public ColorRef ColorKey3_Color;
            public float ColorKey1_ColorAlpha;
            public float ColorKey2_ColorAlpha;
            public float ColorKey3_ColorAlpha;
            public float ColorKey1_ColorKeyTime;
            public float ColorKey2_ColorKeyTime;
            public float ColorKey3_ColorKeyTime;

            public DATAField(UnityBinaryReader r, int dataSize)
            {
                if (dataSize != 224 && dataSize != 96)
                    Flags = 0;
                Flags = r.ReadByte();
                r.SkipBytes(3); // Unused
                MembraneShader_SourceBlendMode = r.ReadLEUInt32();
                MembraneShader_BlendOperation = r.ReadLEUInt32();
                MembraneShader_ZTestFunction = r.ReadLEUInt32();
                FillTextureEffect_Color = new ColorRef(r);
                FillTextureEffect_AlphaFadeInTime = r.ReadLESingle();
                FillTextureEffect_FullAlphaTime = r.ReadLESingle();
                FillTextureEffect_AlphaFadeOutTime = r.ReadLESingle();
                FillTextureEffect_PresistentAlphaRatio = r.ReadLESingle();
                FillTextureEffect_AlphaPulseAmplitude = r.ReadLESingle();
                FillTextureEffect_AlphaPulseFrequency = r.ReadLESingle();
                FillTextureEffect_TextureAnimationSpeed_U = r.ReadLESingle();
                FillTextureEffect_TextureAnimationSpeed_V = r.ReadLESingle();
                EdgeEffect_FallOff = r.ReadLESingle();
                EdgeEffect_Color = new ColorRef(r);
                EdgeEffect_AlphaFadeInTime = r.ReadLESingle();
                EdgeEffect_FullAlphaTime = r.ReadLESingle();
                EdgeEffect_AlphaFadeOutTime = r.ReadLESingle();
                EdgeEffect_PresistentAlphaRatio = r.ReadLESingle();
                EdgeEffect_AlphaPulseAmplitude = r.ReadLESingle();
                EdgeEffect_AlphaPulseFrequency = r.ReadLESingle();
                FillTextureEffect_FullAlphaRatio = r.ReadLESingle();
                EdgeEffect_FullAlphaRatio = r.ReadLESingle();
                MembraneShader_DestBlendMode = r.ReadLEUInt32();
                if (dataSize == 96)
                    return;
                ParticleShader_SourceBlendMode = r.ReadLEUInt32();
                ParticleShader_BlendOperation = r.ReadLEUInt32();
                ParticleShader_ZTestFunction = r.ReadLEUInt32();
                ParticleShader_DestBlendMode = r.ReadLEUInt32();
                ParticleShader_ParticleBirthRampUpTime = r.ReadLESingle();
                ParticleShader_FullParticleBirthTime = r.ReadLESingle();
                ParticleShader_ParticleBirthRampDownTime = r.ReadLESingle();
                ParticleShader_FullParticleBirthRatio = r.ReadLESingle();
                ParticleShader_PersistantParticleBirthRatio = r.ReadLESingle();
                ParticleShader_ParticleLifetime = r.ReadLESingle();
                ParticleShader_ParticleLifetime_Delta = r.ReadLESingle();
                ParticleShader_InitialSpeedAlongNormal = r.ReadLESingle();
                ParticleShader_AccelerationAlongNormal = r.ReadLESingle();
                ParticleShader_InitialVelocity1 = r.ReadLESingle();
                ParticleShader_InitialVelocity2 = r.ReadLESingle();
                ParticleShader_InitialVelocity3 = r.ReadLESingle();
                ParticleShader_Acceleration1 = r.ReadLESingle();
                ParticleShader_Acceleration2 = r.ReadLESingle();
                ParticleShader_Acceleration3 = r.ReadLESingle();
                ParticleShader_ScaleKey1 = r.ReadLESingle();
                ParticleShader_ScaleKey2 = r.ReadLESingle();
                ParticleShader_ScaleKey1Time = r.ReadLESingle();
                ParticleShader_ScaleKey2Time = r.ReadLESingle();
                ColorKey1_Color = new ColorRef(r);
                ColorKey2_Color = new ColorRef(r);
                ColorKey3_Color = new ColorRef(r);
                ColorKey1_ColorAlpha = r.ReadLESingle();
                ColorKey2_ColorAlpha = r.ReadLESingle();
                ColorKey3_ColorAlpha = r.ReadLESingle();
                ColorKey1_ColorKeyTime = r.ReadLESingle();
                ColorKey2_ColorKeyTime = r.ReadLESingle();
                ColorKey3_ColorKeyTime = r.ReadLESingle();
            }
        }

        public override string ToString() => $"EFSH: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public FILEField ICON; // Fill Texture
        public FILEField ICO2; // Particle Shader Texture
        public DATAField DATA; // Data

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "ICON": ICON = new FILEField(r, dataSize); return true;
                case "ICO2": ICO2 = new FILEField(r, dataSize); return true;
                case "DATA": DATA = new DATAField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}