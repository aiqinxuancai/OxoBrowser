using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Base
{
    class IEProxyHelper
    {
        [DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
        private static extern int UrlMkSetSessionOption(int dwOption, string pBuffer, int dwBufferLength, int dwReserved);
        const int URLMON_OPTION_USERAGENT = 0x10000001;

        public static bool SetProxy(string _proxy)
        {

            if (_proxy == "")
            {
                IEProxy proxy = new IEProxy(_proxy);
                proxy.DisableIEProxy();
                if (proxy.RefreshIESettings())
                {
                    return true;
                }
            }
            else
            {
                IEProxy proxy = new IEProxy(_proxy);
                if (proxy.RefreshIESettings())
                {
                    return true;
                }
            }
            return false;
        }


        public static void SetIEUserAgent()
        {
            var ua = "Mozilla/5.0 (compatible; MSIE 11.0; Windows NT 6.1; Win64; x64; Trident/5.0; .NET CLR 2.0.50727; SLCC2; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.3; .NET4.0C; Tablet PC 2.0; .NET4.0E)";
            UrlMkSetSessionOption(URLMON_OPTION_USERAGENT, ua, ua.Length, 0);
        }

        public static void OpenUrlFromBrowser(string _url)
        {
            Process.Start(_url);
        }


        public static void SetIE11KeyforWebBrowserControl(string appName)
        {
            RegistryKey Regkey = null;
            try
            {
                Regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_BROWSER_EMULATION", true);
                if (Regkey == null)
                {
                    Debug.WriteLine("Application Settings Failed - Address Not found");
                    return;
                }
                string FindAppkey = Convert.ToString(Regkey.GetValue(appName));
                if (FindAppkey == "11000")
                {
                    Debug.WriteLine("Required Application Settings Present");
                    Regkey.Close();
                    return;
                }
                else
                {
                    Regkey.SetValue(appName, unchecked((int)0x2AF8), RegistryValueKind.DWord);
                }

                FindAppkey = Convert.ToString(Regkey.GetValue(appName));
                if (FindAppkey == "11000")
                {
                    Debug.WriteLine("Application Settings Applied Successfully");
                }
                else
                {
                    Debug.WriteLine("Application Settings Failed, Ref: " + FindAppkey);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                //Close the Registry 
                if (Regkey != null)
                    Regkey.Close();
            }


        }


        public static void SetGPUKeyforWebBrowserControl(string appName)
        {
            RegistryKey Regkey = null;
            try
            {
                Regkey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"SOFTWARE\\Microsoft\\Internet Explorer\\Main\\FeatureControl\\FEATURE_GPU_RENDERING", true);
                if (Regkey == null)
                {
                    Debug.WriteLine("SetGPUKeyforWebBrowserControl Settings Failed - Address Not found");
                    return;
                }
                string FindAppkey = Convert.ToString(Regkey.GetValue(appName));
                if (FindAppkey == "1")
                {
                    Debug.WriteLine("SetGPUKeyforWebBrowserControl Application Settings Present");
                    Regkey.Close();
                    return;
                }
                else
                {
                    Regkey.SetValue(appName, unchecked((int)0x1), RegistryValueKind.DWord);
                }

                FindAppkey = Convert.ToString(Regkey.GetValue(appName));
                if (FindAppkey == "1")
                {
                    Debug.WriteLine("SetGPUKeyforWebBrowserControl Settings Applied Successfully");
                }
                else
                {
                    Debug.WriteLine("SetGPUKeyforWebBrowserControl Settings Failed, Ref: " + FindAppkey);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                //Close the Registry 
                if (Regkey != null)
                    Regkey.Close();
            }


        }
    }
}
