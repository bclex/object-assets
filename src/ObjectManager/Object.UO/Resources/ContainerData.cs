using OA.Ultima.Core;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace OA.Ultima.Resources
{
    public class ContainerData
    {
        static ContainerData _default;
        static Dictionary<int, ContainerData> _table;

        static ContainerData()
        {
            _table = new Dictionary<int, ContainerData>();
            var path = @"data/containers.cfg";
            if (!File.Exists(path))
            {
                _default = new ContainerData(0x3C, new RectInt(44, 65, 142, 94), 0x48);
                return;
            }
            using (var reader = new StreamReader(path))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.Length == 0 || line.StartsWith("#"))
                        continue;
                    try
                    {
                        var split = line.Split('\t');
                        if (split.Length >= 3)
                        {
                            var gumpID = Utility.ToInt32(split[0]);
                            string[] aRect = split[1].Split(' ');
                            if (aRect.Length < 4)
                                continue;
                            var x = Utility.ToInt32(aRect[0]);
                            var y = Utility.ToInt32(aRect[1]);
                            var width = Utility.ToInt32(aRect[2]);
                            var height = Utility.ToInt32(aRect[3]);
                            var bounds = new RectInt(x, y, width, height);
                            var dropSound = Utility.ToInt32(split[2]);
                            var data = new ContainerData(gumpID, bounds, dropSound);
                            if (_default == null)
                                _default = data;
                            if (split.Length >= 4)
                            {
                                var aIDs = split[3].Split(',');
                                for (var i = 0; i < aIDs.Length; i++)
                                {
                                    var id = Utility.ToInt32(aIDs[i]);
                                    if (_table.ContainsKey(id)) Console.WriteLine(@"Warning: double ItemID entry in Data\containers.cfg");
                                    else _table[id] = data;
                                }
                            }
                        }
                    }
                    catch { }
                }
            }
            if (_default == null)
                _default = new ContainerData(0x3C, new RectInt(44, 65, 142, 94), 0x48);
        }

        public static ContainerData Default
        {
            get { return _default; }
            set { _default = value; }
        }

        public static ContainerData Get(int itemID)
        {
            ContainerData data = null;
            _table.TryGetValue(itemID, out data);
            if (data != null) return data;
            else return _default;
        }

        readonly int _gumpID;
        readonly RectInt _bounds;
        readonly int _dropSound;

        public int GumpID { get { return _gumpID; } }
        public RectInt Bounds { get { return _bounds; } }
        public int DropSound { get { return _dropSound; } }

        public ContainerData(int gumpID, RectInt bounds, int dropSound)
        {
            _gumpID = gumpID;
            _bounds = bounds;
            _dropSound = dropSound;
        }
    }
}
