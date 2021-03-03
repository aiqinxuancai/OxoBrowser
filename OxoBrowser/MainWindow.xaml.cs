using MahApps.Metro.Controls;
using OxoBrowser.Controls;
using System;
using System.Collections.Generic;
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
using OxoBrowser.Wins;
using CefSharp.Wpf;

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


        private void winMain_Loaded(object sender, RoutedEventArgs e)
        {
            InitUI();
            UpdataSoundButton();

            //webMain.Navigate("http://www.dmm.com/netgame/social/-/gadgets/=/app_id=486104/"); //花骑士
            //webMain.Navigate("http://www.dmm.com/netgame/social/-/gadgets/=/app_id=825012/"); //刀剑
            //webMain.Navigate("https://www.whatismybrowser.com/"); //花骑士


            ChromeWindow.Create(this);

            ChromeWindowSynchronize();

        }

        /// <summary>
        /// 初始化UI上的配置文本显示
        /// </summary>
        private void InitUI()
        {
            this.flyoutConfig.textConfigIP.Text = AppConfig.m_config.ProxyIP;
            this.flyoutConfig.textConfigPort.Text = AppConfig.m_config.ProxyPort;
        }


        /// <summary>
        /// 是否将Web显示为图片 
        /// </summary>
        /// <param name="_show"></param>
        private void ShowWebImage(bool show)
        {
            if (show)
            {
                imageWebMain.Source = Screenshot.BrowserSnapShot(ChromeWindow.thisWindow);
                ChromeWindow.thisWindow.Visibility = Visibility.Hidden;
            }
            else
            {
                imageWebMain.Source = null;
                ChromeWindow.thisWindow.Visibility = Visibility.Visible;
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
        /// 顶部设置点击展开
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

        private void btnTitelFlashMin_Click(object sender, RoutedEventArgs e)
        {
            //GetKanColle2ndHtml5Core();
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

        private void winMain_LocationChanged(object sender, EventArgs e)
        {
            ChromeWindowSynchronize();
        }

        private void ChromeWindowSynchronize()
        {
            if (ChromeWindow.thisWindow != null)
            {
                ChromeWindow.thisWindow.Left = this.Left;
                ChromeWindow.thisWindow.Top = this.Top + this.TitleBarHeight;
            }
        }

        private void btnTitelReload_Click(object sender, RoutedEventArgs e)
        {
            ChromeWindow.thisWindow.chromeMain.GetBrowser().Reload();
        }

        private void menuGameSize_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
