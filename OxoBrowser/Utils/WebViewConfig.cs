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
using System.Windows.Interop;
using OxoBrowser.Services;
using Microsoft.Web.WebView2.Wpf;


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

        public static void GetKanColle2ndHtml5Core(WebView2 browser)
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

                browser.ExecuteScriptAsync("document.getElementById('spacing_top').style.height = '0px'");
            }
            catch (System.Exception ex)
            {
            	
            }

        }



        public static void GetToukenHtml5Core(WebView2 browser)
		{
            try
            {

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

        

    }
}
