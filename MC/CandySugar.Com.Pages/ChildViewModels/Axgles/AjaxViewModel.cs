using CandySugar.Com.Library;
using CandySugar.Com.Pages.ChildViews.Axgles;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.RegularExpressions;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages.ChildViewModels.Axgles
{
    public partial class AjaxViewModel : ObservableObject, IQueryAttributable
    {
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Route = (string)query["Param"];
            Title = (string)query["Title"];
        }

        #region Property
        [ObservableProperty]
        private string _Route;
        [ObservableProperty]
        private string _Title;
        #endregion

        #region Method
        public void Play(string Mp4)
        {
            var res = Regex.Matches(Mp4, "https://cdn\\.qooqlevideo\\.com(.*)?mp4").FirstOrDefault().Value;
            if (!res.IsNullOrEmpty()) NextAsync(res);

        }
        private async void NextAsync(string res) 
        {
            await Shell.Current.GoToAsync(Extend.RouteMap[nameof(VideoView)], new Dictionary<string, object> { { "Input", res } });
        }
        #endregion
    }
}
