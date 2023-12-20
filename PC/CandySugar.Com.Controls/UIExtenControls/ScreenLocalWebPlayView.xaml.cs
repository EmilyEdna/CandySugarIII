using CandySugar.Com.Library.KeepOn;
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

        private  void InitWebView(string playroute)
        {
            WebPlayer.Source= new Uri(playroute);
            WebPlayer.NavigationCompleted += async delegate
            {
                await WebPlayer.EnsureCoreWebView2Async();
                WebPlayer.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
                WebPlayer.CoreWebView2.Settings.AreDevToolsEnabled = true;
                await this.Dispatcher.BeginInvoke(Dotry);
            };
        }

        private async void Dotry()
        {
            while (true) 
            {
                var data = await WebPlayer.CoreWebView2.ExecuteScriptAsync("$('iframe')[1].contentWindow.config.url");
                if (data != "null" && data.Contains(".m3u8"))
                {
                    WebPlayer.CoreWebView2.Navigate(new Uri($"{Environment.CurrentDirectory}\\Assets\\Player.html").AbsoluteUri);
                    await WebPlayer.CoreWebView2.ExecuteScriptAsync($"Play('{data.Replace("\"", "")}')");
                    break;
                }
                else await Task.Delay(1000);
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
