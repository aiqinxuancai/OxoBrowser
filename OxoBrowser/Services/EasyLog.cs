using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace OxoBrowser.Services
{
    public enum LogLevel { Trace, Debug, Info, Warn, Error }

    static class EasyLog
    {
        private static object flag = new object();

        private static string baseLogPath = @".\OxoBrowser.log";

        /// <summary>
        /// 如果日志大于10M则清除
        /// </summary>
        static EasyLog ()
        {
            FileInfo finfo = new FileInfo(baseLogPath);
            if (!finfo.Exists)
            {
                FileStream fs = File.Create(baseLogPath);
                fs.Close();
            }
            else
            {
                try
                {
                    if (finfo.Length > 1024 * 1024 * 20)
                    {
                        File.Delete(baseLogPath);
                        FileStream fs = File.Create(baseLogPath);
                        fs.Close();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }
        }


        public static void Write(object obj, LogLevel type = LogLevel.Info, string file = null)
        {
            ThreadPool.QueueUserWorkItem(h =>
            {
                lock (flag)
                {
                    if (file == null)   // 默认输出到应用目录下
                    {
                        file = baseLogPath;//AppDomain.CurrentDomain.BaseDirectory + DateTime.Now.ToString("yyyy_MM_dd") + ".log";
                    }
                    string head = string.Format(">>>{0}[{1}]", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), type.ToString());
                    Debug.WriteLine(head + obj.ToString());
                    File.AppendAllText(file, head + obj.ToString() + "\r\n");
                }
            });
        }




    }
}
