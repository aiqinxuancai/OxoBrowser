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


        [DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
        private static extern int UrlMkSetSessionOption(int dwOption, string pBuffer, int dwBufferLength, int dwReserved);

        const int URLMON_OPTION_USERAGENT = 0x10000001;

        public static void SetIEUserAgent()
        {
            var ua = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Win64; x64; Trident/5.0; .NET CLR 2.0.50727; SLCC2; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; InfoPath.3; .NET4.0C; Tablet PC 2.0; .NET4.0E)";
            UrlMkSetSessionOption(URLMON_OPTION_USERAGENT, ua, ua.Length, 0);
        }

        public static void OpenUrlFromBrowser(string _url)
        {
            Process.Start(_url);
        }
    }
}
