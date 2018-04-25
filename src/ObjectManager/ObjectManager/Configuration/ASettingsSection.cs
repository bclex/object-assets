using OA.ComponentModel;
using System;
using System.ComponentModel;

namespace OA.Configuration
{
    public abstract class ASettingsSection : NotifyPropertyChangedBase
    {
        public event EventHandler Invalidated;

        public override bool SetProperty<T>(ref T storage, T value)
        {
            if (!base.SetProperty<T>(ref storage, value))
                return false;
            var notifier = storage as INotifyPropertyChanged;
            if (notifier != null)
                // Stop listening to the old value since it is no longer part of the settings section.
                notifier.PropertyChanged -= OnSectionPropertyChanged;
            notifier = value as INotifyPropertyChanged;
            if (notifier != null)
                // Start listening to the new value 
                notifier.PropertyChanged += OnSectionPropertyChanged;
            return true;
        }

        void OnSectionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnInvalidated();
        }

        private void OnInvalidated()
        {
            Invalidated?.Invoke(this, EventArgs.Empty);
        }

        protected int Clamp(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}