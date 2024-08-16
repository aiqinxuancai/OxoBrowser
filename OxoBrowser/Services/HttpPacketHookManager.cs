using System;


namespace OxoBrowser.Services
{
    class HttpPacketHookManager
    {
        ///// <summary>
        ///// 该request是否需要被拦截，拦截后会被提交到PacketRoute
        ///// </summary>
        ///// <param name="request"></param>
        ///// <returns></returns>
        //public static bool HookThisRequest(IRequest request)
        //{
        //    var url = new Uri(request.Url);
        //    var extension = url.ToString().ToLower();
        //    if (request.Method == "POST" && url.AbsoluteUri.Contains("touken-ranbu.jp/"))
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        public static int PacketRoute(string path, string result, string postData, string hostName, string headers)
        {
            //TODO 2021.03.02 使用这里作为包处理节点

            return 0;
        }


    }





}
