using CandyControls;
using CandySugar.Com.Library.KeepOn;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CandySugar.Com.Controls.ExtenControls
{
    public class CandyWebPlayControl : CandyWindow
    {
        private string _Route;
        private WebView2 WebPlayer;
        public CandyWebPlayControl(string Route) : base()
        {
            this._Route = Route;
            this.Title = "网页视频播放器";
            this.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/CandySugar.Com.Style;component/Resources/MusicBackgroud.jpg"))
            };
            CreateUI();
            ScreenKeep.PreventForCurrentThread();
            this.Closed += CloseEvent;
            WebPlayer.NavigationCompleted += CompelteEvent;
        }

        private void CloseEvent(object sender, EventArgs e)
        {
            ScreenKeep.RestoreForCurrentThread();
            this.WebPlayer.Dispose();
        }

        private async void CompelteEvent(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            WebPlayer.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            WebPlayer.CoreWebView2.WebResourceRequested += (s, e) => { };
            await WebPlayer.EnsureCoreWebView2Async();
        }

        private void CreateUI()
        {
            Grid grid = new Grid();
            WebPlayer = new WebView2
            {
                Source = new Uri(this._Route)
            };
            grid.Children.Add(WebPlayer);
            Content = grid;
        }
    }
}
