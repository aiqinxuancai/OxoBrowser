﻿using Base;
using CefSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace OxoBrowser.Wins
{
    /// <summary>
    /// ChromeWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ChromeWindow : Window
    {

        public CefSharp.Wpf.ChromiumWebBrowser chromeMain;

        public static ChromeWindow thisWindow ;

        private bool mouseFirstLButtonDown = false;
        private bool mouseFirstLButtonUp = false;

        private PointF dpiPointF;

        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 513;
        private const int WM_LBUTTONUP = 514;
        private const int WM_KEYDOWN = 256;
        private const int WM_KEYUP = 257;
        /// <summary>
        /// 实现wpf无法响应点击消息的问题
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_LBUTTONDOWN)
            {
                if (mouseFirstLButtonDown == false) //不处理第一次操作
                {
                    mouseFirstLButtonDown = true;
                    return IntPtr.Zero;
                }
                
                int x = (ushort)lParam.ToInt32();
                int y = (ushort)(lParam.ToInt32() >> 16) & 0xFFFF;
                chromeMain.GetBrowser().GetHost().SendMouseClickEvent(x / (int)dpiPointF.X, y / (int)dpiPointF.Y, MouseButtonType.Left, false, 1, CefEventFlags.None);
                handled = true;
            }
            if (msg == WM_LBUTTONUP) 
            {
                if (mouseFirstLButtonUp == false) //不处理第一次操作
                {
                    mouseFirstLButtonUp = true;
                    return IntPtr.Zero;
                }
                int x = (ushort)lParam.ToInt32();
                int y = (ushort)(lParam.ToInt32() >> 16) & 0xFFFF;
                chromeMain.GetBrowser().GetHost().SendMouseClickEvent(x / (int)dpiPointF.X, y / (int)dpiPointF.Y, MouseButtonType.Left, true, 1, CefEventFlags.None);
                handled = true;
            }
            if (msg == WM_KEYDOWN)
            {
                if ((int)System.Windows.Forms.Keys.F5 == wParam.ToInt32())
                {
                    handled = true;
                }
            }
            if (msg == WM_KEYUP)
            {
                if ((int)System.Windows.Forms.Keys.F5 == wParam.ToInt32())
                {
                    chromeMain.Reload(); 
                    handled = true;
                }
            }

            return IntPtr.Zero;
        }

        public ChromeWindow()
        {
            InitializeComponent();
            thisWindow = this;
            dpiPointF = WebBrowserZoomInvoker.GetCurrentDIPScale();
        }

        ~ChromeWindow()
        {
            thisWindow = null;
        }




        public static void Create(Window from)
        {
            if (thisWindow == null)
            {
                ChromeWindow winLog = new ChromeWindow();
                winLog.Owner = from;
                winLog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                winLog.Show();

                var interop = new WindowInteropHelper(winLog);
                interop.EnsureHandle();
                interop.Owner = new WindowInteropHelper(from).Handle;
            }
        }


        private void CEFWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            HwndSource hs = HwndSource.FromHwnd(hwnd);
            hs.AddHook(new HwndSourceHook(WndProc));

            chromeMain = new CefSharp.Wpf.ChromiumWebBrowser();
            chromeMain.MaxHeight = 576;
            chromeMain.MaxWidth = 1041;
            chromeMain.MinHeight = 576;
            chromeMain.MinWidth = 1041;
            chromeMain.MouseUp += ChromeMain_MouseUp;
            this.Content = chromeMain;

            // CORS
            chromeMain.BrowserSettings.WebSecurity = CefState.Disabled;
            chromeMain.BrowserSettings.FileAccessFromFileUrls = CefState.Enabled;
            chromeMain.BrowserSettings.UniversalAccessFromFileUrls = CefState.Enabled;

            chromeMain.FrameLoadEnd += ChromeMain_FrameLoadEnd;
            chromeMain.LoadingStateChanged += ChromeMain_LoadingStateChanged;
            chromeMain.Address = "http://pc-play.games.dmm.com/play/namuami_utena/";
            chromeMain.Focus();
        }

        private void ChromeMain_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            
        }

        private void ChromeMain_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("ChromeMain_MouseUp");
        }

        private void ChromeMain_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            Debug.WriteLine("FrameLoadEnd " + e.Url);

            if (e.Frame.Url.Contains("/play/namuami_utena"))
            {
                if (chromeMain.GetBrowser().HasDocument)
                {
                    WebViewConfig.GetBungo(chromeMain);
                }
            }
        }
    }
}
