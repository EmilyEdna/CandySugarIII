using System.Collections.ObjectModel;
using CandySugar.Com.Library;
using CandySugar.Com.Library.Model;
using CandySugar.Com.Pages.ChildViews.Rifans;
using CandySugar.Com.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sdk.Component.Vip.Anime.sdk;
using Sdk.Component.Vip.Anime.sdk.ViewModel;
using Sdk.Component.Vip.Anime.sdk.ViewModel.Enums;
using Sdk.Component.Vip.Anime.sdk.ViewModel.Request;
using Sdk.Component.Vip.Anime.sdk.ViewModel.Response;
using XExten.Advance.IocFramework;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages.ChildViewModels.Rifans
{
    public partial class DetailViewModel : ObservableObject, IQueryAttributable
    {
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Result = (SearchElementResult)query["Param"];
            Application.Current.Dispatcher.DispatchAsync(PreviewAsync);
        }
        #region Property
        [ObservableProperty]
        private SearchElementResult _Result;
        [ObservableProperty]
        private ObservableCollection<PlayInfo> _Current;
        [ObservableProperty]
        private ObservableCollection<WatchElementResult> _LinkResult;
        #endregion

        #region Method
        private async void PreviewAsync()
        {
            try
            {
                var result = (await AnimeFactory.Anime(opt =>
                {
                    opt.RequestParam = new Input
                    {

                        AnimeType = AnimeEnum.Watch,
                        CacheSpan = 5,
                        Watch = new AnimeWatch
                        {
                            Route = Result.Route
                        }
                    };
                }).RunsAsync()).WatchResult;
                Current = new ObservableCollection<PlayInfo>(result.Current.Select(t => new PlayInfo
                {
                    Clarity = $"{t.Key}P",
                    Route = t.Value,
                    Name = Result.Name
                }));
                LinkResult = new ObservableCollection<WatchElementResult>(result.Results);
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }
        private async void Insert(SearchElementResult result)
        {
            await IocDependency.Resolve<ICandyService>().Add(new CollectModel
            {
                Category = 2,
                Cover = result.Cover,
                Name = result.Name,
                Route = result.Route,
            });
        }
        private async void Next(string input)
        {
            await Shell.Current.GoToAsync(Extend.RouteMap[nameof(WatchView)], new Dictionary<string, object> { { "Route", input } });
        }
        #endregion

        #region  Command
        public RelayCommand<string> PlayCommand => new(Next);
        public RelayCommand<WatchElementResult> LinkCommand => new(input =>
        {
            Result = input.ToMapest<SearchElementResult>();
            Application.Current.Dispatcher.DispatchAsync(PreviewAsync);
        });
        public RelayCommand LoveCommand => new(() => Insert(Result));
        #endregion
    }
}
