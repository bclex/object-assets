using OA.Ultima.IO;
using System.Diagnostics;
using System.IO;

namespace OA.Ultima.Data
{
    public static class ClientVersion
    {
        // NOTE FROM ZaneDubya: DO NOT change DefaultVersion from 6.0.6.2.
        // We are focusing our efforts on getting a specific version of the client working.
        // Once we have this version working, we will attempt to support additional versions.
        // We will not support any issues you experience after changing this value.
        public static readonly byte[] DefaultVersion = { 6, 0, 6, 2 };

        static readonly byte[] _unknownClientVersion = { 0, 0, 0, 0 };
        static readonly byte[] _extendedAddItemToContainer = { 6, 0, 1, 7 };
        static readonly byte[] _extendedFeaturesVersion = { 6, 0, 14, 2 };
        static readonly byte[] _convertedToUOPVersion = { 7, 0, 24, 0 };
        static byte[] _clientExeVersion;

        public static byte[] ClientExe
        {
            get
            {
                if (_clientExeVersion == null)
                {
                    var path = FileManager.GetPath("client.exe");
                    if (File.Exists(path))
                    {
                        var exe = FileVersionInfo.GetVersionInfo(path);
                        _clientExeVersion = new byte[] {
                            (byte)exe.FileMajorPart, (byte)exe.FileMinorPart,
                            (byte)exe.FileBuildPart, (byte)exe.FilePrivatePart };
                    }
                    else _clientExeVersion = _unknownClientVersion;
                }
                return _clientExeVersion;
            }
        }

        public static bool InstallationIsUopFormat => GreaterThanOrEqualTo(ClientExe, _convertedToUOPVersion);

        public static bool HasExtendedFeatures(byte[] version) => GreaterThanOrEqualTo(version, _extendedFeaturesVersion);

        public static bool HasExtendedAddItemPacket(byte[] version) => GreaterThanOrEqualTo(version, _extendedAddItemToContainer);

        public static bool EqualTo(byte[] a, byte[] b)
        {
            if (a == null || b == null)
                return false;
            if (a.Length != b.Length)
                return false;
            var index = 0;
            while (index < a.Length)
            {
                if (a[index] != b[index])
                    return false;
                index++;
            }
            return true;
        }

        /// <summary> Compare two arrays of equal size. Returns true if first parameter array is greater than or equal to second. </summary>
        static bool GreaterThanOrEqualTo(byte[] a, byte[] b)
        {
            if (a == null || b == null)
                return false;
            if (a.Length != b.Length)
                return false;
            var index = 0;
            while (index < a.Length)
            {
                if (a[index] > b[index])
                    return true;
                if (a[index] < b[index])
                    return false;
                index++;
            }
            return true;
        }
    }
}