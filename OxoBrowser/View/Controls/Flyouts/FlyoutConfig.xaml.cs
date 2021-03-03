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

namespace OxoBrowser.Controls.Flyouts
{
    /// <summary>
    /// FlyoutSettings.xaml 的交互逻辑
    /// </summary>
    public partial class FlyoutConfig : UserControl
    {
        public FlyoutConfig()
        {
            InitializeComponent();
        }

        //应用
        private void btnApply_Click(object sender, RoutedEventArgs e)
        {
            if (textConfigIP.Text != AppConfig.m_config.ProxyIP || textConfigPort.Text != AppConfig.m_config.ProxyPort)
            {
                AppConfig.m_config.ProxyIP = textConfigIP.Text;
                AppConfig.m_config.ProxyPort = textConfigPort.Text;
                AppConfig.Save();
                TitaniumWebProxy.UpdateUpStreamProxy();
            }
            //应用后关闭
            MainWindow.thisFrm.ToggleFlyout(0); //关闭Flyout
        }

    }
}