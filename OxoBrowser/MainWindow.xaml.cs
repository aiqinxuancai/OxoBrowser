

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
using OxoBrowser.Services;
using System.Windows;
using System.Windows.Interop;
using OpenPandora;


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
            comboBoxGameType.SelectedIndex = (int)AppConfig.Instance.ConfigData.GameType;
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

        private void winMain_LocationChanged(object sender, EventArgs e)
        {
            //ResetWindowSize();
            if (ChromeWindow.Instance != null)
            {
                Window window = Window.GetWindow(this);

                ChromeWindow.Instance.Left = this.Left + 2;
                ChromeWindow.Instance.Top = this.Top + Bar.ActualHeight;

                //Debug.WriteLine($"ChromeWindowSynchronize {ChromeWindow.Singleton.Left }, {ChromeWindow.Singleton.Top}");
            }
        }


        private void btnTitelReload_Click(object sender, RoutedEventArgs e)
        {
            ChromeWindow.Instance.chromeMain.GetBrowser().Reload();
        }

        private void comboBoxGameSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var selectedSize = comboBox.SelectedItem as ComboBoxItem;
            string size = selectedSize.Content.ToString();
            OnWebSelectionChanged(size);
        }

        public void OnWebSelectionChanged(string webSizeStr)
        {
            if (ChromeWindow.Instance == null)
            {
                return;
            }
            try
            {
                int webSize = int.Parse(webSizeStr.Replace("%", ""));
                Debug.WriteLine(webSize);
      
                int oldBottomHeight = (int)this.Bar.ActualHeight;

                SetWinSite(webSize);

                //System.Windows.Forms.Screen screen_ = System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(winMain).Handle);

                Rect screenBounds = User32.GetWindowScreenBounds(winMain);

                if (winMain.MinWidth > screenBounds.Width | ChromeWindow.Instance.Height + 100 > screenBounds.Height)
                {
                    //MessageBox.Show("你设置的尺寸超出了屏幕的大小！还原回100%！", "ERROR");
                    SetWinSite(100);
                    winMain.Height = ChromeWindow.Instance.Height + oldBottomHeight;
                    winMain.Width = winMain.MinWidth;
                }
                else
                {
                    winMain.Height = ChromeWindow.Instance.Height + oldBottomHeight;
                    winMain.Width = winMain.MinWidth;
                }
            }
            catch (Exception ex)
            {
                EasyLog.Write(ex, LogLevel.Error);
            }
        }

        private void SetWinSite(double site)
        {
            if (ChromeWindow.Instance == null)
            {
                return;
            }

            double oldHeight = AppConfig.Instance.ConfigData.SizeWithGameType().Height;
            double oldWidth = AppConfig.Instance.ConfigData.SizeWithGameType().Width;
            double heightMin = Bar.ActualHeight;

            double siteDouble = site / 100; // + dpi_site
            Debug.WriteLine(siteDouble);

            ChromeWindow.Instance.ApplyZoom(siteDouble);


            Size gridGameArea = new Size();

            gridGameArea.Height = oldHeight * (siteDouble);
            gridGameArea.Width = oldWidth * (siteDouble);


            ChromeWindow.Instance.MinHeight = gridGameArea.Height;
            ChromeWindow.Instance.MinWidth = gridGameArea.Width;

            ChromeWindow.Instance.chromeMain.MinHeight = gridGameArea.Height;
            ChromeWindow.Instance.chromeMain.MinWidth = gridGameArea.Width;


            ChromeWindow.Instance.Height = gridGameArea.Height;
            ChromeWindow.Instance.Width = gridGameArea.Width;

            ChromeWindow.Instance.chromeMain.Width = gridGameArea.Width;
            ChromeWindow.Instance.chromeMain.Height = gridGameArea.Height;


            Debug.WriteLine("Chrome位置:{0},{1} {2}x{3}",
                ChromeWindow.Instance.chromeMain.RenderSize.Width,
                ChromeWindow.Instance.chromeMain.RenderSize.Height,
                ChromeWindow.Instance.chromeMain.Width,
                ChromeWindow.Instance.chromeMain.Height
                );



            this.MinHeight = ChromeWindow.Instance.Height + heightMin + 4;
            this.MinWidth = oldWidth * siteDouble + 4;

        }

        public void ResetWindowSize()
        {
            //根据Size来计算当前窗口的高度
            if (ChromeWindow.Instance != null)
            {
                ChromeWindow.Instance.Left = this.Left + 2;
                ChromeWindow.Instance.Top = this.Top + Bar.ActualHeight;


                var size = AppConfig.Instance.ConfigData.SizeWithGameType();

                ChromeWindow.Instance.Width = size.Width;
                ChromeWindow.Instance.Height = size.Height;

                this.Width = ChromeWindow.Instance.Width + 2 * 2;
                this.Height = Bar.ActualHeight + 40 + ChromeWindow.Instance.Height + 4;
            }



        }

        private void btnGoFast_Click(object sender, RoutedEventArgs e)
        {
            //btnGoFast.ContextMenu.IsOpen = true;

            ChromeWindow.Instance.chromeMain.Address = AppConfig.Instance.ConfigData.UrlWithGameType();

        }

        private void KanColleUrl_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Touken_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void comboBoxGameType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AppConfig.Instance.ConfigData.GameType = (GameTypeEnum)comboBoxGameType.SelectedIndex;
            if (ChromeWindow.Instance == null)
            {
                return;
            }
            ChromeWindow.Instance.chromeMain.Address = AppConfig.Instance.ConfigData.UrlWithGameType();
            ResetWindowSize();
        }
    }
}
