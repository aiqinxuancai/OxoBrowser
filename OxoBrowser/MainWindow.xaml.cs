using MahApps.Metro.Controls;
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
            //初始化
            webMain.Navigate("http://www.dmm.com/");
        }
    }
}
