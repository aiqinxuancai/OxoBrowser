﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using WebBrowser = System.Windows.Controls.WebBrowser;
using System.Reflection;
using System.Diagnostics;
using OxoBrowser;
using CefSharp;
using System.Windows.Interop;


namespace Base
{
    class WebViewConfig
    {
        static readonly int OLECMDEXECOPT_DODEFAULT = 0;
        static readonly int OLECMDID_OPTICAL_ZOOM = 63;

        //public static int FlashHeight = 0;
        //public static int FlashWidth = 0;


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

        public static void GetBungo(CefSharp.Wpf.ChromiumWebBrowser browser)
        {
            try
            {
                browser.ExecuteScriptAsync(@"
var del = document.getElementsByClassName('dmm-ntgnavi')[0]
del.parentNode.removeChild(del)
document.getElementById('main-ntg').style.margin= '0px 0px 0 0';
");

//                var gameFrame = browser.GetBrowser().GetFrame("game_frame");
//                gameFrame.ExecuteJavaScriptAsync(@"
//var gameFrameWnd = document.getElementById('game_frame').contentWindow.document;
//var contentsFrameWnd = document.getElementById('contents_iframe').contentWindow.document;

//var wrapper = contentsFrameWnd.getElementById('content-wrapper');
//wrapper.className = '';
//wrapper.children[1].style = 'top: 0px; left: 0px;';

//var style = document.createElement('style')
//style.styleSheet.cssText = 'body { margin:0;     overflow:hidden }  #content-wrapper { 	position:fixed; left:50%; top:0px;  margin-left:-480px; z-index:1; } .relative onframe-origin { top: 0px; left: 0px; } ul.area-menu{ display:none; }  .page-inner { position:absolute; left:0px; top:0px; }';
//                ");

                
            }
            catch (Exception ex)
            {
                Debug.Print($"[{nameof(GetBungo)}]  {ex}");
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
                //<div id="flash"><embed width="960" height="580" id="flash_object" src="http://static.touken-ranbu.jp/client.swf?v=368" type="application/x-shockwave-flash" wmode="opaque" allowScriptAccess="always" allowFullScreen="true" FlashVars="url=http://w004.touken-ranbu.jp&amp;url_static=http://static.touken-ranbu.jp&amp;user_id=396521&amp;tutorial=71106559&amp;cookie_name=sword&amp;cookie_value=h66couimgjcj75jvrsjm8783e7&amp;config=config_prod-sys&amp;t=07d6e1f09e7cb05298a01212e46ff2c2b6d2830d0faa98a126535bfe8994f7e3e1b5bede21888ece505196320f17527f97bb5b9e7bbd175646352a55d0ef1ee5" base="http://static.touken-ranbu.jp/"></div>

                System.Drawing.PointF scaleUI = WebBrowserZoomInvoker.GetCurrentDIPScale();
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
                Debug.WriteLine(ex);
            }
        }

        public static double SetWinSite(WebBrowser _webbrowser, double _site)
        {
            double old_height = 580;
            double old_width = 960;
            double height_min = 75;

            double __site = _site / 100; // + dpi_site
            System.Diagnostics.Debug.WriteLine(__site);

            SetZoom(_webbrowser, (int)_site);//+ dpi_site * 100

            _webbrowser.Height = old_height * (__site);
            _webbrowser.Width = old_width * (__site);

            _webbrowser.MinHeight = _webbrowser.Height + height_min;
            _webbrowser.MinWidth = old_width * __site;

            return _site;
        }

        public static mshtml.IHTMLElement GetFlashObjectFromGameFrame(mshtml.IHTMLElement _gameFrame,out string _firstFrame)
        {
            try
            {
                mshtml.HTMLDocument gameFrameDoc = _gameFrame.document;
                var frm = gameFrameDoc.frames;
                for (int i = 0; i < frm.length; i++)
                {
                    mshtml.IHTMLWindow2 item = (mshtml.IHTMLWindow2)frm.item(i);
                    mshtml.IHTMLDocument2 doc = CodecentrixSample.CrossFrameIE.GetDocumentFromWindow(item); //跨域读取 否则没有权限
                    string text = doc.activeElement.innerHTML != null ? doc.activeElement.innerHTML : "";
                    if (text != null && text.Contains("type=\"application/x-shockwave-flash\"")) // //div id=\"flash\"
                    {
                        _firstFrame = text;
                        foreach (mshtml.IHTMLElement obj in doc.all)
                        {
                            string textObj = obj.innerHTML != null ? obj.innerHTML : "";
                            if (textObj != null && textObj.Contains("type=\"application/x-shockwave-flash\""))
                            {
                                foreach (mshtml.IHTMLElement objContents in obj.all)
                                {
                                    string textObjContents = objContents.innerHTML != null ? objContents.innerHTML : "";
                                    if (textObjContents != null && textObjContents.Contains("type=\"application/x-shockwave-flash\""))
                                    {
                                        foreach (mshtml.IHTMLElement objFlash in objContents.all)
                                        {
                                            string textObjFlash = objFlash.innerHTML != null ? objFlash.innerHTML : "";
                                            if (textObjFlash != null && textObjFlash.Contains("type=\"application/x-shockwave-flash\""))
                                            {
                                                return objFlash;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                _firstFrame = "";
                return null;
            }
            catch (System.Exception ex)
            {
                Debug.WriteLine(ex);
                _firstFrame = "";
                return null;
            }
        }

        public static void RegexGetWidthAndHeight(string _str, out int _width, out int _height)
        {
            Regex regWidth = new Regex("width=\\\"(\\d+)\\\"");
            Regex regHeight = new Regex("height=\\\"(\\d+)\\\"");

            string w = regWidth.Match(_str).Groups[1].Value;
            string h = regHeight.Match(_str).Groups[1].Value;

            if (!string.IsNullOrWhiteSpace(w) && !string.IsNullOrWhiteSpace(h))
            {
                _width = int.Parse(w);
                _height = int.Parse(h);

                return;
            }
            _width = 0;
            _height = 0;
            return;
        }

        public static void ApplyStyleSheet(mshtml.HTMLDocument _doc)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("ApplyStyleSheet");
                var document = _doc;
                if (document == null) return;

                var gameFrame = document.getElementById("game_frame");

                var firstFrame = "";
                mshtml.IHTMLElement flashObject = GetFlashObjectFromGameFrame(gameFrame, out firstFrame);

                if (flashObject != null)
                {
                    //读取宽高
                    string html = flashObject.innerHTML;
                    int w, h;
                    RegexGetWidthAndHeight(html, out w,out h);
                    if (w == 0 && h == 0)
                    {
                        RegexGetWidthAndHeight(firstFrame, out w, out h);
                    }
                    
                    if (w != 0 && h != 0)
                    {
                        //如果宽高都不为空
                        MainWindow.thisFrm.Width = w;
                        MainWindow.thisFrm.Height = h + MainWindow.thisFrm.TitlebarHeight;
                        AppConfig.m_config.FlashWidth = Convert.ToInt32(MainWindow.thisFrm.Width) ;
                        AppConfig.m_config.FlashHeight = Convert.ToInt32(MainWindow.thisFrm.Height);
                    }
                    else
                    {
                        MainWindow.thisFrm.Width = flashObject.offsetWidth;
                        MainWindow.thisFrm.Height = flashObject.offsetHeight + MainWindow.thisFrm.TitlebarHeight;
                        AppConfig.m_config.FlashWidth = Convert.ToInt32(MainWindow.thisFrm.Width);
                        AppConfig.m_config.FlashHeight = Convert.ToInt32(MainWindow.thisFrm.Height);
                    }
                }
                else
                {
                    if (!String.IsNullOrWhiteSpace(firstFrame))
                    {
                        int w, h;
                        RegexGetWidthAndHeight(firstFrame, out w, out h);
                        if (w == 0 && h == 0)
                        {
                            MainWindow.thisFrm.Width = flashObject.offsetWidth;
                            MainWindow.thisFrm.Height = flashObject.offsetHeight + MainWindow.thisFrm.TitlebarHeight;
                            AppConfig.m_config.FlashWidth = Convert.ToInt32(MainWindow.thisFrm.Width);
                            AppConfig.m_config.FlashHeight = Convert.ToInt32(MainWindow.thisFrm.Height);
                        }

                    }

                }


                if (gameFrame == null)
                {
                    if (document.url.Contains(".swf?"))
                    {
                        gameFrame = document.body;
                    }
                }

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
