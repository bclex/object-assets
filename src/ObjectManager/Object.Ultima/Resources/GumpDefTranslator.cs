using OA.Core;
using OA.Ultima.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace OA.Ultima.Resources
{
    public class GumpDefTranslator
    {
        static readonly Dictionary<int, Tuple<int, int>> _translations;

        static GumpDefTranslator()
        {
            _translations = new Dictionary<int, Tuple<int, int>>();
            StreamReader gumpDefFile = null;
            try
            {
                gumpDefFile = new StreamReader(FileManager.GetFile("gump.def"));
            }
            catch { Utils.Warning("GumpDefTranslator: unable to open gump.def file. No item/itemgumpling translations are available."); return; }
            try
            {
                string line;
                while ((line = gumpDefFile.ReadLine()) != null)
                {
                    line = line.Trim();
                    if (line.Length <= 0)
                        continue;
                    if (line[0] == '#')
                        continue;
                    var defs = line.Replace('\t', ' ').Split(' ');
                    if (defs.Length != 3)
                        continue;
                    var inGump = int.Parse(defs[0]);
                    var outGump = int.Parse(defs[1].Replace("{", string.Empty).Replace("}", string.Empty));
                    var outHue = int.Parse(defs[2]);
                    if (_translations.ContainsKey(inGump))
                        _translations.Remove(inGump);
                    _translations.Add(inGump, new Tuple<int, int>(outGump, outHue));
                }
            }
            catch { Utils.Warning("GumpDefTranslator: unable to parse gump.def file. No item/itemgumpling translations are available."); }
            gumpDefFile.Close();
        }

        public static bool ItemHasGumpTranslation(int gumpIndex, out int gumpIndexTranslated, out int defaultHue)
        {
            if (_translations.TryGetValue(gumpIndex, out Tuple<int, int> translation))
            {
                gumpIndexTranslated = translation.Item1;
                defaultHue = translation.Item2;
                return true;
            }
            gumpIndexTranslated = 0;
            defaultHue = 0;
            return false;
        }
    }
}
