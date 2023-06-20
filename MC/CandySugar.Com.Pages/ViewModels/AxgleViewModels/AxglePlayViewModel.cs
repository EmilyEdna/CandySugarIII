using CandySugar.Com.Library.BaseViewModel;
using System.Text;

namespace CandySugar.Com.Pages.ViewModels.AxgleViewModels
{
    public class AxglePlayViewModel : BaseVMModule
    {
        public AxglePlayViewModel(BaseVMService baseServices) : base(baseServices)
        {
        }

        public override void Initialize(INavigationParameters parameters)
        {
            Route = parameters.GetValue<string>("Param");
        }

        #region Property
        private string _Route;
        public string Route
        {
            get => _Route;
            set => SetProperty(ref _Route, value);
        }
        #endregion

        #region Command
        public DelegateCommand BackCommand => new(() => Nav.GoBackAsync());
        public DelegateCommand<WebView> LoadCommand => new(async element =>
        {
            var Height = (DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density);
            var Width = (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density);
            await element.EvaluateJavaScriptAsync($"Play('{Route}','{Width}','{Height}')");
        });

        public DelegateCommand<WebView> ReloadCommand => new(element =>
        {
            element.Reload();
        });

        public DelegateCommand<WebView> ClearCommand => new(ClearAd);
        #endregion

        #region Util
        private static string[] ClassName = { "alert alert-dismissable alert-danger",
            "hd-text-icon",
            "top-nav",
            "well well-filters",
            "navbar navbar-inverse navbar-fixed-top",
            "nav nav-tabs",
            "tab-content m-b-20",
            "pull-left user-container",
            "pull-right big-views hidden-xs",
            "m-t-10 overflow-hidden",
            "col-md-4 col-sm-5",
            "footer-container",
            "col-lg-12",
            "fps60-text-icon",
            "btn btn-primary",
            "vote-box col-xs-7 col-sm-2 col-md-2",
            "pull-right m-t-15",
            "video-banner"};
        public static void ClearAd(WebView WebView)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in ClassName)
            {
                sb.Append($"$(document.getElementsByClassName('{item}')).remove();");
            }
            sb.Append("$(document.getElementById('ps32-container')).remove();");
            sb.Append("$(document.getElementsByTagName('iframe')).remove();");
            sb.Append("$('div[style*=\"position:absolute;left:18px;display: block;font-size:10px;\"]').remove();");
            sb.Append("$('div[style*=\"position:absolute;right:18px; display: block;font-size:10px;\"]').remove();");
            sb.Append("$('#wrapper').css('padding-bottom','0px');");
            sb.Append("$('body').css('padding-top','0px');");
            sb.Append("$('#video-player').css({'max-width':'1190px','width':'1190px','margin-left':'-30px'});");
            WebView.EvaluateJavaScriptAsync(sb.ToString());
        }
        #endregion
    }
}
