using Microsoft.Win32;
using OA.Core;
using System;
using System.Collections.Generic;
using System.IO;

namespace OA.Tes.IO
{
    public class FileManager
    {
        static bool _isDataPresent;
        static public bool IsDataPresent => _isDataPresent;

        static readonly object[] _knownRegkeys = {
            @"Bethesda Softworks\Oblivion", GameId.Oblivion,
            @"Bethesda Softworks\Skyrim", GameId.Skyrim,
            @"Bethesda Softworks\Fallout 3", GameId.Fallout3,
            @"Bethesda Softworks\Fallout NV", GameId.FalloutNV,
            @"Bethesda Softworks\Morrowind", GameId.Morrowind,
            @"Bethesda Softworks\Fallout 4", GameId.Fallout4,
            @"Bethesda Softworks\Skyrim SE", GameId.SkyrimSE,
            @"Bethesda Softworks\Fallout 4 VR", GameId.Fallout4VR,
            @"Bethesda Softworks\Skyrim VR", GameId.SkyrimVR
        };

        static Dictionary<GameId, string> _fileDirectories = new Dictionary<GameId, string>();

        public static bool Is64Bit => true; // IntPtr.Size == 8;

        static FileManager()
        {
            var tesRender = TesSettings.TesRender;
            Utils.Debug($"Initializing TES Data. Is64Bit = {Is64Bit}");
            Utils.Debug("Looking for TES Installation(s):");
            if (tesRender.DataDirectory != null && Directory.Exists(tesRender.DataDirectory))
            {
                Utils.Debug($"Settings: {tesRender.DataDirectory}");
                var gameId = (GameId)Enum.Parse(typeof(GameId), tesRender.GameId);
                _fileDirectories.Add(gameId, tesRender.DataDirectory);
                _isDataPresent = true;
            }
            else
            {
                for (var i = 0; i < _knownRegkeys.Length; i += 2)
                {
                    var exePath = GetExePath(Is64Bit ? $"Wow6432Node\\{(string)_knownRegkeys[i]}" : (string)_knownRegkeys[i]);
                    if (exePath != null && Directory.Exists(exePath))
                    {
                        var dataPath = Path.Combine(exePath, "Data");
                        var gameId = (GameId)_knownRegkeys[i + 1];
                        if (Directory.Exists(dataPath))
                        {
                            Utils.Debug($"Compatible: {dataPath}");
                            Utils.Debug($"GameId: {gameId}");
                            //tesRender.DataDirectory = dataPath;
                            //tesRender.GameId = gameId.ToString();
                            _fileDirectories.Add(gameId, dataPath);
                            _isDataPresent = true;
                        }
                        else Utils.Debug($"Incompatible: {dataPath}");
                    }
                }
            }
            if (_fileDirectories.Count == 0)
                _isDataPresent = false;
            else
            {
                Utils.Debug(string.Empty);
                Utils.Debug($"Selected: {_fileDirectories.Count}");
            }
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
                    {
                        key = Registry.ClassesRoot.OpenSubKey($"VirtualStore\\MACHINE\\SOFTWARE\\{subName}");
                        if (key == null)
                            return null;
                    }
                }
                var path = key.GetValue("Installed Path") as string;
                if (File.Exists(path))
                    path = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                    return null;
                return path;
            }
            catch { return null; }
        }

        public static string GetFilePath(string path, GameId gameId)
        {
            if (_fileDirectories.TryGetValue(gameId, out string fileDirectory))
            {
                path = Path.Combine(fileDirectory, path);
                if (File.Exists(path))
                    return path;
            }
            return null;
        }

        //public static string[] GetFilePaths(string searchPattern)
        //{
        //    var files = Directory.GetFiles(_fileDirectory, searchPattern);
        //    return files;
        //}

        //public static bool Exists(string name)
        //{
        //    try
        //    {
        //        name = Path.Combine(_fileDirectory, name);
        //        Utils.Debug($"Checking if file exists [{name}]");
        //        if (File.Exists(name))
        //            return true;
        //        return false;
        //    }
        //    catch { return false; }
        //}

        //public static bool Exists(string name, int index, string type) => Exists($"{name}{index}.{type}");

        //public static bool Exists(string name, string type) => Exists($"{name}.{type}");

        //public static FileStream GetFile(string path)
        //{
        //    try
        //    {
        //        path = Path.Combine(_fileDirectory, path);
        //        return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        //    }
        //    catch { return null; }
        //}

        //public static FileStream GetFile(string name, uint index, string type) => GetFile($"{name}{index}.{type}");

        //public static FileStream GetFile(string name, string type) => GetFile($"{name}.{type}");

        //public static string GetPath(string name) => Path.Combine(_fileDirectory, name);
    }
}