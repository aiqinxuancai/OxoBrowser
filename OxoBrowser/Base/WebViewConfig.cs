using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using WebBrowser = System.Windows.Controls.WebBrowser;
using System.Reflection;
using System.Diagnostics;

namespace Base
{
    class WebViewConfig
    {
        static readonly int OLECMDEXECOPT_DODEFAULT = 0;
        static readonly int OLECMDID_OPTICAL_ZOOM = 63;

        public static void DisableWebScroll(mshtml.HTMLDocument _doc)
        {
            mshtml.HTMLDocument dom = _doc; //定义HTML

            if (dom != null)
            {
                if (dom.body != null)
                {
                    dom.body.setAttribute("scrollTop", 0); //禁用浏览器的滚动条
                    dom.body.setAttribute("scrollLeft", 0); //禁用浏览器的滚动条
                    dom.body.scrollIntoView();
                }

            }
        }

        public static void SetWebBrowserSilent(WebBrowser webBrowser, bool silent)
        {
            FieldInfo fi = typeof(WebBrowser).GetField("_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            if (fi != null)
            {
                object browser = fi.GetValue(webBrowser);
                if (browser != null)
                    browser.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, browser, new object[] { silent });
            }
        }

        static void SetZoom(WebBrowser _webbrowser, int zoom)
        {
            try
            {

                System.Drawing.PointF scaleUI = ToukenBrowser.WebBrowserZoomInvoker.GetCurrentDIPScale();
                if (100 != (int)(scaleUI.X * 100))
                {
                    //SetZoom(browser, (int)(scaleUI.X * scaleUI.Y * 100));
                    zoom = (int)(scaleUI.X * scaleUI.Y * zoom);
                }


                if (null == _webbrowser)
                {
                    return;
                }

                FieldInfo fiComWebBrowser = _webbrowser.GetType().GetField(
                "_axIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
                if (null != fiComWebBrowser)
                {
                    Object objComWebBrowser = fiComWebBrowser.GetValue(_webbrowser);

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
                //ToukenBrowser.LogClass.WriteErrorLog("SetZoom:" + ex.ToString());
            }
        }

        public static double SetWinSite(WebBrowser _webbrowser, double _site)
        {
            double old_height = 580;
            double old_width = 960;
            double height_min = 75;
            //double dpi_site = 0;
            //int dpi = Dpi.GetDpi();


            double __site = _site / 100; // + dpi_site
            System.Diagnostics.Debug.WriteLine(__site);

            SetZoom(_webbrowser, (int)_site);//+ dpi_site * 100

            _webbrowser.Height = old_height * (__site);
            _webbrowser.Width = old_width * (__site);

            _webbrowser.MinHeight = _webbrowser.Height + height_min;
            _webbrowser.MinWidth = old_width * __site;

            return _site;

        }


        public static void ApplyStyleSheet(mshtml.HTMLDocument _doc)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("ApplyStyleSheet");
                var document = _doc;
                if (document == null) return;
                
                var gameFrame = document.getElementById("game_frame");
                mshtml.HTMLDocument gameFrameDoc = gameFrame.document;
                //foreach (mshtml.IHTMLElement item in )
                //{
                //    if (item.outerHTML.Contains("flash"))
                //    {
                //        Debug.WriteLine(item.outerHTML);
                //    }
                //}

                var frm = gameFrameDoc.frames;
                for (int i = 0; i < frm.length; i++)
                {
                    mshtml.IHTMLWindow2 item = (mshtml.IHTMLWindow2)frm.item(i);
                    mshtml.IHTMLDocument2 doc = CodecentrixSample.CrossFrameIE.GetDocumentFromWindow(item);
                    string text = doc.activeElement.innerHTML;
                    if (text != null && text.Contains("div id=\"flash\""))
                    {
                        Debug.WriteLine("---------------------------");
                        Debug.WriteLine(text);
                        int start = text.IndexOf("div id=\"flash\"");
                        int start2 = text.IndexOf("embed", start);




                    }
                }

                

                //mshtml.HTMLDocument gameFrameDoc = gameFrame.document;
                //mshtml.IHTMLFramesCollection2 frames = (mshtml.IHTMLFramesCollection2)gameFrameDoc.frames;


                //foreach (mshtml.IHTMLElement item in gameFrameDoc.all)
                //{
                //    if (item.outerHTML.Contains("flash"))
                //    {
                //        Debug.WriteLine(item.outerHTML);
                //    }
                //}

                //foreach (mshtml.IHTMLElement item in gameFrame.all)
                //{
                //    if (item.outerHTML.Contains("flash"))
                //    {
                //        Debug.WriteLine(item.outerHTML);
                //    }
                //}



                //var flashFrame = gameFrameDoc.getElementById("flash_object"); 

                //if (gameFrame == null)
                //{
                //    if (document.url.Contains(".swf?"))
                //    {
                //        gameFrame = document.body;
                //    }
                //}

                if (gameFrame != null)
                {
                    var target = gameFrame.document as mshtml.HTMLDocument;
                    if (target != null)
                    {
                        var body = target.body;
                        target.createStyleSheet().cssText = "body {\r\n    margin:0;\r\n    overflow:hidden\r\n}\r\n\r\n#game_frame {\r\n    position:fixe" +
            "d;\r\n    left:50%;\r\n    top:0px;\r\n    margin-left:-480px;\r\n    z-index:1\r\n}\r\nul.area-menu{display:none;}";
                        //this.styleSheetApplied = true;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                //ToukenBrowser.LogClass.WriteErrorLog(ex.ToString());
            }
            return;
        }


        public static void ApplyStyleSheetOld(mshtml.HTMLDocument _doc)
        {
            try
            {

                mshtml.HTMLDocument dom = _doc; //定义HTML
                mshtml.IHTMLElement div = dom.getElementById("area-game");
                string div_str = dom.getElementById("area-game").outerHTML;

                //ToukenBrowser.LogClass.WriteLogFile("area-game:" + div_str);
                System.Diagnostics.Debug.WriteLine("area-game:" + div_str);

                if (div_str.IndexOf("baidu") != -1) //找到广告
                {
                    Regex re = new Regex("height[\"= ]{1,3}(\\d*)[\"= ]{1,3}", RegexOptions.Compiled);
                    MatchCollection mc = re.Matches(div_str);

                    foreach (Match m in mc)
                    {
                        System.Diagnostics.Debug.WriteLine(m.Result("$0") + " / " + m.Result("$1"));
                        if (int.Parse(m.Result("$1")) < 120)
                        {
                            string new_h = m.Result("$0").Replace(m.Result("$1"), "0");
                            System.Diagnostics.Debug.WriteLine(m.Result("$0") + "->" + new_h);
                            div_str = div_str.Replace(m.Result("$0"), new_h);
                        }
                    }
                }

                if (div_str != "" && div_str != null)
                {
                    dom.body.setAttribute("innerHTML", div_str);
                    dom.documentElement.style.overflow = "hidden"; //隐藏浏览器的滚动条
                    dom.body.scrollIntoView();
                    dom.body.setAttribute("scroll", "no"); //禁用浏览器的滚动条
                }

            }
            catch (System.Exception ex)
            {
                //ToukenBrowser.LogClass.WriteErrorLog("CheckGameUrl:" + ex.ToString());
            }


        }

    }
}
