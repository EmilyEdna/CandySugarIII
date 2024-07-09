using CandyControls;
using CandyControls.ControlsModel.Thicks;
using CandySugar.Com.Library;
using CandySugar.Com.Library.KeepOn;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CandySugar.Com.Controls.ExtenControls
{
    public class CandyWebPlayControl : CandyWindow
    {
        private string _Route;
        private WebView2 WebPlayer;
        private bool _Mode;
        /// <summary>
        /// 网页视频播放器
        /// </summary>
        /// <param name="Route"></param>
        /// <param name="Mode">true直接播放，false执行JS后在播放</param>
        public CandyWebPlayControl(string Route, bool Mode) : base()
        {
            this._Route = Route;
            this._Mode = Mode;
            this.Title = "网页视频播放器";
            this.Handle = new WindowHandleStruct(false, true, true, true);
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
            await WebPlayer.EnsureCoreWebView2Async();
            WebPlayer.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            WebPlayer.CoreWebView2.Settings.AreDevToolsEnabled = true;
            WebPlayer.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            WebPlayer.CoreWebView2.WebResourceRequested += (s, e) => { };
            if (!_Mode)
            {
                await this.Dispatcher.BeginInvoke(async () =>
                {
                    var res = await Dotry();
                    if (res.Contains(".m3u8"))
                    {
                        var playuri = res.Replace("\"", "");
                        WebPlayer.CoreWebView2.Navigate(new Uri(CommonHelper.PlayerHtml).AbsoluteUri);
                        await Task.Delay(2000); //等待html加载完成
                        Log.Logger.Information($"流媒体加载成功！地址：{playuri}");
                        await WebPlayer.CoreWebView2.ExecuteScriptAsync($"opt.uri='{playuri}'");
                    }
                });
            }
        }


        private async Task<string> Dotry()
        {
            try
            {
                await Task.Delay(2000); //等待html加载完成
                var data = await WebPlayer.CoreWebView2.ExecuteScriptAsync("$('iframe')[1].contentWindow.config.url");
                var res = data != "null" && data.Contains(".m3u8");
                if (res) return data;
                else return await Dotry();
            }
            catch (Exception)
            {
                this.Close();
                return string.Empty;
            }

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
