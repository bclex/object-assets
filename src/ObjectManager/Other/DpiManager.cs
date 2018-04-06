using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace OA.Core
{
    static class DpiManager
    {
        const int LogPixelsX = 88; // Used for GetDeviceCaps().
        const int LogPixelsY = 90; // Used for GetDeviceCaps().
        const float StandardDpi = 96f; // Used for GetDeviceCaps().

        public static Vector2 GetSystemDpiScalar()
        {
            var result = new Vector2();
            var hdc = GetDC(IntPtr.Zero);
            result.x = GetDeviceCaps(hdc, LogPixelsX) / StandardDpi;
            result.y = GetDeviceCaps(hdc, LogPixelsY) / StandardDpi;
            ReleaseDC(IntPtr.Zero, hdc);
            return result;
        }

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    }
}