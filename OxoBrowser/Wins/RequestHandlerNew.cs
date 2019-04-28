using CefSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OxoBrowser.Wins
{
    class RequestHandlerNew : CefSharp.Handler.DefaultRequestHandler
    {
        private Dictionary<UInt64, MemoryStreamResponseFilter> responseDictionary = new Dictionary<UInt64, MemoryStreamResponseFilter>();

        public IRequestHandler _requestHeandler;

        public RequestHandlerNew(IRequestHandler rh) : base()
        {
            _requestHeandler = rh;
        }


        public override CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            var url = new Uri(request.Url);
            var extension = url.ToString().ToLower();

            if (Uri.TryCreate(request.Url, UriKind.Absolute, out url) == false)
            {
                return CefReturnValue.Cancel;
            }

            if (_requestHeandler != null)
            {
                return _requestHeandler.OnBeforeResourceLoad(browserControl, browser, frame, request, callback); /*(browserControl, browser, frame, request, callback);*/
            }


            return CefReturnValue.Continue;
        }

        public override IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {

            var url = new Uri(request.Url);
            //("加载完毕" + url);
            var extension = url.ToString().ToLower();



            Debug.WriteLine(extension);
            if (url.AbsoluteUri.Contains("/kcsapi/"))//这里填你要截的数据的路径关键字
            {
                var dataFilter = new MemoryStreamResponseFilter();//新建成数据 处理器
                responseDictionary.Add(request.Identifier, dataFilter);
                return dataFilter;
            }


            if (_requestHeandler != null)
            {
                return _requestHeandler.GetResourceResponseFilter(browserControl, browser, frame, request, response);
            }

            return null;
        }

        public override void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
            var url = new Uri(request.Url);
            var extension = url.ToString().ToLower();

            if (!extension.Contains("png") && !extension.Contains("mp3")) {
                MemoryStreamResponseFilter filter;
                if (responseDictionary.TryGetValue(request.Identifier, out filter))
                {
                    Debug.WriteLine("------------------------------------");
                    Debug.WriteLine(extension);
                    if (request.PostData?.Elements?.Count > 0)
                    {
                        foreach (var item in request.PostData.Elements)
                        {
                            Debug.WriteLine(Encoding.UTF8.GetString(item.Bytes)); 
                        }
                    }

                    System.Diagnostics.Debug.WriteLine("responseDictionary.Count:" + responseDictionary.Count);

                    var data = filter.Data;
                    var dataLength = filter.Data.Length;
                    var dataAsUtf8String = Encoding.UTF8.GetString(data);
                    Debug.WriteLine(dataAsUtf8String);

                    responseDictionary.Remove(request.Identifier);
                }
            }


            if (_requestHeandler != null)
            {
                _requestHeandler.OnResourceLoadComplete(browserControl, browser, frame, request, response, status, receivedContentLength);
            }

        }
    }
}
