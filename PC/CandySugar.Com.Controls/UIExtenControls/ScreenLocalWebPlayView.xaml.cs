using CandySugar.Com.Library.KeepOn;
using Microsoft.Web.WebView2.Core;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace CandySugar.Com.Controls.UIExtenControls
{
    /// <summary>
    /// ScreenLoadWebPlayerView.xaml 的交互逻辑
    /// </summary>
    public partial class ScreenLocalWebPlayView : Window
    {
        public ScreenLocalWebPlayView(string playroute)
        {
            InitializeComponent();
            ScreenKeep.PreventForCurrentThread();
            InitWebView(playroute);
        }

        private void InitWebView(string playroute)
        {
            WebPlayer.Source = new Uri(playroute);
            WebPlayer.NavigationCompleted += PlayLoadEvent;
        }

        private async void PlayLoadEvent(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            await WebPlayer.EnsureCoreWebView2Async();
            WebPlayer.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            WebPlayer.CoreWebView2.Settings.AreDevToolsEnabled = true;
            await this.Dispatcher.BeginInvoke(async () =>
            {
                var res = await Dotry();
                if (res.Contains(".m3u8"))
                {
                    var playuri = res.Replace("\"", "");
                    WebPlayer.CoreWebView2.Navigate(new Uri($"{Environment.CurrentDirectory}\\Assets\\Player.html").AbsoluteUri);
                    await Task.Delay(2000); //等待html加载完成
                    Log.Logger.Debug($"流媒体加载成功！地址：{playuri}");
                    await WebPlayer.CoreWebView2.ExecuteScriptAsync($"opt.uri='{playuri}'");
                }
            });
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

        private void Window_Closed(object sender, EventArgs e)
        {
            ScreenKeep.RestoreForCurrentThread();
            this.WebPlayer.Dispose();
            this.Close();
        }
    }
}
