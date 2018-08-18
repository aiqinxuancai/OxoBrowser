using Base;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;


namespace Base
{
    class WebScreenshot
    {
        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

        /// <summary>
        /// 对一个WebBrowser进行截图
        /// </summary>
        /// <param name="targetBrowser">我这里用的是Forms的WebBrowser，如果是wpf的，请自己改成Controls并调整参数</param>
        /// <returns></returns>
        public static ImageSource BrowserSnapShot(Window targetBrowser)
        {
            // 获取宽高
            int screenWidth = (int)targetBrowser.ActualWidth;
            int screenHeight = (int)targetBrowser.ActualHeight;

            IntPtr myIntptr = new WindowInteropHelper(targetBrowser).Handle;
            int hwndInt = myIntptr.ToInt32();
            IntPtr hwnd = myIntptr;
            //创建图形
            System.Drawing.Bitmap bm = new System.Drawing.Bitmap(screenWidth, screenHeight, System.Drawing.Imaging.PixelFormat.Format16bppRgb555);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bm);
            IntPtr hdc = g.GetHdc();

            //调用api 把hwnd的内容用图形绘制到hdc 如果你有代码洁癖 可以不使用api 使用g.CopyFromScreen，请自行研究
            bool result = PrintWindow(hwnd, hdc, 0);
            g.ReleaseHdc(hdc);
            g.Flush();

            
            if (result == true) //成功 转换并返回ImageSource
            {
                ImageSourceConverter imageSourceConverter = new ImageSourceConverter();
                MemoryStream stream = new MemoryStream();
                bm.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return (ImageSource)imageSourceConverter.ConvertFrom(stream);
            }
            return null;
        }
    }
}
