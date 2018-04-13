using Microsoft.Win32;
using OA.Core;
using OA.Ultima.Data;
using System;
using System.Collections.Generic;
using System.IO;

namespace OA.Ultima.IO
{
    public class FileManager
    {
        static bool _isDataPresent;
        static public bool IsDataPresent => _isDataPresent;

        static readonly string[] _knownRegkeys = {
                @"Origin Worlds Online\Ultima Online\KR Legacy Beta",
                @"EA Games\Ultima Online: Mondain's Legacy\1.00.0000",
                @"Origin Worlds Online\Ultima Online\1.0",
                @"Origin Worlds Online\Ultima Online Third Dawn\1.0",
                @"EA GAMES\Ultima Online Samurai Empire",
                @"EA Games\Ultima Online: Mondain's Legacy",
                @"EA GAMES\Ultima Online Samurai Empire\1.0",
                @"EA GAMES\Ultima Online Samurai Empire\1.00.0000",
                @"EA GAMES\Ultima Online: Samurai Empire\1.0",
                @"EA GAMES\Ultima Online: Samurai Empire\1.00.0000",
                @"EA Games\Ultima Online: Mondain's Legacy\1.0",
                @"EA Games\Ultima Online: Mondain's Legacy\1.00.0000",
                @"Origin Worlds Online\Ultima Online Samurai Empire BETA\2d\1.0",
                @"Origin Worlds Online\Ultima Online Samurai Empire BETA\3d\1.0",
                @"Origin Worlds Online\Ultima Online Samurai Empire\2d\1.0",
                @"Origin Worlds Online\Ultima Online Samurai Empire\3d\1.0",
                @"Electronic Arts\EA Games\Ultima Online Stygian Abyss Classic",
                @"Electronic Arts\EA Games\Ultima Online Classic",
                @"Electronic Arts\EA Games\"
            };

        static string _fileDirectory;

        public static bool Is64Bit => IntPtr.Size == 8;
        public static int ItemIDMask => ClientVersion.InstallationIsUopFormat ? 0xffff : 0x3fff;

        static FileManager()
        {
            var ultimaOnline = UltimaSettings.UltimaOnline;
            Utils.Debug($"Initializing UO Data. Is64Bit = {Is64Bit}");
            Utils.Debug("Looking for UO Installation:");
            if (ultimaOnline.DataDirectory != null && Directory.Exists(ultimaOnline.DataDirectory))
            {
                Utils.Debug($"Settings: {ultimaOnline.DataDirectory}");
                _fileDirectory = ultimaOnline.DataDirectory;
                _isDataPresent = true;
            }
            else
            {
                for (var i = 0; i < _knownRegkeys.Length; i++)
                {
                    var exePath = GetExePath(Is64Bit ? $"Wow6432Node\\{_knownRegkeys[i]}" : _knownRegkeys[i]);
                    if (exePath != null && Directory.Exists(exePath))
                    {
                        if (IsClientIsCompatible(exePath))
                        {
                            Utils.Debug($"Compatible: {exePath}");
                            ultimaOnline.DataDirectory = exePath;
                            _fileDirectory = exePath;
                            _isDataPresent = true;
                        }
                        else Utils.Debug($"Incompatible: {exePath}");
                    }
                }
            }
            if (_fileDirectory == null)
                _isDataPresent = false;
            else
            {
                Utils.Debug(string.Empty);
                Utils.Debug($"Selected: {_fileDirectory}");
                var clientVersion = string.Join(".", ClientVersion.ClientExe);
                var patchVersion = string.Join(".", ultimaOnline.PatchVersion);
                Utils.Debug($"Client.Exe version: {clientVersion}; Patch version reported to server: {patchVersion}");
                if (!ClientVersion.EqualTo(ultimaOnline.PatchVersion, ClientVersion.DefaultVersion))
                    Utils.Warning($"Note from ZaneDubya: I will not support any code where the Patch version is not {string.Join(".", ClientVersion.DefaultVersion)}");
            }
        }

        static bool IsClientIsCompatible(string path)
        {
            var files = Directory.EnumerateFiles(path);
            foreach (var filepath in files)
            {
                var extension = Path.GetExtension(filepath).ToLower();
                if (extension == ".uop")
                    return false;
            }
            return true;
        }

        static string GetExePath(string subName)
        {
            try
            {
                var key = Registry.LocalMachine.OpenSubKey($"SOFTWARE\\{subName}");
                if (key == null)
                {
                    key = Registry.CurrentUser.OpenSubKey($"SOFTWARE\\{subName}");
                    if (key == null)
                        return null;
                }
                var path = key.GetValue("ExePath") as string;
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                {
                    path = key.GetValue("Install Dir") as string;
                    if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                    {
                        path = key.GetValue("InstallDir") as string;
                        if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                            return null;
                    }
                }
                if (File.Exists(path))
                    path = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                    return null;
                return path;
            }
            catch { return null; }
        }

        public static string GetFilePath(string path)
        {
            if (_fileDirectory != null)
            {
                path = Path.Combine(_fileDirectory, path);
                if (File.Exists(path))
                    return path;
            }
            return null;
        }

        public static string[] GetFilePaths(string searchPattern)
        {
            var files = Directory.GetFiles(_fileDirectory, searchPattern);
            return files;
        }

        public static bool Exists(string name)
        {
            try
            {
                name = Path.Combine(_fileDirectory, name);
                Utils.Debug($"Checking if file exists [{name}]");
                if (File.Exists(name))
                    return true;
                return false;
            }
            catch { return false; }
        }

        public static bool Exists(string name, int index, string type) => Exists($"{name}{index}.{type}");

        public static bool Exists(string name, string type) => Exists($"{name}.{type}");

        public static FileStream GetFile(string path)
        {
            try
            {
                path = Path.Combine(_fileDirectory, path);
                return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            catch { return null; }
        }

        public static FileStream GetFile(string name, uint index, string type) => GetFile($"{name}{index}.{type}");

        public static FileStream GetFile(string name, string type) => GetFile($"{name}.{type}");

        public static string GetPath(string name) => Path.Combine(_fileDirectory, name);

        public static AFileIndex CreateFileIndex(string uopFile, int length, bool hasExtra, string extension)
        {
            uopFile = GetPath(uopFile);
            var fileIndex = new UopFileIndex(uopFile, length, hasExtra, extension);
            return fileIndex;
        }

        public static AFileIndex CreateFileIndex(string idxFile, string mulFile, int length, int patch_file)
        {
            idxFile = GetPath(idxFile);
            mulFile = GetPath(mulFile);
            var fileIndex = new MulFileIndex(idxFile, mulFile, length, patch_file);
            return fileIndex;
        }
    }
}