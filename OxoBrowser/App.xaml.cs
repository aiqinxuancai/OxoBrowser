using Base;
using CefSharp;
using CefSharp.Wpf;
using OxoBrowser.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
            IEProxyHelper.SetIE11KeyforWebBrowserControl(AppName);
            IEProxyHelper.SetGPUKeyforWebBrowserControl(AppName);

            AppDomain.CurrentDomain.AssemblyResolve += Resolver;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            ThemeService.Apply(AppConfig.Instance.ConfigData.Theme);
            base.OnStartup(e);
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

        private static readonly object CefInitLock = new object();
        private static bool CefInitialized = false;

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static void InitializeCefSharp()
        {
            lock (CefInitLock)
            {
                if (CefInitialized)
                {
                    return;
                }

                var setting = new CefSettings()
                {
                    CachePath = Directory.GetCurrentDirectory() + @"\Cache",
                };

                //setting.RemoteDebuggingPort = 8088;
                setting.Locale = "zh-CN";
                setting.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/126.0.0.0 Safari/537.36";

                //代理设置
                setting.CefCommandLineArgs.Add("enable-npapi", "1");
                if (TitaniumWebProxy.localProxyProt > 0)
                {
                    setting.CefCommandLineArgs.Add("--proxy-server", $"http://127.0.0.1:{TitaniumWebProxy.localProxyProt}");
                }
                else
                {
                    Debug.WriteLine("Proxy port not ready. Skip proxy-server args.");
                }
                //setting.CefCommandLineArgs.Add("--no-proxy-server", "1");

                //硬件加速设置
                setting.CefCommandLineArgs.Add("--enable-media-stream", "1");
                //setting.CefCommandLineArgs.Add("disable-gpu", "0");
                setting.SetOffScreenRenderingBestPerformanceArgs();
                //setting.CefCommandLineArgs.Add("disable-gpu", "1");
                //setting.CefCommandLineArgs.Add("disable-gpu-compositing", "1");
                //setting.CefCommandLineArgs.Add("enable-begin-frame-scheduling", "1");
                setting.CefCommandLineArgs.Add("enable-media-stream", "1");

                //Flash设置
                setting.CefCommandLineArgs["enable-system-flash"] = "0";
                //setting.CefCommandLineArgs.Add("enable-system-flash", "0"); //Automatically discovered and load a system-wide installation of Pepper Flash.
                setting.CefCommandLineArgs.Add("ppapi-flash-path", @".\plugins\pepflashplayer64_23_0_0_162.dll"); //Load a specific pepper flash version (Step 1 of 2)
                setting.CefCommandLineArgs.Add("ppapi-flash-version", "23.0.0.162"); //Load a specific pepper flash version (Step 2 of 2)

                // Set BrowserSubProcessPath based on app bitness at runtime
                setting.BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                       Environment.Is64BitProcess ? "x64" : "x86",
                                                       "CefSharp.BrowserSubprocess.exe");

                if (!Cef.Initialize(setting, performDependencyCheck: false, browserProcessHandler: null))
                {
                    throw new Exception("Unable to Initialize Cef");
                }

                CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

                CefInitialized = true;
            }

        }

        private static Assembly Resolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp"))
            {
                string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                       Environment.Is64BitProcess ? "x64" : "x86",
                                                       assemblyName);

                return File.Exists(archSpecificPath)
                           ? Assembly.LoadFile(archSpecificPath)
                           : null;
            }

            return null;
        }

    }
}
