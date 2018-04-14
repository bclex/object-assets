using OA.Core;
using OA.Core.Windows;
using OA.Ultima.Core.IO;
using System;
using System.Collections.Generic;
using System.IO;

namespace OA.Ultima.Input
{
    public class PlayerMacros
    {
        public int Count
        {
            get { return All.Count; }
        }

        static List<Action> Empty = new List<Action>();
        List<Action> _macros;

        public List<Action> All
        {
            get
            {
                if (_macros == null)
                    return Empty;
                return _macros;
            }
        }

        public void AddNewMacroAction(Action action)
        {
            _macros.Add(action);
        }

        public void AddNewMacroAction(Action action, int index)
        {
            if (index < 0 || index >= _macros.Count) AddNewMacroAction(action);
            else _macros.Insert(index, action);
        }

        public void RemoveMacroAction(Action action)
        {
            _macros.Remove(action);
        }

        // ============================================================================================================
        // Load / save
        // ============================================================================================================

        const uint MAGIC = 0xF14934E0;
        const string c_PathAppend = "_macros2d.txt";
        string _path;

        public void Save()
        {
            var fileExists = false;
            if (_path == null)
                return;
            if (All.Count == 0)
            {
                if (File.Exists(_path))
                {
                    try { File.Delete(_path); }
                    catch { } // ... could not delete?
                }
                return;
            }

            if (File.Exists(_path))
            {
                fileExists = true;
                File.Copy(_path, _path + ".bak", true);
            }

            try
            {
                Utils.Debug("Saving macros...");
                var w = new BinaryFileWriter(_path, false);
                Serialize(w);
                w.Close();
                w = null;
            }
            catch (Exception e)
            {
                Utils.Warning($"Error saving macros: {e.Message}");
                Utils.Debug("Attempting to restore old macros...");
                if (fileExists)
                    File.Copy(_path + ".bak", _path);
                Utils.Debug("Old macros restored.");
            }
        }

        public void Load()
        {
            if (_path == null || _path == string.Empty)
                return;
            if (!File.Exists(_path))
            {
                Utils.Debug("No macros to load. Creating deafult macro set");
                CreateDefaultMacroSet();
                return;
            }

            try
            {
                var r = new BinaryFileReader(new BinaryReader(new FileStream(_path, FileMode.Open)));
                if (Unserialize(r)) Utils.Debug("Macros loaded!");
                else Utils.Debug("Error reading macro file.");
                r.Close();
                r = null;
            }
            catch (Exception e)
            {
                Utils.Warning($"Error loading macros: {e.Message}");
            }
        }

        public void Load(string username)
        {
            _path = string.Format("{0}{1}", username, c_PathAppend);
            Load();
        }

        private void Serialize(BinaryFileWriter w)
        {
            w.Write(MAGIC);
            w.Write((int)0); // version
            w.Write((int)All.Count);
            for (var i = 0; i < All.Count; i++)
            {
                w.Write((ushort)All[i].Keystroke);
                w.Write(All[i].Ctrl);
                w.Write(All[i].Alt);
                w.Write(All[i].Shift);
                w.Write(false);

                w.Write((ushort)All[i].Macros.Count);
                for (var j = 0; j < All[i].Macros.Count; j++)
                {
                    w.Write((int)All[i].Macros[j].Type);
                    w.Write((byte)All[i].Macros[j].ValueType);
                    if (All[i].Macros[j].ValueType == Macro.ValueTypes.Integer)
                        w.Write((int)All[i].Macros[j].ValueInteger);
                    else if (All[i].Macros[j].ValueType == Macro.ValueTypes.String)
                        w.Write((string)All[i].Macros[j].ValueString);
                }
            }
        }

        private bool Unserialize(BinaryFileReader r)
        {
            var magic = r.ReadUInt();
            if (magic != MAGIC)
                return false;

            if (_macros == null)
                _macros = new List<Action>();
            _macros.Clear();
            var version = r.ReadInt();
            var count = r.ReadInt();
            for (var i = 0; i < count; i++)
            {
                var action = new Action();
                action.Keystroke = (WinKeys)r.ReadUShort();
                action.Ctrl = r.ReadBool();
                action.Alt = r.ReadBool();
                action.Shift = r.ReadBool();
                r.ReadBool(); // unused filler byte

                var macroCount = r.ReadUShort();
                for (var j = 0; j < macroCount; j++)
                {
                    var type = r.ReadInt();
                    var valueType = (Macro.ValueTypes)r.ReadByte();
                    if (valueType == Macro.ValueTypes.Integer) action.Macros.Add(new Macro((MacroType)type, r.ReadInt()));
                    else if (valueType == Macro.ValueTypes.String) action.Macros.Add(new Macro((MacroType)type, r.ReadString()));
                    else action.Macros.Add(new Macro((MacroType)type));
                }
                _macros.Add(action);

            }

            return true;
        }

        // ============================================================================================================
        // Default macro set
        // ============================================================================================================

        public void CreateDefaultMacroSet()
        {
            if (_macros == null)
                _macros = new List<Action>();
            _macros.Clear();

            Action action;
            
            action = new Action();
            action.Keystroke = WinKeys.S;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.OpenGump, (int)MacroDisplay.Status));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.T;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.OpenGump, (int)MacroDisplay.Chat));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.B;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.OpenGump, (int)MacroDisplay.MageSpellbook));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.C;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.ToggleWarPeace));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.P;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.OpenGump, (int)MacroDisplay.Paperdoll));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.K;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.OpenGump, (int)MacroDisplay.Skills));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.J;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.OpenGump, (int)MacroDisplay.Journal));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.Q;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.OpenGump, (int)MacroDisplay.QuestLog));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.W;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.OpenGump, (int)MacroDisplay.SpellWeavingSpellbook));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.I;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.OpenGump, (int)MacroDisplay.Backpack));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.R;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.OpenGump, (int)MacroDisplay.Overview));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.O;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.OpenGump, (int)MacroDisplay.Configuration));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.X;
            action.Alt = true;
            action.Macros.Add(new Macro(MacroType.TargetSelf));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.S;
            action.Shift = true;
            action.Macros.Add(new Macro(MacroType.LastTarget));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.NumPad8;
            action.Macros.Add(new Macro(MacroType.Say, "Forward"));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.NumPad5;
            action.Macros.Add(new Macro(MacroType.Say, "Stop"));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.NumPad4;
            action.Macros.Add(new Macro(MacroType.Say, "Turn Left"));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.NumPad6;
            action.Macros.Add(new Macro(MacroType.Say, "Turn Right"));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.NumPad7;
            action.Macros.Add(new Macro(MacroType.Say, "Left"));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.NumPad9;
            action.Macros.Add(new Macro(MacroType.Say, "Right"));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.NumPad2;
            action.Macros.Add(new Macro(MacroType.Say, "Backwards"));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.NumPad1;
            action.Macros.Add(new Macro(MacroType.Say, "Raise Anchor"));
            AddNewMacroAction(action);
            
            action = new Action();
            action.Keystroke = WinKeys.NumPad3;
            action.Macros.Add(new Macro(MacroType.Say, "Drop Anchor"));
            AddNewMacroAction(action);

            // NİGHTSİGHT
            action = new Action();
            action.Keystroke = WinKeys.F1;
            action.Macros.Add(new Macro(MacroType.CastSpell, 5)); 
            action.Macros.Add(new Macro(MacroType.LastTarget));
            AddNewMacroAction(action);
            
            // TEST
            action = new Action();
            action.Keystroke = WinKeys.F2;
            action.Macros.Add(new Macro(MacroType.Say, "Delaying 1 second."));
            action.Macros.Add(new Macro(MacroType.Delay, "10"));
            action.Macros.Add(new Macro(MacroType.Say, "Delay 2 seconds."));
            action.Macros.Add(new Macro(MacroType.Delay, "20"));
            action.Macros.Add(new Macro(MacroType.Say, "Delay complete!"));
            AddNewMacroAction(action);
            
            Save();
        }
    }
}
