using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.RegularExpressions;
using XExten.Advance.LinqFramework;
#if ANDROID
using XExten.Advance.Maui.Bar;
using XExten.Advance.Maui.Direction;
using XExten.Advance.Maui.Direction.Platforms.Android;
#endif

namespace CandySugar.Com.Pages.ChildViewModels.Axgles
{
    public partial class VideoViewModel : ObservableObject, IQueryAttributable
    {
        public VideoViewModel()
        {
            DisplayWebView = true;
            DisplayExoView = false;
        }
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Route = (string)query["Param"];
        }

        #region Property
        [ObservableProperty]
        private string _URI;
        [ObservableProperty]
        private string _Route;
        [ObservableProperty]
        private bool _DisplayWebView;
        [ObservableProperty]
        private bool _DisplayExoView;
        #endregion

        #region Method
        public void Play(string Mp4)
        {
            var res = Regex.Matches(Mp4, "https://cdn\\.qooqlevideo\\.com(.*)?mp4").FirstOrDefault().Value;
            if (!res.IsNullOrEmpty())
            {
#if ANDROID
                IBarStatus.Instance.HiddenStatusBar();
                IDirection.Instance.LockOrientation(OrientationEnum.Landscape);
#endif
                URI = res;
                DisplayExoView = true;
                DisplayWebView = false;
            }
        }

        #endregion
    }
}
