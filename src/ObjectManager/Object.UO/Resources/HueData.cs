using OA.Core.Diagnostics;
using OA.Ultima.IO;
using System;
using System.IO;
using UnityEngine;

namespace OA.Ultima.Resources
{
    public class HueData
    {
        public const int HueCount = 4096;

        private static object graphicsDevice;
        private static Texture2D _hueTexture0, _hueTexture1;
        private const int _hueTextureWidth = 32; // Each hue is 32 colors
        private const int _hueTextureHeight = 2048;
        private static ushort[] _hues = new ushort[HueCount];

        public static void Initialize(object graphicsDevice)
        {
            HueData.graphicsDevice = graphicsDevice;
            //graphicsDevice.DeviceReset -= graphicsDevice_DeviceReset;
            //graphicsDevice.DeviceReset += graphicsDevice_DeviceReset;
            GetHueData();
        }

        public static ushort GetHue(int index, int offset)
        {
            index = (index + offset);
            if (index < 0) return 0xffff;
            else return _hues[index & 0x1fff];
        }

        static void graphicsDevice_DeviceReset(object sender, EventArgs e)
        {
            GetHueData();
        }

        static void GetHueData()
        {
            _hueTexture0 = new Texture2D(_hueTextureWidth, _hueTextureHeight);
            _hueTexture1 = new Texture2D(_hueTextureWidth, _hueTextureHeight);
            var hueData = getTextureData();
            //_hueTexture0.SetData(hueData, 0, _hueTextureWidth * _hueTextureHeight);
            //_hueTexture1.SetData(hueData, _hueTextureWidth * _hueTextureHeight, _hueTextureWidth * _hueTextureHeight);
        }

        static uint[] getTextureData()
        {
            var reader = new BinaryReader(FileManager.GetFile("hues.mul"));
            var currentHue = 0;
            var currentIndex = 0;
            var data = new uint[_hueTextureWidth * _hueTextureHeight * 2];
            Metrics.ReportDataRead((int)reader.BaseStream.Length);
            currentIndex += 32;
            while (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                reader.ReadInt32(); //Header
                for (var entry = 0; entry < 8; entry++)
                {
                    for (var i = 0; i < 32; i++)
                    {
                        const float multiplier = 0xff / 0x1f;
                        var color = reader.ReadUInt16();
                        if (i == 31)
                            _hues[currentHue] = (ushort)color;
                        data[currentIndex++] = 0xFF000000 + (
                            ((uint)(((color >> 10) & 0x1F) * multiplier)) |
                            ((uint)(((color >> 5) & 0x1F) * multiplier) << 8) |
                            ((uint)((color & 0x1F) * multiplier) << 16)
                            );
                    }
                    reader.ReadInt16(); //table start
                    reader.ReadInt16(); //table end
                    reader.ReadBytes(20); //name
                    currentHue++;
                }
            }
            reader.Close();
            var webSafeHuesBegin = _hueTextureHeight * 2 - 216;
            for (var b = 0; b < 6; b++)
                for (var g = 0; g < 6; g++)
                    for (var r = 0; r < 6; r++)
                        data[(webSafeHuesBegin + r + g * 6 + b * 36) * 32 + 31] = (uint)(
                            0xff000000 +
                            b * 0x00330000 +
                            g * 0x00003300 +
                            r * 0x00000033);
            return data;
        }

        public static Texture2D CreateHueSwatch(int width, int height, int[] hues)
        {
            var pixels = new uint[width * height];
            for (var i = 0; i < pixels.Length; i++)
            {
                var hue = hues[i];
                var pixel = new uint[1];
                //if (hue < _hueTextureHeight) HueTexture0.GetData(0, new RectInt(31, hue % _hueTextureHeight, 1, 1), pixel, 0, 1);
                //else HueTexture1.GetData(0, new RectInt(31, hue % _hueTextureHeight, 1, 1), pixel, 0, 1);
                pixels[i] = pixel[0];
            }
            var t = new Texture2D(width, height);
            //t.SetData(pixels);
            return t;
        }

        public static uint[] GetAllHues()
        {
            var hues = new uint[HueCount];
            var allHues = getTextureData();
            for (var i = 0; i < HueCount; i++)
                hues[i] = allHues[i * 32 + 31];
            return hues;
        }

        public static Texture2D HueTexture0
        {
            get { return _hueTexture0; }
        }

        public static Texture2D HueTexture1
        {
            get { return _hueTexture1; }
        }

        public static int GetWebSafeHue(Color inColor)
        {
            return GetWebSafeHue((int)inColor.r, (int)inColor.g, (int)inColor.b);
        }

        public static int GetWebSafeHue(int r, int g, int b)
        {
            var index = 0;
            for (var i = 0; i < 6; i++)
                if (r <= _kCutOffValuesForWebSafeColors[i])
                {
                    index += i * 1;
                    break;
                }
            for (var i = 0; i < 6; i++)
                if (g <= _kCutOffValuesForWebSafeColors[i])
                {
                    index += i * 6;
                    break;
                }
            for (var i = 0; i < 6; i++)
                if (b <= _kCutOffValuesForWebSafeColors[i])
                {
                    index += i * 36;
                    break;
                }
            return _kWebSafeHues[index];
        }
        static int[] _kCutOffValuesForWebSafeColors = { 0x19, 0x4C, 0x7F, 0xB2, 0xE5, 0xFF };
        static int[] _kWebSafeHues = {
            0000, 3881, 3882, 3883, 3884, 3885,
            3886, 3887, 3888, 3889, 3890, 3891,
            3892, 3893, 3894, 3895, 3896, 3897,
            3898, 3899, 3900, 3901, 3902, 3903,
            3904, 3905, 3906, 3907, 3908, 3909,
            3910, 3911, 3912, 3913, 3914, 3915,
            3916, 3917, 3918, 3919, 3920, 3921,
            3922, 3923, 3924, 3925, 3926, 3927,
            3928, 3929, 3930, 3931, 3932, 3933,
            3934, 3935, 3936, 3937, 3938, 3939,
            3940, 3941, 3942, 3943, 3944, 3945,
            3946, 3947, 3948, 3949, 3950, 3951,
            3952, 3953, 3954, 3955, 3956, 3957,
            3958, 3959, 3960, 3961, 3962, 3963,
            3964, 3965, 3966, 3967, 3968, 3969,
            3970, 3971, 3972, 3973, 3974, 3975,
            3976, 3977, 3978, 3979, 3980, 3981,
            3982, 3983, 3984, 3985, 3986, 3987,
            3988, 3989, 3990, 3991, 3992, 3993,
            3994, 3995, 3996, 3997, 3998, 3999,
            4000, 4001, 4002, 4003, 4004, 4005,
            4006, 4007, 4008, 4009, 4010, 4011,
            4012, 4013, 4014, 4015, 4016, 4017,
            4018, 4019, 4020, 4021, 4022, 4023,
            4024, 4025, 4026, 4027, 4028, 4029,
            4030, 4031, 4032, 4033, 4034, 4035,
            4036, 4037, 4038, 4039, 4040, 4041,
            4042, 4043, 4044, 4045, 4046, 4047,
            4048, 4049, 4050, 4051, 4052, 4053,
            4054, 4055, 4056, 4057, 4058, 4059,
            4060, 4061, 4062, 4063, 4064, 4065,
            4066, 4067, 4068, 4069, 4070, 4071,
            4072, 4073, 4074, 4075, 4076, 4077,
            4078, 4079, 4080, 4081, 4082, 4083,
            4084, 4085, 4086, 4087, 4088, 4089,
            4090, 4091, 4092, 4093, 4094, 4095 };
    }
}