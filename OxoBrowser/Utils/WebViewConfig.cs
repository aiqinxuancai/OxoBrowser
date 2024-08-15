using System;
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
using OxoBrowser.Services;


namespace Base
{
    class WebViewConfig
    {
        private const string kDMMDoukenCSS = """
            body {
                margin:0;
                overflow:hidden;
            }

            #game_frame {
            	position:fixed;
            	left:50%;
            	top:0px;
            	margin-left:-480px;
            	z-index:1;
            }

            ul.area-menu{
            	display:none;
            }

            .dmm-ntgnavi {
            	display:none;
            }
            """;

        public static void GetKanColle2ndHtml5Core(CefSharp.Wpf.ChromiumWebBrowser browser)
        {
            try
            {
	            browser.ExecuteScriptAsync("var node = document.createElement('style'); " +
	                "node.innerHTML = 'html, body, iframe {overflow:hidden;margin:0;}'; " +
	                "document.body.appendChild(node);");
	
	            browser.ExecuteScriptAsync("var node = document.createElement('style'); " +
	                "node.innerHTML = 'game_frame {position:fixed; left:50%; top:0px; margin-left:-480px; z-index:1;}'; " +
	                "document.body.appendChild(node);");
	
	            browser.ExecuteScriptAsync("var node = document.createElement('style'); " +
	                "node.innerHTML = 'ul.area-menu {display: none;}'; " +
	                "document.body.appendChild(node);");
	            browser.ExecuteScriptAsync("var node = document.createElement('style'); " +
	                "node.innerHTML = '.dmm-ntgnavi {display: none;}'; " +
	                "document.body.appendChild(node);");
	
	            var game_frame = browser.GetBrowser().GetFrame("game_frame");
	            var source = game_frame?.GetSourceAsync();
	            source?.Wait();
	
	
	            //Browser.SetZoomLevel(Math.Log(zoomFactor, 1.2));
	
	
	            //if (StyleSheetApplied)
	            //{
	            //    Browser.Size = Browser.MinimumSize = new Size(
	            //        (int)(KanColleSize.Width * zoomFactor),
	            //        (int)(KanColleSize.Height * zoomFactor)
	            //        );
	
	            //    CenteringBrowser();
	            //}
	            //"overlap-contents"
	            //var contents_iframe = browser.GetBrowser().GetFrame("contents_iframe");
	            //var list2 = game_frame.Browser.GetFrameNames();
	            //var list = browser.GetBrowser().GetFrameNames();
	            //browser.GetBrowser();
	
	            game_frame?.ExecuteJavaScriptAsync("document.getElementById('spacing_top').style.height = '0px'");
            }
            catch (System.Exception ex)
            {
            	
            }

        }



        public static void GetToukenHtml5Core(CefSharp.Wpf.ChromiumWebBrowser browser)
		{
            try
            {
                var mainframe = GetFrameContainsUrl(browser, @"http://pc-play.games.dmm.com/play/tohken");
                var gameframe = GetFrame(browser, "game_frame");
                var css = "var node = document.createElement('style'); " +
                                "node.innerHTML = '" + kDMMDoukenCSS + "'" +
                                "document.body.appendChild(node);";

                browser.ExecuteScriptAsync("var node = document.createElement('style'); " +
                "node.innerHTML = 'html, body, iframe {overflow:hidden;margin:0;}'; " +
                "document.body.appendChild(node);");

                //chrome.ExecuteScriptAsync("var node = document.createElement('style'); " +
                //    "node.innerHTML = '#game_frame {position:fixed; left:50%; top:0px; margin-left:-480px; z-index:1;}'; " +
                //    "document.body.appendChild(node);");

                browser.ExecuteScriptAsync("var node = document.createElement('style'); " +
    "node.innerHTML = '#game_frame {position: fixed; left: 0; top: 0; width: 100% !important; height: 100% !important; z-index:1;}'; " +
    "document.body.appendChild(node);");



                browser.ExecuteScriptAsync("var node = document.createElement('style'); " +
                    "node.innerHTML = 'ul.area-menu {display: none;}'; " +
                    "document.body.appendChild(node);");
                browser.ExecuteScriptAsync("var node = document.createElement('style'); " +
                    "node.innerHTML = '.dmm-ntgnavi {display: none;}'; " +
                    "document.body.appendChild(node);");


                browser.ExecuteScriptAsync("var node = document.createElement('style'); " +
    "node.innerHTML = '#header {display: none;}'; " +
    "document.body.appendChild(node);");

                //mainframe?.EvaluateScriptAsync(string.Format(Properties.Resources.PageScript, StyleClassID));
                //mainframe?.EvaluateScriptAsync(string.Format(Properties.Resources.PageScript, StyleClassID));

                //mainframe.EvaluateScriptAsync(string.Format(Properties.Resources.PageScript, StyleClassID));
                //gameframe.EvaluateScriptAsync(string.Format(Properties.Resources.FrameScript, StyleClassID));

                //gameframe?.EvaluateScriptAsync(string.Format(Properties.Resources.FrameScript, StyleClassID));
            }
            catch (Exception ex)
            {
                EasyLog.Write(ex.ToString());
            }
        }


        public void ApplyStyleSheet(FrameLoadEndEventArgs e = null)
        {
            
        }

        public static IFrame GetFrame(CefSharp.Wpf.ChromiumWebBrowser browser, string frameName)
        {
            IFrame frame = null;
            var identifiers = browser.GetBrowser().GetFrameIdentifiers();
            foreach (var i in identifiers)
            {
                frame = browser.GetBrowser().GetFrame(i);
                if (frame.Name == frameName)
                    return frame;
            }

            return null;
        }

        public static IFrame GetFrameContainsUrl(CefSharp.Wpf.ChromiumWebBrowser browser, string url)
        {
            IFrame frame = null;
            var identifiers = browser.GetBrowser().GetFrameNames();
            foreach (var item in identifiers)
            {
                frame = browser.GetBrowser().GetFrame(item);

                if (frame.Url.Contains(url))
                {
                    return frame;
                }
            }

            return null;
        }

    }
}
