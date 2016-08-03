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


        /// <summary>
        /// The flags are used to zoom web browser's content.
        /// </summary>
        static readonly int OLECMDEXECOPT_DODEFAULT = 0;
        static readonly int OLECMDID_OPTICAL_ZOOM = 63;

        /// <summary>
        /// This function is used to zoom web browser's content.
        /// </summary>
        /// <param name="webbrowser">The instance of web browser.</param>
        /// <param name="zoom">The zoom scale. It should be 50~400</param>
        /// <remarks>This function must be invoked after the webbrowser has completely loaded the URI.</remarks>
        static void SetZoom(WebBrowser webbrowser, int zoom)
        {
            try
            {
                if (null == webbrowser)
                {
                    return;
                }

                FieldInfo fiComWebBrowser = webbrowser.GetType().GetField(
                    "_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
                if (null != fiComWebBrowser)
                {
                    Object objComWebBrowser = fiComWebBrowser.GetValue(webbrowser);

                    if (null != objComWebBrowser)
                    {
                        object[] args = new object[]
                        {
                            OLECMDID_OPTICAL_ZOOM,
                            OLECMDEXECOPT_DODEFAULT, 
                            zoom, 
                            IntPtr.Zero
                        };
                        objComWebBrowser.GetType().InvokeMember(
                            "ExecWB",
                            BindingFlags.InvokeMethod,
                            null, objComWebBrowser,
                            args);
                    }
                }
            }
            catch (System.Exception ex)
            {
                //ToukenBrowser.LogClass.WriteErrorLog(ex.ToString());
            }
        }

        public static readonly String AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly String EmptyHTMLFilePath = Path.Combine(AppDataPath, "Empty.html");
        public static readonly String EmptyHTML = @"<html><head><meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8""><style>body {overflow-y: hidden;}v\:* {behavior:url(#default#VML);}o\:* {behavior:url(#default#VML);}w\:* {behavior:url(#default#VML);}.shape {behavior:url(#default#VML);}</style></head><body></body></html>";

        public static void AddZoomInvoker(WebBrowser browser)
        {
            if (NeedZoom())
            {
                InitEmptyHTML();
                browser.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(browser_LoadCompleted);
                browser.Navigate(new System.Uri(EmptyHTMLFilePath));
            }
        }

        static void browser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            WebBrowser browser = sender as WebBrowser;
            if (null != browser)
            {
                browser.LoadCompleted -= new System.Windows.Navigation.LoadCompletedEventHandler(browser_LoadCompleted);
                System.Drawing.PointF scaleUI = GetCurrentDIPScale();
                if (100 != (int)(scaleUI.X * 100))
                {
                    SetZoom(browser, (int)(scaleUI.X * scaleUI.Y * 100));
                }
            }
        }

        static void InitEmptyHTML()
        {
            try
            {
                if (!File.Exists(EmptyHTMLFilePath))
                {
                    using (FileStream file = new FileStream(EmptyHTMLFilePath, FileMode.Create))
                    {
                        file.Write(
                            Encoding.UTF8.GetBytes(EmptyHTML),
                            0,
                            Encoding.UTF8.GetByteCount(EmptyHTML));
                    }
                    File.SetAttributes(EmptyHTMLFilePath, FileAttributes.Hidden | FileAttributes.ReadOnly);
                }
            }
            catch (System.Exception ex)
            {
                //ToukenBrowser.LogClass.WriteErrorLog(ex.ToString());
            }
        }

        static bool NeedZoom()
        {
            System.Drawing.PointF scaleUI = GetCurrentDIPScale();
            if (100 != (int)(scaleUI.X * 100))
            {
                return true;
            }

            return false;
        }
    }
}

