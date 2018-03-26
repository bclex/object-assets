using OA.Core.Windows;

namespace OA.Core.Input
{
    public class InputEvent
    {
        readonly WinKeys _modifiers;
        bool _handled;

        public bool Alt => (_modifiers & WinKeys.Alt) == WinKeys.Alt;

        public bool Control => (_modifiers & WinKeys.Control) == WinKeys.Control;

        public bool Shift => (_modifiers & WinKeys.Shift) == WinKeys.Shift;

        public bool Handled
        {
            get { return _handled; }
            set { _handled = value; }
        }

        public InputEvent(WinKeys modifiers)
        {
            _modifiers = modifiers;
        }

        protected InputEvent(InputEvent parent)
        {
            _modifiers = parent._modifiers;
        }
    }
}
