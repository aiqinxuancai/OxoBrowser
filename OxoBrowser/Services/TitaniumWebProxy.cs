using Base;
using OxoBrowser.Base;
using OxoBrowser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Titanium.Web.Proxy;
using Titanium.Web.Proxy.Models;

namespace OxoBrowser.Services
{
    public class TitaniumWebProxy
    {
        public static ProxyServer proxyServer;
        public static int localProxyProt;

        public static void Init() 
        {
            proxyServer = new ProxyServer(false);
            proxyServer.ForwardToUpstreamGateway = true;

            //proxyServer.BeforeRequest += OnRequest;
            //proxyServer.BeforeResponse += OnResponse;
            //proxyServer.ServerCertificateValidationCallback += OnCertificateValidation;
            //proxyServer.ClientCertificateSelectionCallback += OnCertificateSelection;

            UpdateUpStreamProxy();
            localProxyProt = GetCanUsePort();
            EasyLog.Write("HttpProxyStart:" + localProxyProt);
            var endPoint = new ExplicitProxyEndPoint(IPAddress.Parse("127.0.0.1"), localProxyProt, false);
            proxyServer.AddEndPoint(endPoint);
            proxyServer.Start();
        }

        public static void UpdateUpStreamProxy()
        {
            string proxy = AppConfig.Instance.ConfigData.ProxyIP + ":" + AppConfig.Instance.ConfigData.ProxyPort;
            

            try
            {
                if (Regex.IsMatch(proxy, @"^\d+$")) //只输入了端口 则补充完全
                {
                    proxy = "127.0.0.1:" + proxy;
                }

                if (string.IsNullOrWhiteSpace(AppConfig.Instance.ConfigData.ProxyIP) || string.IsNullOrWhiteSpace(proxy))
                {
                    proxyServer.UpStreamHttpProxy = null;
                }
                else
                {
                    string[] proxys = proxy.Split(":".ToCharArray());
                    if (proxys.Length != 2)
                    {
                        EasyLog.Write("错误的代理设置");
                    }
                    else
                    {
                        EasyLog.Write($"更新代理地址:{proxys[0]}:{proxys[1]}");

                        proxyServer.UpStreamHttpProxy = new ExternalProxy
                        {
                            HostName = proxys[0],
                            Port = int.Parse(proxys[1]),
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                EasyLog.Write(ex);
                proxyServer.UpStreamHttpProxy = null;
            }

            
        }

        public static int GetCanUsePort()
        {
            for (int i = 0; i < 20; i++)
            {
                var port = 37161 + i;
                if (SocketHelper.PortInUse(port) == false)
                {
                    return port;
                }
            }
            return 0;
        }

        //public static async Task OnRequest(object sender, SessionEventArgs e)
        //{
        //    Debug.WriteLine(e.WebSession.Request.Url);
        //}

        ///// Allows overriding default certificate validation logic
        //public static Task OnCertificateValidation(object sender, CertificateValidationEventArgs e)
        //{
        //    //set IsValid to true/false based on Certificate Errors
        //    if (e.SslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
        //        e.IsValid = true;

        //    return Task.FromResult(0);
        //}

        ///// Allows overriding default client certificate selection logic during mutual authentication
        //public static Task OnCertificateSelection(object sender, CertificateSelectionEventArgs e)
        //{
        //    //set e.clientCertificate to override
        //    e.ClientCertificate = null;
        //    return Task.FromResult(0);
        //}
    }
}
