using CandySugar.Com.Library.KeepOn;
using Microsoft.Web.WebView2.Core;
using Serilog;
using System;
using System.Threading.Tasks;
using System.Windows;
using XExten.Advance.StaticFramework;

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
                var data = await WebPlayer.CoreWebView2.ExecuteScriptAsync("$('iframe')[1].contentWindow.config.url");
                if (data != "null" && data.Contains(".m3u8"))
                {
                    Log.Logger.Debug($"流媒体地址：{data.Replace("\"", "")}");
                    WebPlayer.CoreWebView2.Navigate(new Uri($"{Environment.CurrentDirectory}\\Assets\\Player.html").AbsoluteUri);
                    await Task.Delay(2000); //等待html加载完成
                    await WebPlayer.CoreWebView2.ExecuteScriptAsync($"opt.uri='{data.Replace("\"", "")}'");
                }
            });
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ScreenKeep.RestoreForCurrentThread();
            this.WebPlayer.Dispose();
            this.Close();
        }
    }
}
