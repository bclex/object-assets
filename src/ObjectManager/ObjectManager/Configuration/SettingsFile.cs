using OA.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace OA.Configuration
{
    public class SettingsFile
    {
        readonly Dictionary<string, ASettingsSection> _sectionCache;
        static readonly object _syncRoot = new object();

        readonly string _filename;
        readonly Timer _saveTimer;

        public SettingsFile(string filename)
        {
            _sectionCache = new Dictionary<string, ASettingsSection>();
            _saveTimer = new Timer
            {
                Interval = 10000, // save settings every 10 seconds
                AutoReset = true
            };
            _saveTimer.Elapsed += OnTimerElapsed;
            _filename = filename;
        }

        public bool Exists
        {
            get { return File.Exists(_filename); }
        }

        public void Save()
        {
            return;
            try
            {
                lock (_syncRoot)
                {
                    var serializer = new TextSettingsFileWriter(_filename);
                    serializer.Serialize(_sectionCache);
                }
            }
            catch (Exception e) { Utils.Exception(e); }
        }

        internal void InvalidateDirty()
        {
            // possibly save the file here?
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            // Lock the timer so we dont start it till its done save.
            lock (_saveTimer)
            {
                Save();
            }
        }

        public SettingsFile Load()
        {
            try
            {
                lock (_syncRoot)
                {
                    if (LoadFromFile(_filename)) Utils.Log("Read settings from settings file.");
                    else
                    {
                        if (File.Exists(_filename + ".bak"))
                        {
                            Utils.Error("Unable to read settings file.  Trying backup file");
                            if (LoadFromFile(_filename + ".bak")) Utils.Warning("Read settings from backup settings file.");
                            else Utils.Error("Unable to read backup settings file. All settings are set to default values.");
                        }
                        else Utils.Error("Unable to read settings file. All settings are set to default values.");
                    }
                }
            }
            catch (Exception e) { Utils.Exception(e); }
            _saveTimer.Enabled = true;
            return this;
        }

        private bool LoadFromFile(string fileName)
        {
            if (!File.Exists(fileName))
                return false;
            try
            {
                var serializer = new TextSettingsFileWriter(fileName);
                serializer.Deserialize(_sectionCache);
                return true;
            }
            catch (Exception e)
            {
                Utils.Exception(e);
                return false;
            }
        }

        internal T CreateOrOpenSection<T>(string sectionName)
            where T : ASettingsSection, new()
        {
            ASettingsSection section;
            // If we've already deserialized the section, just return it.
            if (_sectionCache.TryGetValue(sectionName, out section))
                return (T)section;
            section = new T();
            InvalidateDirty();
            _sectionCache[sectionName] = section;
            return (T)section;
        }
    }
}