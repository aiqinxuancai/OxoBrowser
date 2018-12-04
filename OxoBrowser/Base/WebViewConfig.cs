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


namespace Base
{
    class WebViewConfig
    {

        public static void GetKanColle2ndHtml5Core(CefSharp.Wpf.ChromiumWebBrowser browser)
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

 
            //"overlap-contents"
            //var contents_iframe = browser.GetBrowser().GetFrame("contents_iframe");
            //var list2 = game_frame.Browser.GetFrameNames();
            //var list = browser.GetBrowser().GetFrameNames();
            //browser.GetBrowser();

            game_frame?.ExecuteJavaScriptAsync("document.getElementById('spacing_top').style.height = '0px'");

        }
    }
}
