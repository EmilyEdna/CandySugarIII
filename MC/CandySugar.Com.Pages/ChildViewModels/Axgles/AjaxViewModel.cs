using CandySugar.Com.Library;
using CandySugar.Com.Pages.ChildViews.Axgles;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using System.Text.RegularExpressions;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages.ChildViewModels.Axgles
{
    public partial class AjaxViewModel : ObservableObject, IQueryAttributable
    {
        private bool IsLoaded;
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            IsLoaded = false;
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
            if (!IsLoaded)
            {
                var res = Regex.Matches(Mp4, "https://cdn\\.qooqlevideo\\.com(.*)?mp4").FirstOrDefault().Value;
                if (!res.IsNullOrEmpty())
                {
                    IsLoaded = true;
                    NextAsync(res);
                    WeakReferenceMessenger.Default.Send<AjaxViewModel>();
                }
            }
        }
        private async void NextAsync(string res) 
        {
            await Shell.Current.GoToAsync(Extend.RouteMap[nameof(VideoView)], new Dictionary<string, object> { { "Input", res } });
        }
        #endregion
    }
}
