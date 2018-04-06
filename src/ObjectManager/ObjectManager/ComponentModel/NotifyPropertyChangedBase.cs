using System;
using System.Web.Script.Serialization;

namespace OA.ComponentModel
{
    public abstract class NotifyPropertyChangedBase
    {
        [ScriptIgnore]
        public EventHandler PropertyChanged;

        public virtual bool SetProperty<T>(ref T storage, T value)
        {
            if (EqualityHelper.IsEqual(storage, value))
                return false;
            storage = value;
            OnPropertyChanged();
            return true;
        }

        protected virtual void OnPropertyChanged()
        {
            PropertyChanged?.Invoke(this, null);
        }

        protected float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
