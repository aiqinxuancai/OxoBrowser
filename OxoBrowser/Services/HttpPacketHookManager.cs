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
using CefSharp;

namespace OxoBrowser
{
    class HttpPacketHookManager
    {
        //private delegate int HttpCallback(string path, string result, string postData, string host, string headers);
        //private static HttpCallback HttpPackCallback;

        /// <summary>
        /// NekoxyHook初始化，仅用来快速设置端口修改
        /// </summary>
        public static void InitNekoxy() 
        {
            ReLoadNekoxy();

            //HttpProxy.AfterReadRequestHeaders += HttpProxy_AfterReadRequestHeaders;
            //HttpProxy.AfterReadResponseHeaders += HttpProxy_AfterReadResponseHeaders;
            //HttpProxy.AfterSessionComplete += HttpProxy_AfterSessionComplete;
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
            //TODO 这里要做一下循环的Try，以免中转端口被占用
            HttpProxy.Startup(37161, false, false);
        }

        /// <summary>
        /// 该request是否需要被拦截，拦截后会被提交到PacketRoute
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool HookThisRequest(IRequest request)
        {
            var url = new Uri(request.Url);
            var extension = url.ToString().ToLower();
            if (request.Method == "POST" && url.AbsoluteUri.Contains("touken-ranbu.jp/"))
            {
                return true;
            }
            return false;
        }

        public static int PacketRoute(string path, string result, string postData, string hostName, string headers)
        {
            //TODO 2021.03.02 使用这里作为包处理节点

            return 0;
        }

        [Obsolete]
        private static void HttpProxy_AfterSessionComplete(Session obj)
        {
            //Task.Run(() => Debug.WriteLine(obj));
            //执行封包操作
        }

        [Obsolete]
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
