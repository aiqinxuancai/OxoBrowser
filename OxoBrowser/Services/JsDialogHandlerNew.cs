using System;
using CefSharp;

namespace OxoBrowser.Services
{
    class JsDialogHandlerNew : IJsDialogHandler
    {
        public bool OnJSBeforeUnload(IWebBrowser browserControl, IBrowser browser, string message, bool isReload, IJsDialogCallback callback)
        {
            return true;
        }

        public bool OnJSDialog(IWebBrowser browserControl, IBrowser browser, string originUrl, CefJsDialogType dialogType,
            string messageText, string defaultPromptText, IJsDialogCallback callback, ref bool suppressMessage)
        {
            EasyLog.Write($"OnJSDialog:{dialogType}:{messageText}");
            switch (dialogType)
            {
                case CefJsDialogType.Alert:
                    callback.Continue(false, string.Empty);
                    return true;
                case CefJsDialogType.Confirm:
                    callback.Continue(false, string.Empty);
                    return true;
                case CefJsDialogType.Prompt:
                    callback.Continue(false, string.Empty);
                    return true;
                default:
                    return false;
            }
        }

        public bool OnBeforeUnloadDialog(IWebBrowser chromiumWebBrowser, IBrowser browser, string messageText, bool isReload, IJsDialogCallback callback)
        {
            return true;
        }

        public void OnDialogClosed(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
        }

        public void OnResetDialogState(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
        }
    }
}
