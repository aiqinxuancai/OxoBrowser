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


        public ChromeWindow()
        {
            InitializeComponent();
        }

        private void CEFWindow_Loaded(object sender, RoutedEventArgs e)
        {
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
                    WebViewConfig.GetKanColle2ndHtml5Core();
                }
            }
        }
    }
}
