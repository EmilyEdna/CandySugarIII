using System.Collections.ObjectModel;
using CandySugar.Com.Library;
using CandySugar.Com.Library.Model;
using CandySugar.Com.Pages.ChildViews.Rifans;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sdk.Component.Vip.Anime.sdk;
using Sdk.Component.Vip.Anime.sdk.ViewModel;
using Sdk.Component.Vip.Anime.sdk.ViewModel.Enums;
using Sdk.Component.Vip.Anime.sdk.ViewModel.Request;
using Sdk.Component.Vip.Anime.sdk.ViewModel.Response;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages.ViewModels
{
    public partial class RifanViewModel : ObservableObject
    {
        public RifanViewModel()
        {
            Bar = new ObservableCollection<BarModel>
            {
                new BarModel{ Name="全部",Route="1" },
                new BarModel{ Name="里番",Route="2" },
                new BarModel{ Name="Motion Anime",Route="3" },
                new BarModel{ Name="3D动画",Route="4" },
                new BarModel{ Name="Cosplay",Route="2" },
            };
        }
        #region Field
        private SearchEnum QeuryType;
        private int Page = 1;
        private int Total;
        #endregion


        #region 
        [ObservableProperty]
        private string _QueryKey;
        [ObservableProperty]
        private ObservableCollection<BarModel> _Bar;
        [ObservableProperty]
        private ObservableCollection<SearchElementResult> _SearchResult;
        #endregion

        #region Method
        private async void InitAsync()
        {
            try
            {
                var result = (await AnimeFactory.Anime(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        CacheSpan = 5,
                        AnimeType = AnimeEnum.Search,
                        Search = new AnimeSearch
                        {
                            Page = Page,
                            Keyword = QueryKey,
                            SearchType = QeuryType
                        }
                    };
                }).RunsAsync()).SearchResult;
                if (Page <= 1)
                {
                    Total = result.Total;
                    SearchResult = new ObservableCollection<SearchElementResult>(result.Results);
                }
                else result.Results.ForEach(SearchResult.Add);
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }
        private async void Next(SearchElementResult Model) 
        {
            await Shell.Current.GoToAsync(Extend.RouteMap[nameof(DetailView)], new Dictionary<string, object> { { "Param", Model } });
        }
        #endregion

        #region Command
        public RelayCommand<string> CatalogCommand => new(args =>
        {
            Page = 1;
            QueryKey = string.Empty;
            QeuryType = (SearchEnum)args.AsInt();
            Application.Current.Dispatcher.DispatchAsync(InitAsync);
        });
        public RelayCommand QueryCommand => new(() =>
        {
            Page = 1;
            Application.Current.Dispatcher.DispatchAsync(InitAsync);
        });
        public RelayCommand MoreCommand => new(() => {

            Page += 1;
            if (Page <= Total)
                Application.Current.Dispatcher.DispatchAsync(InitAsync);
        });
        public RelayCommand<SearchElementResult> PreivewCommand => new(Next);
        #endregion
    }
}
