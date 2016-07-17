using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

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

            //数据在这里
            return 0;

        }
    }
}
