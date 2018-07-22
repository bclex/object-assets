using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class WATRRecord : Record
    {
        public class DATAField
        {
            public float WindVelocity;
            public float WindDirection;
            public float WaveAmplitude;
            public float WaveFrequency;
            public float SunPower;
            public float ReflectivityAmount;
            public float FresnelAmount;
            public float ScrollXSpeed;
            public float ScrollYSpeed;
            public float FogDistance_NearPlane;
            public float FogDistance_FarPlane;
            public ColorRef4 ShallowColor;
            public ColorRef4 DeepColor;
            public ColorRef4 ReflectionColor;
            public byte TextureBlend;
            public float RainSimulator_Force;
            public float RainSimulator_Velocity;
            public float RainSimulator_Falloff;
            public float RainSimulator_Dampner;
            public float RainSimulator_StartingSize;
            public float DisplacementSimulator_Force;
            public float DisplacementSimulator_Velocity;
            public float DisplacementSimulator_Falloff;
            public float DisplacementSimulator_Dampner;
            public float DisplacementSimulator_StartingSize;
            public ushort Damage;

            public DATAField(UnityBinaryReader r, int dataSize)
            {
                if (dataSize != 102 && dataSize != 86 && dataSize != 62 && dataSize != 42 && dataSize != 2)
                    WindVelocity = 1;
                if (dataSize == 2)
                {
                    Damage = r.ReadLEUInt16();
                    return;
                }
                WindVelocity = r.ReadLESingle();
                WindDirection = r.ReadLESingle();
                WaveAmplitude = r.ReadLESingle();
                WaveFrequency = r.ReadLESingle();
                SunPower = r.ReadLESingle();
                ReflectivityAmount = r.ReadLESingle();
                FresnelAmount = r.ReadLESingle();
                ScrollXSpeed = r.ReadLESingle();
                ScrollYSpeed = r.ReadLESingle();
                FogDistance_NearPlane = r.ReadLESingle();
                if (dataSize == 42)
                {
                    Damage = r.ReadLEUInt16();
                    return;
                }
                FogDistance_FarPlane = r.ReadLESingle();
                ShallowColor = r.ReadT<ColorRef4>(dataSize);
                DeepColor = r.ReadT<ColorRef4>(dataSize);
                ReflectionColor = r.ReadT<ColorRef4>(dataSize);
                TextureBlend = r.ReadByte();
                r.SkipBytes(3); // Unused
                if (dataSize == 62)
                {
                    Damage = r.ReadLEUInt16();
                    return;
                }
                RainSimulator_Force = r.ReadLESingle();
                RainSimulator_Velocity = r.ReadLESingle();
                RainSimulator_Falloff = r.ReadLESingle();
                RainSimulator_Dampner = r.ReadLESingle();
                RainSimulator_StartingSize = r.ReadLESingle();
                DisplacementSimulator_Force = r.ReadLESingle();
                if (dataSize == 86)
                {
                    //DisplacementSimulator_Velocity = DisplacementSimulator_Falloff = DisplacementSimulator_Dampner = DisplacementSimulator_StartingSize = 0F;
                    Damage = r.ReadLEUInt16();
                    return;
                }
                DisplacementSimulator_Velocity = r.ReadLESingle();
                DisplacementSimulator_Falloff = r.ReadLESingle();
                DisplacementSimulator_Dampner = r.ReadLESingle();
                DisplacementSimulator_StartingSize = r.ReadLESingle();
                Damage = r.ReadLEUInt16();
            }
        }

        public struct GNAMField
        {
            public FormId<WATRRecord> Daytime;
            public FormId<WATRRecord> Nighttime;
            public FormId<WATRRecord> Underwater;

            public GNAMField(UnityBinaryReader r, int dataSize)
            {
                Daytime = new FormId<WATRRecord>(r.ReadLEUInt32());
                Nighttime = new FormId<WATRRecord>(r.ReadLEUInt32());
                Underwater = new FormId<WATRRecord>(r.ReadLEUInt32());
            }
        }

        public override string ToString() => $"WATR: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public STRVField TNAM; // Texture
        public BYTEField ANAM; // Opacity
        public BYTEField FNAM; // Flags
        public STRVField MNAM; // Material ID
        public FMIDField<SOUNRecord> SNAM; // Sound
        public DATAField DATA; // DATA
        public GNAMField GNAM; // GNAM

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = r.ReadSTRV(dataSize); return true;
                case "TNAM": TNAM = r.ReadSTRV(dataSize); return true;
                case "ANAM": ANAM = r.ReadT<BYTEField>(dataSize); return true;
                case "FNAM": FNAM = r.ReadT<BYTEField>(dataSize); return true;
                case "MNAM": MNAM = r.ReadSTRV(dataSize); return true;
                case "SNAM": SNAM = new FMIDField<SOUNRecord>(r, dataSize); return true;
                case "DATA": DATA = new DATAField(r, dataSize); return true;
                case "GNAM": GNAM = new GNAMField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}