using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Nekoxy;
using System.Threading.Tasks;
using System.Diagnostics;
using Base;


namespace OxoBrowser
{
    class HttpHook
    {
        private JsonCallback m_callback;
        [DllImport("AiHttpHook.dll", CharSet = CharSet.Ansi)]
        static extern int StartPageHook(IntPtr _dwNewLong);
        private delegate int JsonCallback(string _file, string _json, string _send);

        public void Init()
        {
            m_callback = new JsonCallback(JsonProc);
            StartPageHook(Marshal.GetFunctionPointerForDelegate(m_callback));
        }

        private int JsonProc(string _file, string _json, string _send)
        {
            return 0;
        }

        //NekoxyHook
        public static void InitNekoxy()
        {
            ReLoadNekoxy();
            HttpProxy.AfterReadRequestHeaders += HttpProxy_AfterReadRequestHeaders;
            HttpProxy.AfterReadResponseHeaders += HttpProxy_AfterReadResponseHeaders;
            HttpProxy.AfterSessionComplete += HttpProxy_AfterSessionComplete;
        }

        public static void ReLoadNekoxy()
        {
            HttpProxy.Shutdown();
            if (string.IsNullOrWhiteSpace(AppConfig.m_config.ProxyIP))
            {
                HttpProxy.UpstreamProxyConfig = new ProxyConfig(ProxyConfigType.SpecificProxy);
            }
            else
            {
                HttpProxy.UpstreamProxyConfig = new ProxyConfig(ProxyConfigType.SpecificProxy, AppConfig.m_config.ProxyIP, int.Parse(AppConfig.m_config.ProxyPort));
            }
            
            HttpProxy.Startup(37161, false, false);
        }

        private static void HttpProxy_AfterSessionComplete(Session obj)
        {
            //Task.Run(() => Debug.WriteLine(obj));
            //执行封包操作
        }

        private static void HttpProxy_AfterReadResponseHeaders(HttpResponse obj)
        {
            //Task.Run(() => Console.WriteLine(obj));
        }

        private static void HttpProxy_AfterReadRequestHeaders(HttpRequest obj)
        {
            //Task.Run(() => Console.WriteLine(obj));
        }

    }





}
