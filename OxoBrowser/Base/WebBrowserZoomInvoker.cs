using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.IO;
using OxoBrowser;

namespace Base
{
    public class WebBrowserZoomInvoker
    {
        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr ptr);

        [DllImport("user32.dll", EntryPoint = "ReleaseDC")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateDC(
            string lpszDriver,  // driver name
            string lpszDevice,  // device name
            string lpszOutput,  // not used; should be NULL
            Int64 lpInitData    // optional printer data
        );

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(
            IntPtr hdc,         // handle to DC
            int nIndex          // index of capability
        );

        [DllImport("user32.dll")]
        public static extern bool SetProcessDPIAware();

        const int LOGPIXELSX = 88;
        const int LOGPIXELSY = 90;

        public static System.Drawing.PointF GetCurrentDIPScale()
        {
            System.Drawing.PointF scaleUI = new System.Drawing.PointF(1.0f, 1.0f);
            try
            {
                
                if (App.SysVersion.Major >= 6)
                {
                    SetProcessDPIAware();
                }
                
                IntPtr screenDC = GetDC(IntPtr.Zero);
                int dpi_x = GetDeviceCaps(screenDC, LOGPIXELSX);
                int dpi_y = GetDeviceCaps(screenDC, LOGPIXELSY);

                scaleUI.X = (float)dpi_x / 96.0f;
                scaleUI.Y = (float)dpi_y / 96.0f;
                ReleaseDC(IntPtr.Zero, screenDC);
                return scaleUI;
            }
            catch (System.Exception ex)
            {
                //ToukenBrowser.LogClass.WriteErrorLog(ex.ToString());
            }

            return scaleUI;
        }
    }
}

