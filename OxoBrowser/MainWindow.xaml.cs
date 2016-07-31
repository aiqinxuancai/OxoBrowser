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

namespace OxoBrowser
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    [Export(typeof(IShell))]
    public partial class MainWindow : MetroWindow
    {
        public static MainWindow thisFrm;

        public MainWindow()
        {
            thisFrm = this;
            InitializeComponent();

        }

        private void winMain_Loaded(object sender, RoutedEventArgs e)
        {
            WebViewConfig.SetWebBrowserSilent(webMain, true);
            webMain.Navigate("http://www.dmm.com/");
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

        private void ToggleFlyout(int index)
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

        }

        private void webMain_Navigated(object sender, NavigationEventArgs e)
        {
            WebViewConfig.ApplyStyleSheet((mshtml.HTMLDocument) webMain.Document);
        }

        private void btnTitelFlashMin_Click(object sender, RoutedEventArgs e)
        {
            WebViewConfig.ApplyStyleSheet((mshtml.HTMLDocument)webMain.Document);
        }
    }
}
