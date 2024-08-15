using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Base;
using OxoBrowser.Services;
using Wpf.Ui.Controls;

namespace OxoBrowser.View
{
    /// <summary>
    /// FlyoutSettings.xaml 的交互逻辑
    /// </summary>
    public partial class FlyoutConfigDialog : ContentDialog
    {

        public FlyoutConfigDialog(ContentPresenter contentPresenter)
        : base(contentPresenter)
        {
            InitializeComponent();

            //载入数据
            SetupView();
        }

        private void SetupView()
        {
            textConfigIP.Text = AppConfig.Instance.ConfigData.ProxyIP;
            textConfigPort.Text = AppConfig.Instance.ConfigData.ProxyPort;



            switch (AppConfig.Instance.ConfigData.GameType)
            {
                case GameTypeEnum.KanColle:
                    KanColleRadio.IsChecked = true;
                    break;
                case GameTypeEnum.Touken:
                    ToukenRadio.IsChecked = true;
                    break;
            }

        }

        protected override void OnButtonClick(ContentDialogButton button)
        {
            if (button == ContentDialogButton.Primary)
            {
                if (textConfigIP.Text != AppConfig.Instance.ConfigData.ProxyIP || textConfigPort.Text != AppConfig.Instance.ConfigData.ProxyPort)
                {
                    AppConfig.Instance.ConfigData.ProxyIP = textConfigIP.Text;
                    AppConfig.Instance.ConfigData.ProxyPort = textConfigPort.Text;
                    TitaniumWebProxy.UpdateUpStreamProxy();
                }
            }

            if ((bool)KanColleRadio.IsChecked)
            {
                AppConfig.Instance.ConfigData.GameType = GameTypeEnum.KanColle;
            } 
            else if ((bool)ToukenRadio.IsChecked)
            {
                AppConfig.Instance.ConfigData.GameType = GameTypeEnum.Touken;
            }

            MainWindow.Instance.ShowWebImage(false);
            MainWindow.Instance.ResetWindowSize();
            base.OnButtonClick(button);
        }
    }
}