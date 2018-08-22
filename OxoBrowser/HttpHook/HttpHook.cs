using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Nekoxy;
using System.Threading.Tasks;
using System.Diagnostics;
using Base;
using OxoBrowser.Services;

namespace OxoBrowser
{

    class AiHttpHook64
    {
        [DllImport("AiHttpHook64.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int StartHook(IntPtr callback);
    }

    class AiHttpHook
    {
        [DllImport("AiHttpHook.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int StartHook(IntPtr callback);
    }


    class HttpHook
    {
        private delegate int HttpCallback(string path, string result, string postData, string host, string headers);
        private static HttpCallback HttpPackCallback;

        public void InitAiHttpHook()
        {
            HttpPackCallback = new HttpCallback(JsonRoute);

            if (Environment.Is64BitProcess)
            {
                //64-bit
                int ret = AiHttpHook64.StartHook(Marshal.GetFunctionPointerForDelegate(HttpPackCallback));
            }
            else
            {
                //估计很长一段时间内都是32-bit
                int ret = AiHttpHook.StartHook(Marshal.GetFunctionPointerForDelegate(HttpPackCallback));
            }
        }

        /// <summary>
        /// AiHttpHook方式的回调函数，支持https等包的直接获取（仅IE下测试通过）
        /// </summary>
        /// <param name="path"></param>
        /// <param name="result"></param>
        /// <param name="postData"></param>
        /// <param name="hostName"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static int JsonRoute(string path, string result, string postData, string hostName, string headers)
        {
            return 0;
        }

        /// <summary>
        /// NekoxyHook初始化，支持http包，不支持https获取明文
        /// </summary>
        public static void InitNekoxy() //使用Nekoxy库来处理封包
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
            //这里要做一下循环的Try，以免中转端口被占用
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
