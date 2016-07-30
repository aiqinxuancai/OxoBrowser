using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Base
{
    /// <summary>
    /// 设置WebBrowser控件的代理服务
    /// 设置后不会影响IE浏览器
    /// MSDN:http://msdn.microsoft.com/en-us/library/aa385114%28v=vs.85%29.aspx
    /// </summary>
    public class IEProxy
    {
        private const int INTERNET_OPTION_PROXY = 38;
        private const int INTERNET_OPEN_TYPE_PROXY = 3;
        private const int INTERNET_OPEN_TYPE_DIRECT = 1;
        private const int INTERNET_OPTION_USER_AGENT = 41;
        private const int URLMON_OPTION_USERAGENT = 0x10000001;

        private string ProxyStr;

        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);

        [DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
        private static extern int UrlMkSetSessionOption(int dwOption, string pBuffer, int dwBufferLength, int dwReserved);

        public struct Struct_INTERNET_PROXY_INFO
        {
            public int dwAccessType;
            public IntPtr proxy;
            public IntPtr proxyBypass;
        }

        public struct Struct_OPTION_USER_AGENT_INFO
        {
            public int dwAccessType;
            public IntPtr proxy;
            public IntPtr proxyBypass;
        }

        /// <summary>
        /// 设置WebBrowser控件代理服务
        /// </summary>
        /// <param name="strProxy"></param>
        /// <returns></returns>
        private bool InternetSetOption(string strProxy)
        {
            int bufferLength;
            IntPtr intptrStruct;
            Struct_INTERNET_PROXY_INFO struct_IPI;

            if (string.IsNullOrEmpty(strProxy) || strProxy.Trim().Length == 0)
            {
                strProxy = string.Empty;
                struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_DIRECT;
            }
            else
            {
                struct_IPI.dwAccessType = INTERNET_OPEN_TYPE_PROXY;
            }

            struct_IPI.proxy = Marshal.StringToHGlobalAnsi(strProxy);
            struct_IPI.proxyBypass = Marshal.StringToHGlobalAnsi("local");

            bufferLength = Marshal.SizeOf(struct_IPI);
            intptrStruct = Marshal.AllocCoTaskMem(bufferLength);
            Marshal.StructureToPtr(struct_IPI, intptrStruct, true);
            return InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY, intptrStruct, bufferLength);
        }

        public IEProxy(string strProxy)
        {
            this.ProxyStr = strProxy;
        }

        /// 设置IE代理服务
        public bool RefreshIESettings()
        {
            return InternetSetOption(this.ProxyStr);
        }

        /// 取消IE代理服务
        /// </summary>
        /// <returns></returns>
        public bool DisableIEProxy()
        {
            return InternetSetOption(string.Empty);
        }

        public void ChangeUserAgent(string Agent)
        {
            IntPtr bstr = Marshal.StringToBSTR(Agent);

            bool result = InternetSetOption(IntPtr.Zero, INTERNET_OPTION_USER_AGENT, bstr, Agent.Length);
            //System.Diagnostics.Trace.WriteLine("设置Anget_User:"+result.ToString());
            UrlMkSetSessionOption(URLMON_OPTION_USERAGENT, Agent, Agent.Length, 0);
        }
    }
}
