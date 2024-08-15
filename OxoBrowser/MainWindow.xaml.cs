

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
using System.Reflection.Metadata;
using Wpf.Ui.Controls;
using Wpf.Ui;
using Wpf.Ui.Extensions;
using OxoBrowser.View;

namespace OxoBrowser
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
        public static MainWindow Instance { set; get; }

        private static readonly bool DebuggingSubProcess = Debugger.IsAttached;


        public CefSharp.Wpf.ChromiumWebBrowser chromeMain;

        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
        }


        private void winMain_Loaded(object sender, RoutedEventArgs e)
        {
            InitUI();
            UpdataSoundButton();
            ChromeWindow.Create(this);
            ResetWindowSize();
        }

        /// <summary>
        /// 初始化UI上的配置文本显示
        /// </summary>
        private void InitUI()
        {
            //this.flyoutConfig.textConfigIP.Text = AppConfig.Instance.ConfigData.ProxyIP;
            //this.flyoutConfig.textConfigPort.Text = AppConfig.Instance.ConfigData.ProxyPort;
        }


        /// <summary>
        /// 是否将Web显示为图片 
        /// </summary>
        /// <param name="_show"></param>
        public void ShowWebImage(bool show)
        {
            if (show)
            {
                imageWebMain.Source = Screenshot.BrowserSnapShot(ChromeWindow.Instance);
                ChromeWindow.Instance.Visibility = Visibility.Hidden;
            }
            else
            {
                imageWebMain.Source = null;
                ChromeWindow.Instance.Visibility = Visibility.Visible;
            }

        }

        /// <summary>
        /// 用于刷新显示声音状态
        /// </summary>
        /// <param name="_soundClose"></param>
        private void UpdataSoundButton()
        {
            if (AppConfig.Instance.ConfigData.SoundClose == false)
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
        private async void btnTitelConfig_Click(object sender, RoutedEventArgs e)
        {
            if(imageWebMain.Source == null)
            {


                
                //ToggleFlyout(0);
                ShowWebImage(true);
                ShowSettingsView();



            }
            else
            {
                ShowWebImage(false);
            }
        }

        private async void ShowSettingsView()
        {
            IContentDialogService service = new ContentDialogService();
            service.SetContentPresenter(DialogPresenter);
            //ContentDialogResult result = await service.ShowSimpleDialogAsync(
            //    new SimpleContentDialogCreateOptions()
            //    {
            //        Title = "设置",
            //        Content = new FlyoutConfig(),
            //        PrimaryButtonText = "保存",

            //        CloseButtonText = "取消",
            //    }
            //);


            var termsOfUseContentDialog = new FlyoutConfigDialog(service.GetContentPresenter());
            _ = await termsOfUseContentDialog.ShowAsync();
        }



        //public void ToggleFlyout(int index)
        //{
        //    var flyout = this.flyouts.Items[index] as Flyout;
        //    if (flyout == null)
        //    {
        //        return;
        //    }
        //    flyout.IsOpen = !flyout.IsOpen;
        //}

        private void settingsFlyout_ClosingFinished(object sender, RoutedEventArgs e)
        {
            ShowWebImage(false);

        }

        private void btnTitelSound_Click(object sender, RoutedEventArgs e)
        {
            AppConfig.Instance.ConfigData.SoundClose = true;
            UpdataSoundButton();
        }

        private void btnTitelSoundClose_Click(object sender, RoutedEventArgs e)
        {
            AppConfig.Instance.ConfigData.SoundClose = false;
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
            ResetWindowSize();
        }


        private void btnTitelReload_Click(object sender, RoutedEventArgs e)
        {
            ChromeWindow.Instance.chromeMain.GetBrowser().Reload();
        }

        private void menuGameSize_Click(object sender, RoutedEventArgs e)
        {
            
        }

        public void ResetWindowSize()
        {
            //根据Size来计算当前窗口的高度
            if (ChromeWindow.Instance != null)
            {
                ChromeWindow.Instance.Left = this.Left + 2;
                ChromeWindow.Instance.Top = this.Top + Bar.ActualHeight + 32;


                var size = AppConfig.Instance.ConfigData.SizeWithGameType();

                ChromeWindow.Instance.Width = size.Width;
                ChromeWindow.Instance.Height = size.Height;

                this.Width = ChromeWindow.Instance.Width + 2 * 2;
                this.Height = Bar.ActualHeight + 32 + ChromeWindow.Instance.Height + 4;
            }



        }

    }
}
