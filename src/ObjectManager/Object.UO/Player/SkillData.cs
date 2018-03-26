using OA.Ultima.Resources;
using System;
using System.Collections.Generic;

namespace OA.Ultima.Player
{
    public class SkillData
    {
        public Action<SkillEntry> OnSkillChanged;

        private readonly Dictionary<int, SkillEntry> _skills = new Dictionary<int, SkillEntry>();
        private bool _skillsLoaded;

        public Dictionary<int, SkillEntry> List
        {
            get
            {
                if (!_skillsLoaded)
                {
                    _skillsLoaded = true;
                    foreach (var skill in SkillsData.List)
                        if (skill.Index == -1) { } // do nothing.
                        else _skills.Add(skill.ID, new SkillEntry(this, skill.ID, skill.Index, skill.UseButton, skill.Name, 0.0f, 0.0f, 0, 0.0f));
                }
                return _skills;
            }
        }

        public SkillEntry SkillEntry(int skillID)
        {
            if (List.Count > skillID) return List[skillID];
            else return null;
        }

        public SkillEntry SkillEntryByIndex(int index)
        {
            foreach (var skill in _skills.Values)
                if (skill.Index == index)
                    return skill;
            return null;
        }
    }

    public class SkillEntry
    {
        readonly SkillData _dataParent;

        int _id;
        int _index;
        bool _hasUseButton;
        string _name;
        float _value;
        float _valueUnmodified;
        byte _lockType;
        float _cap;

        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }
        public int Index
        {
            get { return _index; }
            set { _index = value; }
        }
        public bool HasUseButton
        {
            get { return _hasUseButton; }
            set { _hasUseButton = value; }
        }
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public float Value
        {
            get { return _value; }
            set
            {
                _value = value;
                _dataParent.OnSkillChanged?.Invoke(this);
            }
        }
        public float ValueUnmodified
        {
            get { return _valueUnmodified; }
            set
            {
                _valueUnmodified = value;
                _dataParent.OnSkillChanged?.Invoke(this);
            }
        }
        public byte LockType
        {
            get { return _lockType; }
            set
            {
                _lockType = value;
                _dataParent.OnSkillChanged?.Invoke(this);
            }
        }
        public float Cap
        {
            get { return _cap; }
            set
            {
                _cap = value;
                _dataParent.OnSkillChanged?.Invoke(this);
            }
        }

        public SkillEntry(SkillData dataParent, int id, int index, bool useButton, string name, float value, float unmodified, byte locktype, float cap)
        {
            _dataParent = dataParent;
            ID = id;
            Index = index;
            HasUseButton = useButton;
            Name = name;
            Value = value;
            ValueUnmodified = unmodified;
            LockType = locktype;
            Cap = cap;
        }
    }
}
