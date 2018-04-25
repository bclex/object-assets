using OA.Ultima.Core.IO;
using OA.Ultima.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OA.Ultima.Resources
{
    public class SkillsData
    {
        public static int DefaultLength = 55;

        static AFileIndex _fileIndex = FileManager.CreateFileIndex("Skills.idx", "Skills.mul", DefaultLength, -1);
        public static AFileIndex FileIndex { get { return _fileIndex; } }

        static Skill[] _list = new Skill[DefaultLength];
        public static Skill[] List { get { return _list; } }
        static string[] _listNames;
        public static string[] ListNames
        {
            get
            {
                if (_listNames == null)
                {
                    _listNames = new string[_list.Length];
                    for (var i = 0; i < _list.Length; i++)
                        _listNames[i] = _list[i].Name;
                }
                return _listNames;
            }
        }

        public static void Initialize()
        {
            for (var i = 0; i < DefaultLength; i++)
                GetSkill(i);
        }

        public static Skill GetSkill(int index)
        {
            if (_list[index] != null)
                return _list[index];
            var r = _fileIndex.Seek(index, out int length, out int extra, out bool patched);
            if (r == null)
                return _list[index] = new Skill(SkillVars.NullV);
            return _list[index] = LoadSkill(index, r);
        }

        private static unsafe Skill LoadSkill(int index, BinaryFileReader reader)
        {
            var nameLength = _fileIndex.Index[index].Length - 2;
            var extra = _fileIndex.Index[index].Extra;
            var set1 = new byte[1];
            var set2 = new byte[nameLength];
            var set3 = new byte[1];
            set1 = reader.ReadBytes(1);
            set2 = reader.ReadBytes(nameLength);
            set3 = reader.ReadBytes(1);
            var useBtn = ToBool(set1);
            var name = ToString(set2);
            return new Skill(new SkillVars(index, name, useBtn, extra, set3[0]));
        }

        public static string ToString(byte[] buffer)
        {
            var b = new StringBuilder(buffer.Length);
            for (var i = 0; i < buffer.Length; i++)
                b.Append(ToString(buffer[i]));
            return b.ToString();
        }

        public static bool ToBool(byte[] buffer)
        {
            return BitConverter.ToBoolean(buffer, 0);
        }

        public static string ToString(byte b)
        {
            return ToString((char)b);
        }

        public static string ToString(char c)
        {
            return c.ToString();
        }

        public static string ToHexidecimal(int input, int length)
        {
            return String.Format("0x{0:X{1}}", input, length);
        }
    }

    public class Skill
    {
        SkillVars _data;
        public SkillVars Data
        {
            get { return _data; }
        }

        int _index = -1;
        public int Index
        {
            get { return _index; }
        }

        bool _useButton;
        public bool UseButton
        {
            get { return _useButton; }
            set { _useButton = value; }
        }

        string _name = String.Empty;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        SkillCategory _category;
        public SkillCategory Category
        {
            get { return _category; }
            set { _category = value; }
        }

        byte _unknown;
        public byte Unknown
        {
            get { return _unknown; }
        }

        public int ID
        {
            get { return _index + 1; }
        }

        public Skill(SkillVars data)
        {
            _data = data;
            _index = _data.Index;
            _useButton = _data.UseButton;
            _name = _data.Name;
            _category = _data.Category;
            _unknown = _data.Unknown;
        }

        public void ResetFromData()
        {
            _index = _data.Index;
            _useButton = _data.UseButton;
            _name = _data.Name;
            _category = _data.Category;
            _unknown = _data.Unknown;
        }

        public void ResetFromData(SkillVars data)
        {
            _data = data;
            _index = _data.Index;
            _useButton = _data.UseButton;
            _name = _data.Name;
            _category = _data.Category;
            _unknown = _data.Unknown;
        }

        public override string ToString()
        {
            return String.Format("{0} ({1:X4}) {2} {3}", _index, _index, _useButton ? "[x]" : "[ ]", _name);
        }
    }

    public sealed class SkillVars
    {
        public static SkillVars NullV
        {
            get { return new SkillVars(-1, "null", false, 0, 0x0); }
        }

        int _index = -1;
        public int Index
        {
            get { return _index; }
        }

        readonly string _name = String.Empty;
        public string Name
        {
            get { return _name; }
        }

        readonly int _extra;
        public int Extra
        {
            get { return _extra; }
        }

        readonly bool _useButton;
        public bool UseButton
        {
            get { return _useButton; }
        }

        readonly byte _unknown;
        public byte Unknown
        {
            get { return _unknown; }
        }

        SkillCategory _category;
        public SkillCategory Category
        {
            get { return _category; }
        }

        public int NameLength
        {
            get { return _name.Length; }
        }

        public SkillVars(int index, string name, bool useButton, int extra, byte unk)
        {
            _index = index;
            _name = name;
            _useButton = useButton;
            _extra = extra;
            _unknown = unk;
            _category = null;
        }
    }

    public class SkillCategories
    {
        static SkillCategory[] _list = new SkillCategory[0];
        public static SkillCategory[] List
        {
            get { return _list; }
        }

        private SkillCategories() { }

        public static SkillCategory GetCategory(int index)
        {
            if (_list.Length > 0)
                if (index < _list.Length)
                    return _list[index];
            _list = LoadCategories();
            if (_list.Length > 0)
                return GetCategory(index);
            return new SkillCategory(SkillCategoryData.DefaultData);
        }

        private static unsafe SkillCategory[] LoadCategories()
        {
            var list = new SkillCategory[0];
            var grpPath = FileManager.GetFilePath("skillgrp.mul");
            if (grpPath == null) return new SkillCategory[0];
            else
            {
                var toAdd = new List<SkillCategory>();
                using (var stream = new FileStream(grpPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    var bin = new BinaryReader(stream);
                    var START = new byte[4]; //File Start Offset
                    bin.Read(START, 0, 4);
                    var index = 0;
                    long x = stream.Length, y = 0;
                    while (y < x) //Position < Length
                    {
                        var name = ParseName(stream);
                        var fileIndex = stream.Position - name.Length;
                        if (name.Length > 0)
                        {
                            toAdd.Add(new SkillCategory(new SkillCategoryData(fileIndex, index, name)));
                            y = stream.Position;
                            ++index;
                        }
                    }
                }
                if (toAdd.Count > 0)
                {
                    list = new SkillCategory[toAdd.Count];
                    for (var i = 0; i < toAdd.Count; i++)
                        list[i] = toAdd[i];
                    toAdd.Clear();
                }
            }
            return list;
        }

        private static unsafe string ParseName(Stream stream)
        {
            var bin = new BinaryReader(stream);
            var tempName = String.Empty;
            var esc = false;
            while (!esc && bin.PeekChar() != -1)
            {
                var DATA = new byte[1];
                bin.Read(DATA, 0, 1);
                var c = (char)DATA[0];
                if (Char.IsLetterOrDigit(c) || Char.IsWhiteSpace(c) || Char.IsPunctuation(c))
                {
                    tempName += SkillsData.ToString(c);
                    continue;
                }
                esc = true;
            }
            return tempName.Trim();
        }
    }

    public class SkillCategory
    {
        SkillCategoryData _data;
        public SkillCategoryData Data { get { return _data; } }

        int _index = -1;
        public int Index { get { return _index; } }

        string _name = String.Empty;
        public string Name { get { return _name; } }

        public SkillCategory(SkillCategoryData data)
        {
            _data = data;
            _index = _data.Index;
            _name = _data.Name;
        }

        public void ResetFromData()
        {
            _index = _data.Index;
            _name = _data.Name;
        }

        public void ResetFromData(SkillCategoryData data)
        {
            _data = data;
            _index = _data.Index;
            _name = _data.Name;
        }
    }

    public sealed class SkillCategoryData
    {
        public static SkillCategoryData DefaultData { get { return new SkillCategoryData(0, -1, "null"); } }

        long _fileIndex = -1;
        public long FileIndex
        {
            get { return _fileIndex; }
        }

        int _index = -1;
        public int Index
        {
            get { return _index; }
        }

        readonly string _name = String.Empty;
        public string Name
        {
            get { return _name; }
        }

        public SkillCategoryData(long fileIndex, int index, string name)
        {
            _fileIndex = fileIndex;
            _index = index;
            _name = name;
        }
    }
}