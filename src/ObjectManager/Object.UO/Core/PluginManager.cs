using OA.Core;
using OA.Ultima.Core.Patterns;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OA.Ultima.Core
{
    class PluginManager
    {
        readonly List<IModule> _modules = new List<IModule>();

        public PluginManager(string baseAppPath)
        {
            Configure(baseAppPath);
        }

        void Configure(string baseAppPath)
        {
            var directory = new DirectoryInfo(Path.Combine(baseAppPath, "plugins"));
            if (!directory.Exists)
                return;
            var assemblies = directory.GetFiles("*.dll");
            foreach (var file in assemblies)
            {
                try
                {
                    Utils.Info("Loading plugin {0}.", file.Name);
                    var assembly = Assembly.LoadFile(file.FullName);
                    var modules = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IModule)));
                    foreach (var module in modules)
                    {
                        Utils.Info("Activating module {0}.", module.FullName);
                        var instance = (IModule)Activator.CreateInstance(module);
                        LoadModule(instance);
                    }
                }
                catch (Exception e)
                {
                    Utils.Warning($"An error occurred while trying to load plugin. [{file.FullName}]");
                    Utils.Warning(e.Message);
                }
            }
        }

        void LoadModule(IModule module)
        {
            _modules.Add(module);
            module.Load();
        }
    }
}
