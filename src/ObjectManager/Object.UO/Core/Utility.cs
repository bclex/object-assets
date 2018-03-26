using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace OA.Ultima.Core
{
    /// <summary>
    /// Utility class used to host common functions that do not fit inside a specific object.
    /// </summary>
    public class Utility
    {
        #region Buffer Formatting
        /// <summary>
        /// Formats the buffer to an output to a hex editor view of the data
        /// </summary>
        /// <param name="buffer">The buffer to be formatted</param>
        /// <returns>A System.String containing the formatted buffer</returns>
        public static string FormatBuffer(byte[] buffer)
        {
            if (buffer == null)
                return string.Empty;
            var formatted = FormatBuffer(buffer, buffer.Length);
            return formatted;
        }

        /// <summary>
        /// Formats the buffer to an output to a hex editor view of the data
        /// </summary>
        /// <param name="buffer">The buffer to be formatted</param>
        /// <param name="length">The length in bytes to format</param>
        /// <returns>A System.String containing the formatted buffer</returns>
        public static string FormatBuffer(byte[] buffer, int length)
        {
            if (buffer == null)
                return string.Empty;
            var builder = new StringBuilder();
            var ms = new MemoryStream(buffer);
            FormatBuffer(builder, ms, length);
            return builder.ToString();
        }

        /// <summary>
        /// Formats the stream buffer to an output to a hex editor view of the data
        /// </summary>
        /// <param name="input">The stream to be formatted</param>
        /// <param name="length">The length in bytes to format</param>
        /// <returns>A System.String containing the formatted buffer</returns>
        public static string FormatBuffer(Stream input, int length)
        {
            var builder = new StringBuilder();
            FormatBuffer(builder, input, length);
            return builder.ToString();
        }

        /// <summary>
        /// Formats the stream buffer to an output to a hex editor view of the data
        /// </summary>
        /// <param name="builder">The string builder to output the formatted buffer to</param>
        /// <param name="input">The stream to be formatted</param>
        /// <param name="length">The length in bytes to format</param>
        public static void FormatBuffer(StringBuilder builder, Stream input, int length)
        {
            length = (int)Math.Min(length, input.Length);
            builder.AppendLine("        0  1  2  3  4  5  6  7   8  9  A  B  C  D  E  F");
            builder.AppendLine("       -- -- -- -- -- -- -- --  -- -- -- -- -- -- -- --");
            var byteIndex = 0;
            var whole = length >> 4;
            var rem = length & 0xF;
            for (var i = 0; i < whole; ++i, byteIndex += 16)
            {
                var bytes = new StringBuilder(49);
                var chars = new StringBuilder(16);
                for (var j = 0; j < 16; ++j)
                {
                    var c = input.ReadByte();
                    bytes.Append(c.ToString("X2"));
                    if (j != 7) bytes.Append(' ');
                    else bytes.Append("  ");
                    if (c >= 0x20 && c < 0x80) chars.Append((char)c);
                    else chars.Append('.');
                }
                builder.Append(byteIndex.ToString("X4"));
                builder.Append("   ");
                builder.Append(bytes);
                builder.Append("  ");
                builder.AppendLine(chars.ToString());
            }
            if (rem != 0)
            {
                var bytes = new StringBuilder(49);
                var chars = new StringBuilder(rem);
                for (var j = 0; j < 16; ++j)
                    if (j < rem)
                    {
                        var c = input.ReadByte();
                        bytes.Append(c.ToString("X2"));
                        if (j != 7) bytes.Append(' ');
                        else bytes.Append("  ");
                        if (c >= 0x20 && c < 0x80) chars.Append((char)c);
                        else chars.Append('.');
                    }
                    else bytes.Append("   ");
                builder.Append(byteIndex.ToString("X4"));
                builder.Append("   ");
                builder.Append(bytes);
                builder.Append("  ");
                builder.AppendLine(chars.ToString());
            }
        }
        #endregion

        #region Encoding
        static Encoding _utf8;
        static Encoding _utf8WithEncoding;

        public static Encoding UTF8
        {
            get
            {
                if (_utf8 == null)
                    _utf8 = new UTF8Encoding(false, false);
                return _utf8;
            }
        }

        public static Encoding UTF8WithEncoding
        {
            get
            {
                if (_utf8WithEncoding == null)
                    _utf8WithEncoding = new UTF8Encoding(true, false);
                return _utf8WithEncoding;
            }
        }
        #endregion

        // Color utilities, made freely available on http://snowxna.wordpress.com/
        #region ColorUtility
        //static readonly char[] HexDigits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

        //public static string ColorToHexString(Color color)
        //{
        //    var bytes = new byte[4];
        //    bytes[0] = color.A;
        //    bytes[1] = color.R;
        //    bytes[2] = color.G;
        //    bytes[3] = color.B;
        //    char[] chars = new char[8];
        //    for (int i = 0; i < 4; i++)
        //    {
        //        int b = bytes[i];
        //        chars[i * 2] = HexDigits[b >> 4];
        //        chars[i * 2 + 1] = HexDigits[b & 0xF];
        //    }
        //    return new string(chars);
        //}

        //static byte HexDigitToByte(char c)
        //{
        //    switch (c)
        //    {
        //        case '0': return (byte)0;
        //        case '1': return (byte)1;
        //        case '2': return (byte)2;
        //        case '3': return (byte)3;
        //        case '4': return (byte)4;
        //        case '5': return (byte)5;
        //        case '6': return (byte)6;
        //        case '7': return (byte)7;
        //        case '8': return (byte)8;
        //        case '9': return (byte)9;
        //        case 'A': case 'a': return (byte)10;
        //        case 'B': case 'b': return (byte)11;
        //        case 'C': case 'c': return (byte)12;
        //        case 'D': case 'd': return (byte)13;
        //        case 'E': case 'e': return (byte)14;
        //        case 'F': case 'f': return (byte)15;
        //    }
        //    return (byte)0;
        //}

        //public static uint UintFromColor(Color color)
        //{
        //    return (uint)(((uint)colora << 24) | ((uint)color.b << 16) | ((uint)color.g << 8) | (color.r));
        //}

        //public static Color ColorFromHexString(string hex)
        //{
        //    switch (hex.Length)
        //    {
        //        case 8:
        //            {
        //                int a = (HexDigitToByte(hex[0]) << 4) + HexDigitToByte(hex[1]);
        //                int r = (HexDigitToByte(hex[2]) << 4) + HexDigitToByte(hex[3]);
        //                int g = (HexDigitToByte(hex[4]) << 4) + HexDigitToByte(hex[5]);
        //                int b = (HexDigitToByte(hex[6]) << 4) + HexDigitToByte(hex[7]);
        //                return new Color((byte)r, (byte)g, (byte)b, (byte)a);
        //            }
        //        case 6:
        //            {
        //                int r = (HexDigitToByte(hex[0]) << 4) + HexDigitToByte(hex[1]);
        //                int g = (HexDigitToByte(hex[2]) << 4) + HexDigitToByte(hex[3]);
        //                int b = (HexDigitToByte(hex[4]) << 4) + HexDigitToByte(hex[5]);
        //                return new Color((byte)r, (byte)g, (byte)b);
        //            }
        //        case 3:
        //            {
        //                int r = (HexDigitToByte(hex[0]) << 4) + HexDigitToByte(hex[0]);
        //                int g = (HexDigitToByte(hex[1]) << 4) + HexDigitToByte(hex[1]);
        //                int b = (HexDigitToByte(hex[2]) << 4) + HexDigitToByte(hex[2]);
        //                return new Color((byte)r, (byte)g, (byte)b);
        //            }
        //        default:
        //            return Color.black;
        //    }
        //}

        //private static readonly Dictionary<string, Color> ColorTable = new Dictionary<string, Color>()
        //{
        //    {"white", Color.white},
        //    {"red", Color.red},
        //    {"blue", Color.blue},
        //    {"green", Color.green},
        //    //{"orange", Color.Orange},
        //    {"yellow", Color.yellow}
        //    //add more colors here
        //};

        //public static Color? ColorFromString(string color)
        //{
        //    Color output;
        //    if (!ColorTable.TryGetValue(color.ToLower(), out output)) return null;
        //    return output;
        //}
        #endregion

        // Version string.
        static string _versionString;
        public static string VersionString
        {
            get
            {
                if (_versionString == null)
                {
                    var v = Assembly.GetExecutingAssembly().GetName().Version;
                    var d = new DateTime(v.Build * TimeSpan.TicksPerDay).AddYears(1999).AddDays(-1);
                    _versionString = string.Format("ABC {0}.{1} ({2})", v.Major, v.Minor, string.Format("{0:MMMM d, yyyy}", d));
                }
                return _versionString;
            }
        }

        #region To[Something]
        public static bool ToBoolean(string value)
        {
            bool b;
            bool.TryParse(value, out b);
            return b;
        }

        public static double ToDouble(string value)
        {
            double d;
            double.TryParse(value, out d);
            return d;
        }

        public static TimeSpan ToTimeSpan(string value)
        {
            TimeSpan t;
            TimeSpan.TryParse(value, out t);
            return t;
        }

        public static int ToInt32(string value)
        {
            int i;
            if (value.StartsWith("0x")) int.TryParse(value.Substring(2), NumberStyles.HexNumber, null, out i);
            else int.TryParse(value, out i);
            return i;
        }
        #endregion

        public static int IPAddress
        {
            get
            {
                byte[] iIPAdress = { 127, 0, 0, 1 };
                var iAddress = BitConverter.ToInt32(iIPAdress, 0);
                return iAddress;
            }
        }

        public static long GetLongAddressValue(IPAddress address)
        {
#pragma warning disable 618
            return address.Address;
#pragma warning restore 618
        }

        static System.Random _random;
        public static int RandomValue(int low, int high)
        {
            if (_random == null)
                _random = new System.Random();
            var rnd = _random.Next(low, high + 1);
            return rnd;
        }

        public static bool IsPointThisDistanceAway(Vector2Int initial, Vector2Int final, int distance)
        {
            // fast distance. Not super accurate, but fast. Fast is good.
            if (Math.Abs(final.x - initial.x) + Math.Abs(final.y - initial.y) > distance) return true;
            else return false;
        }

        public static Vector3 GetHueVector(int hue)
        {
            return GetHueVector(hue, false, false, false);
        }

        public static Vector3 GetHueVector(int hue, bool partial, bool transparent, bool noLighting)
        {
            if ((hue & 0x4000) != 0)
                transparent = true;
            if ((hue & 0x8000) != 0)
                partial = true;
            if (hue == 0)
                return new Vector3(0, 0, transparent ? 0.5f : 0);
            return new Vector3(hue & 0x0FFF, (noLighting ? 4 : 0) + (partial ? 2 : 1), transparent ? 0.5f : 0);
        }

        /// <summary>
        /// Reads a color from a ushort hue; format is bgr.
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string GetColorFromUshort(ushort color)
        {
            const float multiplier = 0xFF / 0x1F;
            var uintColor = (uint)(
                ((uint)(((color >> 10) & 0x1F) * multiplier) << 16) |
                ((uint)(((color >> 5) & 0x1F) * multiplier) << 8) |
                ((uint)((color & 0x1F) * multiplier))
                );
            return string.Format("{0:X6}", uintColor);
        }

        public static string GetColorFromInt(int color)
        {
            return string.Format("{0:X6}", color);
        }

        public static int DistanceBetweenTwoPoints(Vector2Int p1, Vector2Int p2)
        {
            return Convert.ToInt32(Math.Sqrt((p1.x - p2.x) * (p1.x - p2.x) + (p1.y - p2.y) * (p1.y - p2.y)));
        }

        //public static void SaveTexture(Texture2D texture, string path)
        //{
        //    if (texture != null)
        //        texture.SaveAsPng(new FileStream(path, FileMode.Create), texture.Width, texture.Height);
        //}

        public static string CapitalizeFirstCharacter(string str)
        {
            if (str == null || str == string.Empty)
                return string.Empty;
            if (str.Length == 1)
                return char.ToUpper(str[0]).ToString();
            return char.ToUpper(str[0]) + str.Substring(1);
        }

        public static string CapitalizeAllWords(string str)
        {
            if (str == null || str == string.Empty)
                return string.Empty;
            if (str.Length == 1)
                return char.ToUpper(str[0]).ToString();
            var b = new StringBuilder();
            var capitalizeNext = true;
            for (int i = 0; i < str.Length; i++)
            {
                if (capitalizeNext) b.Append(char.ToUpper(str[i]));
                else b.Append(str[i]);
                capitalizeNext = (" .,;!".Contains(str[i]));
            }
            return b.ToString();
        }


        public static string[] CreateStringLinesFromList<T>(List<T> list)
        {
            var arrayList = new string[list.Count];
            for (var i = 0; i < arrayList.Length; i++)
                arrayList[i] = list[i].ToString();
            return arrayList;
        }
    }
}
