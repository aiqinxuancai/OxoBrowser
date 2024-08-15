using Base;
using CefSharp;
using OxoBrowser.Services;
using OxoBrowser.Utils;
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

        public static ChromeWindow Instance ;

        private bool mouseFirstLButtonDown = false;
        private bool mouseFirstLButtonUp = false;

        private PointF dpiPointF;

        private const int WM_MOUSEMOVE = 0x200;
        private const int WM_LBUTTONDOWN = 513;
        private const int WM_LBUTTONUP = 514;
        private const int WM_KEYDOWN = 256;
        private const int WM_KEYUP = 257;
        private const int WM_USER = 1024;
        private const int WM_MOUSEWHEEL = 0x020A;

        private const int TKB_CHECK_HOOK = 51301;
        private const int TKB_CHROME_INPUT_FIX = 51302;

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
                //if (AppConfig.Instance.ConfigData.OpenChromeFixInput)
                //{
                //    return IntPtr.Zero;
                //}
                if (mouseFirstLButtonDown == false) //不处理第一次操作
                {
                    EasyLog.Write("WndProc:hookFirstMessage");
                    mouseFirstLButtonDown = true;
                    return IntPtr.Zero;
                }

                int x = (ushort)lParam.ToInt32();
                int y = (ushort)(lParam.ToInt32() >> 16) & 0xFFFF;
                int trueX = x;
                int trueY = y;

                if (DpiAwareness.processAwareness == DpiAwareness.PROCESS_DPI_AWARENESS.Process_DPI_Unaware)
                {
                    trueX = x;
                    trueY = y;
                }
                else
                {
                    trueX = (int)(x / dpiPointF.X);
                    trueY = (int)(y / dpiPointF.Y);
                }

                OpenPandora.User32.SendMessage(hwnd.ToInt32(), OpenPandora.User32.WM_SETFOCUS, 0, 0);
                FocusManager.SetIsFocusScope(chromeMain, true);
                chromeMain.GetBrowser().GetHost().SendMouseMoveEvent(new MouseEvent(trueX, trueY, CefEventFlags.None), false);
                chromeMain.GetBrowser().GetHost().SendMouseClickEvent(trueX, trueY, MouseButtonType.Left, false, 1, CefEventFlags.None);

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
                int trueX = x;
                int trueY = y;

                if (DpiAwareness.processAwareness == DpiAwareness.PROCESS_DPI_AWARENESS.Process_DPI_Unaware)
                {
                    trueX = x;
                    trueY = y;
                }
                else
                {
                    trueX = (int)(x / dpiPointF.X);
                    trueY = (int)(y / dpiPointF.Y);
                }
                OpenPandora.User32.SendMessage(hwnd.ToInt32(), OpenPandora.User32.WM_SETFOCUS, 0, 0);
                FocusManager.SetIsFocusScope(chromeMain, true);
                chromeMain.GetBrowser().GetHost().SendMouseClickEvent(trueX, trueY, MouseButtonType.Left, true, 1, CefEventFlags.None);
                handled = true;
                Debug.WriteLine($"消息点击：{x},{y} -> {trueX},{trueY}");
            }
            if (msg == WM_KEYDOWN)
            {
                //116 = F5
                if (116 == wParam.ToInt32())
                {
                    handled = true;
                }
            }
            if (msg == WM_KEYUP)
            {
                if (116 == wParam.ToInt32())
                {
                    chromeMain.Reload();
                    handled = true;
                }
            }
            if (msg == WM_MOUSEWHEEL)
            {
                short delta = -120;
                try
                {
                    // 转换为 Int64
                    long wParamLong = wParam.ToInt64();

                    // 提取 delta 值
                    delta = (short)(wParamLong >> 16);
                }
                catch (Exception ex)
                {
                }

                try
                {
                    int x = (ushort)lParam.ToInt32();
                    int y = (ushort)(lParam.ToInt32() >> 16) & 0xFFFF;
                    // 获取鼠标的位置
                    int trueX = x;
                    int trueY = y;

                    if (DpiAwareness.processAwareness == DpiAwareness.PROCESS_DPI_AWARENESS.Process_DPI_Unaware)
                    {
                        trueX = x;
                        trueY = y;
                    }
                    else
                    {
                        trueX = (int)(x / dpiPointF.X);
                        trueY = (int)(y / dpiPointF.Y);
                    }

                    // 发送鼠标滚轮滚动消息
                    //host.SendMouseWheelEvent(mouseEvent, 0, delta);

                    chromeMain.GetBrowser().GetHost().SendMouseWheelEvent(new MouseEvent(trueX, trueY, CefEventFlags.None), 0, (int)delta);
                    handled = true;
                }
                catch (Exception ex)
                {


                }

            }
            if (msg == TKB_CHECK_HOOK) // 检测子类化是否安装
            {
                EasyLog.Write("RecvMessage:TKB_CHECK_HOOK");
                handled = true;
                return new IntPtr(1);
            }
            if (msg == TKB_CHROME_INPUT_FIX) // 检测子类化是否安装
            {
                EasyLog.Write("RecvMessage:TKB_CHECK_HOOK");

                //AppConfig.Instance.ConfigData.OpenChromeFixInput = wParam.ToInt32() == 1;
                //GlobalNotification.Default.Post(NotificationType.kSettingChange, null);
                handled = true;
                return new IntPtr(1);
            }

            return IntPtr.Zero;
        }


        public ChromeWindow()
        {
            InitializeComponent();
            Instance = this;
            dpiPointF = WebBrowserZoomInvoker.GetCurrentDIPScale();
        }

        ~ChromeWindow()
        {
            Instance = null;
        }


        public static void Create(Window from)
        {
            if (Instance == null)
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
            chromeMain.MaxHeight = 720;
            chromeMain.MaxWidth = 1200;
            chromeMain.MinHeight = 720;
            chromeMain.MinWidth = 1200;
            chromeMain.MouseUp += ChromeMain_MouseUp;
            this.Content = chromeMain;
            chromeMain.FrameLoadEnd += ChromeMain_FrameLoadEnd;
            chromeMain.LoadingStateChanged += ChromeMain_LoadingStateChanged;
            chromeMain.RequestHandler = new OxoRequestHandler();
            chromeMain.Address = "https://www.dmm.com/";
            //chromeMain.Address = "http://pc-play.games.dmm.com/play/bungo/";
            //chromeMain.Address = "http://html5test.com/";
            //chromeMain.Address = "http://www.dmm.com/netgame/social/-/gadgets/=/app_id=854854/";
            //chromeMain.Address = "https://apis.baidu.com/store/aladdin/land?cardType=ipSearch";
        

            chromeMain.Focus();
        }

        private void ChromeMain_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if (e.IsLoading)
                return;

            this.Dispatcher.Invoke((Action)(() =>
            {
                WebViewConfig.GetKanColle2ndHtml5Core(chromeMain);
            }));
        }


        
        private void ChromeMain_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("ChromeMain_MouseUp");
        }

        private void ChromeMain_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            Debug.WriteLine("FrameLoadEnd "  + e.Frame.Name + " " + e.Url);
        }
    }
}
