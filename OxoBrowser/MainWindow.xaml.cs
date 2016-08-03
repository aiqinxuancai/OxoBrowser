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

namespace OxoBrowser
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
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
            InitUI();
            UpdataSoundButton();
            WebViewConfig.SetWebBrowserSilent(webMain, true);
            WebBrowserZoomInvoker.AddZoomInvoker(webMain);
            webMain.Navigate("http://www.dmm.com/netgame/social/-/gadgets/=/app_id=738496/"); //花骑士
            //webMain.Navigate("http://www.dmm.com/netgame/social/-/gadgets/=/app_id=825012/"); //刀剑
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

                //UpdataWindowSize(SizeNew);

                if (SizeNew != 100)
                {
                    //SetTimerWebSite();
                }

                System.Windows.Forms.Screen screen_ = System.Windows.Forms.Screen.PrimaryScreen;

                if (winMain.MinWidth > screen_.Bounds.Width | webMain.Height + 100 > screen_.Bounds.Height)
                {
                    MessageBox.Show("你设置的尺寸超出了屏幕的大小！还原回100%！", "ERROR");
                    //UpdataWindowSize(100);

                    if (SizeNew != 100)
                    {
                        //SetTimerWebSite();
                    }
                    winMain.Height = webMain.Height + 30;
                    winMain.Width = winMain.MinWidth;
                }
                else
                {
                    winMain.Height = webMain.Height + 30;
                    winMain.Width = winMain.MinWidth;
                }
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        //private void UpdataWindowSize(double _site)
        //{
        //    double old_height = 580;
        //    double old_width = 960;
        //    double height_min = 75;

        //    double __site = _site / 100; // + dpi_site
        //    Debug.WriteLine(__site);

        //    SetZoom(webMain, (int)_site);//+ dpi_site * 100

        //    webMain.Height = old_height * (__site);
        //    webMain.Width = old_width * (__site);

        //    winMain.MinHeight = webMain.Height + height_min;
        //    winMain.MinWidth = old_width * __site;

        //    if (_site != 100)
        //    {
        //        SetTimerWebSite();
        //    }
        //}

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
            WebViewConfig.ApplyStyleSheet((mshtml.HTMLDocument)webMain.Document);
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
