using System.Collections.ObjectModel;
using CandySugar.Com.Library;
using CandySugar.Com.Pages.ChildViews.Comics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sdk.Component.Vip.Comic.sdk;
using Sdk.Component.Vip.Comic.sdk.ViewModel;
using Sdk.Component.Vip.Comic.sdk.ViewModel.Enums;
using Sdk.Component.Vip.Comic.sdk.ViewModel.Request;
using Sdk.Component.Vip.Comic.sdk.ViewModel.Response;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages.ViewModels
{
    public partial class ComicViewModel : ObservableObject, IQueryAttributable
    {
        public ComicViewModel()
        {
            Page = 1;
        }

        #region Field
        private int Page;
        private int Total;
        #endregion

        #region Property
        [ObservableProperty]
        private string _QueryKey;
        [ObservableProperty]
        private ObservableCollection<SearchElementResult> _SearchResult;
        #endregion

        #region Method
        private async void SearchAsync()
        {
            try
            {
                var result = (await ComicFactory.Comic(opt =>
                {
                    opt.RequestParam = new Input
                    {

                        ComicType = ComicEnum.Search,
                        CacheSpan = 5,
                        Search = new ComicSearch
                        {
                            Keyword = QueryKey,
                            Page = Page
                        }
                    };
                }).RunsAsync()).SearchResult;
                if (Page == 1)
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
            await Shell.Current.GoToAsync(Extend.RouteMap[nameof(CatalogView)], new Dictionary<string, object> { { "Param", Model } });
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.Count == 0) return;
            QueryKey = query["Tag"].ToString();
            Page = 1;
            Application.Current.Dispatcher.DispatchAsync(SearchAsync);
        }
        #endregion

        #region Command
        public RelayCommand QueryCommand => new(() =>
        {
            if (QueryKey.IsNullOrEmpty()) return;
            Application.Current.Dispatcher.DispatchAsync(SearchAsync);
        });
        public RelayCommand MoreCommand => new(() =>
        {
            Page += 1;
            if (Page <= Total)
                Application.Current.Dispatcher.DispatchAsync(SearchAsync);
        });
        public RelayCommand<SearchElementResult> PreivewCommand => new(Next);
        #endregion


    }
}
