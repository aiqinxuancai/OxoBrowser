/*
 * Copyright (C) 2007 Eitan Pogrebizsky <openpandora@gmail.com>, 
 * and individual contributors.
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;

namespace OpenPandora
{
    public class User32
    {
        //
        // Imports
        //

        [DllImport("user32.dll")]
        public static extern bool ChangeWindowMessageFilter(uint message, uint dwFlag);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(int hwnd, int wMsg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32", EntryPoint = "FindWindowExA")]
        public static extern int FindWindowEx(int hwndParent, int hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        public static extern int GetWindowRect(int hwnd, ref RECT rc);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int SetWindowsHookEx(int idHook, HOOKPROC lpfn, int hMod, int dwThreadId);

        [DllImport("user32")]
        public static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern short GetKeyState(int vKey);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern int CallNextHookEx(int idHook, int nCode, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        public static extern int UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll")]
        public static extern int SetActiveWindow(int hwnd);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(int hwnd);

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(int hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool IsIconic(int hwnd);

        [DllImport("user32.dll")]
        public static extern int EnumChildWindows(int hwndParent, EnumChildProc lpEnumFunc, ref RECT lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(int hwnd, [MarshalAs(UnmanagedType.LPTStr)] string lpClassName, int capacity);

        [DllImport("user32", CharSet = CharSet.Auto)]
        public static extern int DrawAnimatedRects(int hwnd, int idAni, ref RECT lprcFrom, ref RECT lprcTo);

        [DllImport("User32", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(int uAction, int uParam, ref ANIMATIONINFO lpvParam, int fuWinIni);

        public const int SPI_GETANIMATION = 0x48;

        public const int IDANI_OPEN = 0x1;
        public const int IDANI_CLOSE = 0x2;
        public const int IDANI_CAPTION = 0x3;

        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_NORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_MAXIMIZE = 3;
        private const int SW_SHOWNOACTIVATE = 4;
        private const int SW_SHOW = 5;
        private const int SW_MINIMIZE = 6;
        private const int SW_SHOWMINNOACTIVE = 7;
        private const int SW_SHOWNA = 8;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWDEFAULT = 10;
        private const int SW_FORCEMINIMIZE = 11;
        private const int SW_MAX = 11;

        public const int WM_SETFOCUS = 0x0007;

        [DllImport("user32.dll")]
        private static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("user32.dll")]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MonitorInfo lpmi);

        private const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

        [StructLayout(LayoutKind.Sequential)]
        private struct MonitorInfo
        {
            public uint Size;
            public RectStruct Monitor;
            public RectStruct WorkArea;
            public uint Flags;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct RectStruct
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static Rect GetWindowScreenBounds(Window window)
        {
            var windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(window);
            IntPtr hMonitor = MonitorFromWindow(windowInteropHelper.Handle, MONITOR_DEFAULTTONEAREST);

            MonitorInfo monitorInfo = new MonitorInfo();
            monitorInfo.Size = (uint)Marshal.SizeOf(typeof(MonitorInfo));
            bool success = GetMonitorInfo(hMonitor, ref monitorInfo);
            if (!success)
            {
                throw new InvalidOperationException("Unable to get monitor info.");
            }

            return new Rect(
                monitorInfo.Monitor.Left,
                monitorInfo.Monitor.Top,
                monitorInfo.Monitor.Right - monitorInfo.Monitor.Left,
                monitorInfo.Monitor.Bottom - monitorInfo.Monitor.Top
            );
        }
        public static void SwitchToProcess(Process process)
        {
            if (IsIconic((int)process.MainWindowHandle))
            {
                ShowWindowAsync((int)process.MainWindowHandle, SW_RESTORE);
            }

            SetForegroundWindow((int)process.MainWindowHandle);
        }

        //
        // Delegates
        //

        public delegate int HOOKPROC(int nCode, int wParam, int lParam);
        public delegate bool EnumChildProc(int hwnd, ref RECT lParam);

        //
        // Structs
        //

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct ANIMATIONINFO
        {
            public int cbSize;
            public int iMinAnimate;
        }
    }
}