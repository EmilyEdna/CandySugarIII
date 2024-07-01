using CandySugar.Com.Controls.ExtenControls;
using CandySugar.Com.Library.KeepOn;
using Microsoft.Web.WebView2.Core;
using Serilog;
using Stylet;
using System;

namespace CandySugar.Com.Controls.UIExtenControls
{
    /// <summary>
    /// ScreenWebPlayView.xaml 的交互逻辑
    /// </summary>
    public partial class ScreenWebPlayView : CandyWindow
    {
        public ScreenWebPlayView()
        {
            InitializeComponent();
            ScreenKeep.PreventForCurrentThread();
            WebPlayer.NavigationCompleted += CompelteEvent;
        }

        private async void CompelteEvent(object sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            WebPlayer.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            WebPlayer.CoreWebView2.WebResourceRequested += (s, e) => { };
            await WebPlayer.EnsureCoreWebView2Async();
        }


        private void Window_Closed(object sender, EventArgs e)
        {
            ScreenKeep.RestoreForCurrentThread();
            this.WebPlayer.Dispose();
            this.Close();
        }
    }

    public class ScreenWebPlayViewModel : PropertyChangedBase
    {
        private string _Route;
        public string Route
        {
            get => _Route;
            set => SetAndNotify(ref _Route, value);
        }
        private string _Name;
        public string Name
        {
            get => _Name;
            set => SetAndNotify(ref _Name, value);
        }
    }
}
