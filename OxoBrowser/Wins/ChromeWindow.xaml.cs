using Base;
using CefSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 513;
        private const int WM_LBUTTONUP = 514;

        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {

            if (msg == WM_LBUTTONDOWN)
            {
                //var a = ((UInt32)y << 16) | (UInt32)x;
                int x = (ushort)lParam.ToInt32();
                int y = (ushort)(lParam.ToInt32() >> 16) & 0xFFFF;


                chromeMain.GetBrowser().GetHost().SendMouseClickEvent(x, y, MouseButtonType.Left, false, 1, CefEventFlags.None);
                //System.Threading.Thread.Sleep(10);

                handled = true;
            }
            if (msg == WM_LBUTTONUP)
            {
                int x = (ushort)lParam.ToInt32();
                int y = (ushort)(lParam.ToInt32() >> 16) & 0xFFFF;

                //在这里添加鼠标移动的响应
                chromeMain.GetBrowser().GetHost().SendMouseClickEvent(x, y, MouseButtonType.Left, true, 1, CefEventFlags.None);
                handled = true;
            }
            return IntPtr.Zero;
        }

        public ChromeWindow()
        {
            InitializeComponent();
            thisWindow = this;


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
                // this is it 
                interop.Owner = new WindowInteropHelper(from).Handle;
            }
        }


        private void CEFWindow_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            HwndSource hs = HwndSource.FromHwnd(hwnd);
            hs.AddHook(new HwndSourceHook(WndProc));

            chromeMain = new CefSharp.Wpf.ChromiumWebBrowser();
            chromeMain.MaxHeight = 720;
            chromeMain.MaxWidth = 1200;
            chromeMain.MinHeight = 720;
            chromeMain.MinWidth = 1200;
            chromeMain.MouseUp += ChromeMain_MouseUp;
            this.Content = chromeMain;
            //chromeMain.SendMouseWheelEvent()
            //chromeMain.Address = "https://www.dmm.com/";

            chromeMain.FrameLoadEnd += ChromeMain_FrameLoadEnd;
            //chromeMain.Address = "http://html5test.com/";
            chromeMain.Address = "http://www.dmm.com/netgame/social/-/gadgets/=/app_id=854854/";
        }

        private void ChromeMain_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("ChromeMain_MouseUp");
        }

        private void ChromeMain_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            Debug.WriteLine("FrameLoadEnd " + e.Url);

            if (e.Frame.Name == "game_frame")
            {
                if (chromeMain.GetBrowser().HasDocument)
                {
                    WebViewConfig.GetKanColle2ndHtml5Core(chromeMain);
                }
            }
        }
    }
}
