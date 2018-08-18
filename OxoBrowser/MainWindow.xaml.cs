using MahApps.Metro.Controls;
using OxoBrowser.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Base;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Reflection;
using System.IO;
using CefSharp;
using System.Windows.Interop;

namespace OxoBrowser
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public static MainWindow thisFrm;

        private static readonly bool DebuggingSubProcess = Debugger.IsAttached;


        public CefSharp.Wpf.ChromiumWebBrowser chromeMain;

        public MainWindow()
        {
            thisFrm = this;
            InitializeComponent();

        }

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
            return (new IntPtr(0));
        }

        private void winMain_Loaded(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            HwndSource hs = HwndSource.FromHwnd(hwnd);
            hs.AddHook(new HwndSourceHook(WndProc));

            InitUI();
            UpdataSoundButton();
            WebViewConfig.SetWebBrowserSilent(webMain, true);
            WebBrowserZoomInvoker.AddZoomInvoker(webMain);
            //webMain.Navigate("http://www.dmm.com/netgame/social/-/gadgets/=/app_id=486104/"); //花骑士
            //webMain.Navigate("http://www.dmm.com/netgame/social/-/gadgets/=/app_id=825012/"); //刀剑
            //webMain.Navigate("https://www.whatismybrowser.com/"); //花骑士
            //Cef.Shutdown();

            webMain.Visibility = Visibility.Hidden;
            
            var setting = new CefSharp.CefSettings()
            {
                CachePath = Directory.GetCurrentDirectory() + @"\Cache",
            };

            //setting.RemoteDebuggingPort = 8088;
            setting.Locale = "zh-CN";
            //setting.UserAgent = "Mozilla/6.0 (Windows NT 6.2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2228.0 Safari/537.36";
            setting.CefCommandLineArgs.Add("enable-npapi", "1");
            setting.CefCommandLineArgs.Add("--proxy-server", "http://127.0.0.1:37161");
            //setting.CefCommandLineArgs.Add("--no-proxy-server", "1");

            setting.CefCommandLineArgs.Add("--enable-media-stream", "1");
            //setting.CefCommandLineArgs.Add("disable-gpu", "0");
            //setting.
            setting.SetOffScreenRenderingBestPerformanceArgs();

            //setting.CefCommandLineArgs.Add("disable-gpu", "1");
            //setting.CefCommandLineArgs.Add("disable-gpu-compositing", "1");
            //setting.CefCommandLineArgs.Add("enable-begin-frame-scheduling", "1");


            setting.CefCommandLineArgs.Add("enable-media-stream", "1");

            setting.CefCommandLineArgs["enable-system-flash"] = "0";
            //setting.CefCommandLineArgs.Add("enable-system-flash", "0"); //Automatically discovered and load a system-wide installation of Pepper Flash.
            setting.CefCommandLineArgs.Add("ppapi-flash-path", @".\plugins\pepflashplayer64_23_0_0_162.dll"); //Load a specific pepper flash version (Step 1 of 2)
            setting.CefCommandLineArgs.Add("ppapi-flash-version", "23.0.0.162"); //Load a specific pepper flash version (Step 2 of 2)


            //CefSharp.Cef.Initialize(setting);

            if (!Cef.Initialize(setting))
            {
                throw new Exception("Unable to Initialize Cef");
            }

            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

            
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
                    GetKanColle2ndHtml5Core();
                }
            }
        }

        private void InitUI()
        {
            this.flyoutConfig.textConfigIP.Text = AppConfig.m_config.ProxyIP;
            this.flyoutConfig.textConfigPort.Text = AppConfig.m_config.ProxyPort;
        }


        /// <summary>
        /// 是否将Web显示为图片 
        /// </summary>
        /// <param name="_show"></param>
        private void ShowWebImage(bool _show)
        {
            if (_show)
            {
                imageWebMain.Source = WebScreenshot.BrowserSnapShot(webMain);
                webMain.Visibility = Visibility.Hidden;
            }
            else
            {
                imageWebMain.Source = null;
                webMain.Visibility = Visibility.Visible;
            }

        }

        /// <summary>
        /// 用于刷新显示声音状态
        /// </summary>
        /// <param name="_soundClose"></param>
        private void UpdataSoundButton()
        {
            if (AppConfig.m_config.SoundClose == false)
            {
                btnTitelSound.Visibility = Visibility.Visible;
                btnTitelSoundClose.Visibility = Visibility.Collapsed;
                SoundSetting.SoundClose(false);
            }
            else
            {
                btnTitelSound.Visibility = Visibility.Collapsed;
                btnTitelSoundClose.Visibility = Visibility.Visible;
                SoundSetting.SoundClose(true);
            }

        }
        /// <summary>
        /// 顶部设置点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTitelConfig_Click(object sender, RoutedEventArgs e)
        {
            if(imageWebMain.Source == null)
            {
                ToggleFlyout(0);
                ShowWebImage(true);
            }
            else
            {
                ToggleFlyout(0);
            }
        }

        /// <summary>
        /// 游戏窗口缩放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuGameSize_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            try
            {
                string sizeString = item.Header.ToString();
                AppConfig.m_config.WebSize = sizeString;
                AppConfig.Save();
                this.btnTitelWindowSize.Content = sizeString;
                int SizeNew = int.Parse(sizeString.Replace("%", ""));

                UpdataWindowSize(SizeNew);
                System.Windows.Forms.Screen screenSize = System.Windows.Forms.Screen.FromHandle(webMain.Handle);
                if (winMain.MinWidth > screenSize.Bounds.Width | webMain.Height + 100 > screenSize.Bounds.Height)
                {
                    MessageBox.Show("你设置的尺寸超出了屏幕的大小！还原回100%！", "ERROR");
                    if (SizeNew != 100)
                    {
                        UpdataWindowSize(100);
                    }
                    winMain.Height = webMain.Height ; //+30
                    winMain.Width = winMain.MinWidth;
                }
                else
                {
                    winMain.Height = webMain.Height ; //+30
                    winMain.Width = winMain.MinWidth;
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }


        static readonly int OLECMDEXECOPT_DODEFAULT = 0;
        static readonly int OLECMDID_OPTICAL_ZOOM = 63;

        //static void SetZoom(WebBrowser webbrowser, int zoom)
        //{
        //    try
        //    {
        //        System.Drawing.PointF scaleUI = WebBrowserZoomInvoker.GetCurrentDIPScale();
        //        if (100 != (int)(scaleUI.X * 100))
        //        {
        //            zoom = (int)(scaleUI.X * scaleUI.Y * zoom);
        //        }
        //        if (null == webbrowser)
        //        {
        //            return;
        //        }

        //        FieldInfo fiComWebBrowser = webbrowser.GetType().GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
        //        if (null != fiComWebBrowser)
        //        {
        //            Object objComWebBrowser = fiComWebBrowser.GetValue(webbrowser);

        //            if (null != objComWebBrowser)
        //            {
        //                object[] args = new object[]
        //                    {
        //                    OLECMDID_OPTICAL_ZOOM,
        //                    OLECMDEXECOPT_DODEFAULT,
        //                    zoom,
        //                    IntPtr.Zero
        //                    };
        //                objComWebBrowser.GetType().InvokeMember(
        //                "ExecWB",
        //                BindingFlags.InvokeMethod,
        //                null, objComWebBrowser,
        //                args);
        //            }
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //    }
        //}

        private void UpdataWindowSize(double _site)
        {
            double __site = _site / 100; // + dpi_site
            Debug.WriteLine(__site);

            //SetZoom(webMain, (int)_site);//+ dpi_site * 100

            
            double h = Convert.ToDouble(AppConfig.m_config.FlashHeight);
            double w = Convert.ToDouble(AppConfig.m_config.FlashWidth);
            webMain.Height = h * (__site);
            webMain.Width = w * (__site);

            winMain.MinHeight = webMain.Height;
            winMain.MinWidth = webMain.Width;

            if (_site != 100)
            {
                //SetTimerWebSite();
            }
        }

        public void ToggleFlyout(int index)
        {
            var flyout = this.flyouts.Items[index] as Flyout;
            if (flyout == null)
            {
                return;
            }
            flyout.IsOpen = !flyout.IsOpen;
        }

        private void settingsFlyout_ClosingFinished(object sender, RoutedEventArgs e)
        {
            ShowWebImage(false);

        }

        private void btnTitelSound_Click(object sender, RoutedEventArgs e)
        {
            AppConfig.m_config.SoundClose = true;
            AppConfig.Save();
            UpdataSoundButton();
        }
        private void btnTitelSoundClose_Click(object sender, RoutedEventArgs e)
        {
            AppConfig.m_config.SoundClose = false;
            AppConfig.Save();
            UpdataSoundButton();
        }

        private void webMain_Navigated(object sender, NavigationEventArgs e)
        {
            WebViewConfig.ApplyStyleSheet((mshtml.HTMLDocument) webMain.Document);
        }

        private void btnTitelFlashMin_Click(object sender, RoutedEventArgs e)
        {

            GetKanColle2ndHtml5Core();
        }


        private void GetKanColle2ndHtml5Core()
        {

            chromeMain.ExecuteScriptAsync("var node = document.createElement('style'); " +
                "node.innerHTML = 'html, body, iframe {overflow:hidden;margin:0;}'; " +
                "document.body.appendChild(node);");

            chromeMain.ExecuteScriptAsync("var node = document.createElement('style'); " +
                "node.innerHTML = 'game_frame {position:fixed; left:50%; top:0px; margin-left:-480px; z-index:1;}'; " +
                "document.body.appendChild(node);");

            chromeMain.ExecuteScriptAsync("var node = document.createElement('style'); " +
                "node.innerHTML = 'ul.area-menu {display: none;}'; " +
                "document.body.appendChild(node);");
            chromeMain.ExecuteScriptAsync("var node = document.createElement('style'); " +
                "node.innerHTML = '.dmm-ntgnavi {display: none;}'; " +
                "document.body.appendChild(node);");

            var game_frame = chromeMain.GetBrowser().GetFrame("game_frame");
            //game_frame.ExecuteJavaScriptAsync("if(document.getElementById('spacing_top')) {alert(document.getElementById('spacing_top').height);}");
            game_frame.ExecuteJavaScriptAsync("document.getElementById('spacing_top').style.height = '0px'");
            //chromeMain.ExecuteScriptAsync("alert(window.document.getElementById('spacing_top').innerHTML);");

            //game_frame.ExecuteJavaScriptAsync("var node = document.createElement('style'); " +
            //                                    "node.innerHTML = 'spacing_top {height: 0px;}'; " +
            //                                    "document.getElementById('spacing_top').appendChild(node);");

            //chromeMain.ExecuteScriptAsync("document.getElementById('spacing_top').height=0;");
            //chromeMain.ExecuteScriptAsync("var node = document.createElement('style'); " +
            //                                    "node.innerHTML = 'spacing_top {height: 0px;}'; " +
            //                                    "document.body.appendChild(node);");
        }


        private void webMain_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            Debug.WriteLine(e.Uri.ToString());
        }

        private void btnTitelWindowTop_Checked(object sender, RoutedEventArgs e)
        {
            winMain.Topmost = true;
        }

        private void btnTitelWindowTop_Unchecked(object sender, RoutedEventArgs e)
        {
            winMain.Topmost = false;
        }

        private void btnTitelWindowSize_Click(object sender, RoutedEventArgs e)
        {
            this.menuGameSize.PlacementTarget = this.btnTitelWindowSize;
            this.menuGameSize.Placement = PlacementMode.Bottom;
            this.menuGameSize.IsOpen = true;
        }


    }
}
