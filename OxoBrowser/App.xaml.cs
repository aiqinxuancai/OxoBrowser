using Base;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;



namespace OxoBrowser
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// 程序appdata目录
        /// </summary>
        public static string AppDataPath = "";
        /// <summary>
        /// 程序运行目录
        /// </summary>
        public static string AppPath = "";
        public static Version SysVersion = null;

        public static string AppName = "";

        App() //初始化
        {
            InitAppPath();
            //IEProxyHelper.SetIEUserAgent();
            IEProxyHelper.SetIE11KeyforWebBrowserControl(AppName);
            IEProxyHelper.SetGPUKeyforWebBrowserControl(AppName);
            AppConfig.Init();
            HttpHook.InitNekoxy();
            //IEProxyHelper.SetProxy("127.0.0.1:37161");
        }


        void InitAppPath()
        {
            string appdata_path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OxoBrowser";

            if (Directory.Exists(appdata_path) == false)
            {
                Directory.CreateDirectory(appdata_path);
            }
            AppDataPath = appdata_path;
            AppPath = System.IO.Directory.GetCurrentDirectory();
            SysVersion = System.Environment.OSVersion.Version;
            AppName = Process.GetCurrentProcess().ProcessName + ".exe";
        }

    }
}
