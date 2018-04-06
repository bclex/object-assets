using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace OA.Core.Windows
{
    public abstract class MessageHook : IDisposable
    {
        public abstract int HookType { get; }

        IntPtr _wnd;
        WndProcHandler _hook;
        IntPtr _prevWndProc;
        IntPtr _hIMC;

        public IntPtr HWnd
        {
            get { return _wnd; }
        }

        protected MessageHook(IntPtr hWnd)
        {
            _wnd = hWnd;
            _hook = WndProcHook;
            _prevWndProc = (IntPtr)NativeMethods.SetWindowLong(
                hWnd,
                NativeConstants.GWL_WNDPROC, (int)Marshal.GetFunctionPointerForDelegate(_hook));
            _hIMC = NativeMethods.ImmGetContext(_wnd);
            Application.AddMessageFilter(new InputMessageFilter(_hook));
        }

        ~MessageHook()
        {
            Dispose(false);
        }

        protected virtual IntPtr WndProcHook(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case NativeConstants.WM_GETDLGCODE:
                    return (IntPtr)(NativeConstants.DLGC_WANTALLKEYS);
               case NativeConstants.WM_IME_SETCONTEXT:
                    if ((int)wParam == 1)
                        NativeMethods.ImmAssociateContext(hWnd, _hIMC);
                    break;
                case NativeConstants.WM_INPUTLANGCHANGE:
                    int rrr = (int)NativeMethods.CallWindowProc(_prevWndProc, hWnd, msg, wParam, lParam);
                    NativeMethods.ImmAssociateContext(hWnd, _hIMC);
                    return (IntPtr)1;
            }
            return NativeMethods.CallWindowProc(_prevWndProc, hWnd, msg, wParam, lParam);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }

    // This is the class that brings back the alt messages
    // http://www.gamedev.net/community/forums/topic.asp?topic_id=554322
    class InputMessageFilter : IMessageFilter
    {
        readonly WndProcHandler _hook;

        public InputMessageFilter(WndProcHandler hook)
        {
            _hook = hook;
        }

        [DllImport("user32.dll", EntryPoint = "TranslateMessage")]
        protected extern static bool m_TranslateMessage(ref System.Windows.Forms.Message m);

        bool IMessageFilter.PreFilterMessage(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case NativeConstants.WM_SYSKEYDOWN:
                case NativeConstants.WM_SYSKEYUP:
                    {
                        var b = m_TranslateMessage(ref m);
                        _hook(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
                        return true;
                    }
                case NativeConstants.WM_SYSCHAR:
                    {
                        _hook(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
                        return true;
                    }
                case NativeConstants.WM_KEYDOWN:
                case NativeConstants.WM_KEYUP:
                    {
                        bool b = m_TranslateMessage(ref m);
                        _hook(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
                        return true;
                    }
                case NativeConstants.WM_CHAR:
                    {
                        _hook(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
                        return true;
                    }
                case NativeConstants.WM_DEADCHAR:
                    {
                        _hook(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
                        return true;
                    }
            }
            return false;
        }
    }
}
