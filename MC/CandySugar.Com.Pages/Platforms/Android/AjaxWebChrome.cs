using Android.Webkit;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using XExten.Advance.LinqFramework;
using static Android.Webkit.ConsoleMessage;

namespace CandySugar.Com.Pages
{
    public class AjaxWebChrome : MauiWebChromeClient
    {
        private object DataContext;
        public AjaxWebChrome(IWebViewHandler handler) : base(handler)
        {
            if (handler.VirtualView is Microsoft.Maui.Controls.WebView view)
            {
                if (view.Parent is Grid)
                {
                    var dc = view.Parent.Parent.BindingContext;
                    if (dc != null)
                        DataContext = dc;
                }
               
            }
        }

        public override bool OnConsoleMessage(ConsoleMessage consoleMessage)
        {
            if (consoleMessage.InvokeMessageLevel() == MessageLevel.Log)
            {
                var info = consoleMessage.Message();
                if (!info.IsNullOrEmpty())
                {
                    if (info.Contains(".mp4"))
                    {
                        var Method = DataContext.GetType().GetMethod("Play");
                        Method?.Invoke(DataContext, new object[] { info });
                    }
                }
            }
            return base.OnConsoleMessage(consoleMessage);
        }

    }
}
