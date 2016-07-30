using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
