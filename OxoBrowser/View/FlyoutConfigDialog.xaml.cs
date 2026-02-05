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
using System.Net;
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
        private bool _isThemeInitializing;

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



            _isThemeInitializing = true;
            var themeIndex = (int)AppConfig.Instance.ConfigData.Theme;
            if (themeIndex < 0 || themeIndex >= comboBoxTheme.Items.Count)
            {
                themeIndex = 0;
            }
            comboBoxTheme.SelectedIndex = themeIndex;
            _isThemeInitializing = false;

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

        private void ComboBoxTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isThemeInitializing)
            {
                return;
            }

            if (comboBoxTheme.SelectedIndex < 0)
            {
                return;
            }

            var selectedTheme = (CatppuccinTheme)comboBoxTheme.SelectedIndex;
            AppConfig.Instance.ConfigData.Theme = selectedTheme;
            ThemeService.Apply(selectedTheme);
        }

        private void ButtonUseSystemProxy_Click(object sender, RoutedEventArgs e)
        {
            buttonUseSystemProxy.IsEnabled = false;

            try
            {
                if (TryGetSystemProxy(out var proxy, out var error))
                {
                    if (TrySplitHostPort(proxy, out var host, out var port))
                    {
                        textConfigIP.Text = host;
                        textConfigPort.Text = port;

                        AppConfig.Instance.ConfigData.ProxyIP = host;
                        AppConfig.Instance.ConfigData.ProxyPort = port;
                        TitaniumWebProxy.UpdateUpStreamProxy();
                    }
                    else
                    {
                        EasyLog.Write($"系统代理格式不支持：{proxy}");
                    }
                }
                else
                {
                    EasyLog.Write($"未检测到可用的系统代理：{error}");
                }
            }
            catch (Exception ex)
            {
                EasyLog.Write(ex);
            }
            finally
            {
                buttonUseSystemProxy.IsEnabled = true;
            }
        }

        private static bool TryGetSystemProxy(out string proxy, out string error)
        {
            proxy = string.Empty;
            error = string.Empty;

            try
            {
                var systemProxy = WebRequest.GetSystemWebProxy();
                if (systemProxy == null)
                {
                    error = "系统代理为空";
                    return false;
                }

                Uri[] testUris = new[]
                {
                    new Uri("http://www.163.com/"),
                    new Uri("https://www.163.com/")
                };

                string lastError = string.Empty;

                foreach (var testUri in testUris)
                {
                    if (systemProxy.IsBypassed(testUri))
                    {
                        continue;
                    }

                    var proxyUri = systemProxy.GetProxy(testUri);
                    if (proxyUri == null || proxyUri == testUri)
                    {
                        continue;
                    }

                    if (!string.Equals(proxyUri.Scheme, Uri.UriSchemeHttp, StringComparison.OrdinalIgnoreCase)
                        && !string.Equals(proxyUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
                    {
                        lastError = $"系统代理类型不支持：{proxyUri.Scheme}";
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(proxyUri.Host) || proxyUri.Port <= 0)
                    {
                        continue;
                    }

                    proxy = $"{proxyUri.Host}:{proxyUri.Port}";
                    return true;
                }

                error = string.IsNullOrWhiteSpace(lastError)
                    ? "系统未启用代理或代理不适用于测试地址"
                    : lastError;
                return false;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        private static bool TrySplitHostPort(string proxy, out string host, out string port)
        {
            host = string.Empty;
            port = string.Empty;

            if (string.IsNullOrWhiteSpace(proxy))
            {
                return false;
            }

            var parts = proxy.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length != 2)
            {
                return false;
            }

            host = parts[0].Trim();
            port = parts[1].Trim();
            return !string.IsNullOrWhiteSpace(host) && !string.IsNullOrWhiteSpace(port);
        }
    }
}
