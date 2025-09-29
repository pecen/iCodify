using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace iCodify.UI.Wpf.AeroChromeDemo
{
    internal static class AeroBackdrop
    {
        // Win11 “System Backdrop” (Mica/Acrylic) constants
        private const int DWMWA_SYSTEMBACKDROP_TYPE = 38; // Windows 11 22H2+
        private const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20; // Win 10 1809+
        private const int DWMWA_BORDER_COLOR = 34;
        private const int DWMWA_CAPTION_COLOR = 35;
        private const int DWMWA_TEXT_COLOR = 36;

        // Backdrop types
        private const int DWMSBT_AUTO = 0;
        private const int DWMSBT_NONE = 1;
        private const int DWMSBT_MAINWINDOW = 2;  // Mica
        private const int DWMSBT_TRANSIENTWINDOW = 3;
        private const int DWMSBT_TABBEDWINDOW = 4; // Mica Alt
        private const int DWMSBT_ACRYLIC = 5;      // Acrylic (23H2+)

        // Win7/8/10 DWM blur (äldre API)
        private const int DWM_BB_ENABLE = 0x1;

        [StructLayout(LayoutKind.Sequential)]
        private struct MARGINS { public int cxLeftWidth, cxRightWidth, cyTopHeight, cyBottomHeight; }

        [StructLayout(LayoutKind.Sequential)]
        private struct DWM_BLURBEHIND
        {
            public int dwFlags;
            public bool fEnable;
            public IntPtr hRgnBlur;
            public bool fTransitionOnMaximized;
        }

        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmEnableBlurBehindWindow(IntPtr hWnd, ref DWM_BLURBEHIND pBlurBehind);

        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmIsCompositionEnabled(out bool pfEnabled);

        [DllImport("dwmapi.dll", PreserveSig = true)]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        public static void TryApply(Window window)
        {
            var hwnd = new WindowInteropHelper(window).EnsureHandle();

            // Försök modern Win11-backdrop först (Mica/Acrylic)
            if (TrySetSystemBackdrop(hwnd))
            {
                // Valfri: harmonisera färger lite
                TrySetDarkMode(hwnd, isDark: true);
                return;
            }

            // Annars: äldre DWM-blur/glas
            if (IsDwmEnabled())
            {
                // a) Prova blur-behind
                var bb = new DWM_BLURBEHIND
                {
                    dwFlags = DWM_BB_ENABLE,
                    fEnable = true,
                    hRgnBlur = IntPtr.Zero,
                    fTransitionOnMaximized = true
                };
                _ = DwmEnableBlurBehindWindow(hwnd, ref bb);

                // b) Alternativ: extend frame (klassisk Aero glass längs kanter)
                var margins = new MARGINS { cxLeftWidth = 1, cxRightWidth = 1, cyTopHeight = 36, cyBottomHeight = 1 };
                _ = DwmExtendFrameIntoClientArea(hwnd, ref margins);
            }
            else
            {
                // Fallback – inget att göra; XAML har en mörk brush
            }
        }

        private static bool TrySetSystemBackdrop(IntPtr hwnd)
        {
            try
            {
                // Prioritera Mica (MainWindow). Om Acrylic finns (Win11 23H2+) kan du testa DWMSBT_ACRYLIC.
                int backdrop = DWMSBT_MAINWINDOW;
                int hr = DwmSetWindowAttribute(hwnd, DWMWA_SYSTEMBACKDROP_TYPE, ref backdrop, sizeof(int));
                return hr == 0;
            }
            catch { return false; }
        }

        private static void TrySetDarkMode(IntPtr hwnd, bool isDark)
        {
            try
            {
                int dark = isDark ? 1 : 0;
                _ = DwmSetWindowAttribute(hwnd, DWMWA_USE_IMMERSIVE_DARK_MODE, ref dark, sizeof(int));
            }
            catch { /* ignore */ }
        }

        private static bool IsDwmEnabled()
            => DwmIsCompositionEnabled(out bool on) == 0 && on;

        // Fix för maximerat fönster så att det inte täcker bakgrundsskärmens hela workarea (t.ex. bakom taskbar).
        public static void FixMaximizedBounds(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            if (hwnd == IntPtr.Zero) return;

            var src = HwndSource.FromHwnd(hwnd);
            if (src == null) return;

            src.AddHook(WndProc);
        }

        public static void Cleanup(Window window)
        {
            // inget specifikt behövs för nu; hook tas bort när HwndSource försvinner
        }

        private const int WM_GETMINMAXINFO = 0x0024;

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT { public int X; public int Y; }

        [StructLayout(LayoutKind.Sequential)]
        private struct MINMAXINFO
        {
            public POINT ptReserved;
            public POINT ptMaxSize;
            public POINT ptMaxPosition;
            public POINT ptMinTrackSize;
            public POINT ptMaxTrackSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public int dwFlags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left, Top, Right, Bottom; }

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);
        private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_GETMINMAXINFO)
            {
                // Se till att maximerat fönster får rätt storlek relativt arbetsytan
                var mmi = Marshal.PtrToStructure<MINMAXINFO>(lParam);
                IntPtr monitor = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
                if (monitor != IntPtr.Zero)
                {
                    var mi = new MONITORINFO { cbSize = Marshal.SizeOf<MONITORINFO>() };
                    if (GetMonitorInfo(monitor, ref mi))
                    {
                        var work = mi.rcWork;
                        var monitorRect = mi.rcMonitor;
                        mmi.ptMaxPosition.X = Math.Abs(work.Left - monitorRect.Left);
                        mmi.ptMaxPosition.Y = Math.Abs(work.Top - monitorRect.Top);
                        mmi.ptMaxSize.X = Math.Abs(work.Right - work.Left);
                        mmi.ptMaxSize.Y = Math.Abs(work.Bottom - work.Top);
                        Marshal.StructureToPtr(mmi, lParam, true);
                        handled = true;
                    }
                }
            }
            return IntPtr.Zero;
        }

        // Små rundade hörn (Win11)
        public static void ApplyRoundedCorners(Window window)
        {
            try
            {
                var hwnd = new WindowInteropHelper(window).Handle;
                if (hwnd == IntPtr.Zero) return;

                // 33 = DWMWA_WINDOW_CORNER_PREFERENCE (inte dokumenterat här, men existerar)
                const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
                // 2 = DWM_WINDOW_CORNER_PREFERENCE::DWMNCRP_ROUND
                int round = 2;
                _ = DwmSetWindowAttribute(hwnd, DWMWA_WINDOW_CORNER_PREFERENCE, ref round, sizeof(int));
            }
            catch { /* ignore */ }
        }
    }
}

