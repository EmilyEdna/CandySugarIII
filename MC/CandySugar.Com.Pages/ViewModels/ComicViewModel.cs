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
    public partial class ComicViewModel : ObservableObject
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
            await Shell.Current.GoToAsync($"{Extend.RouteMap[nameof(CatalogView)]}?Param={Model}");
        }
        #endregion

        #region Command
        public RelayCommand QueryCommand => new(() =>
        {
            if (QueryKey.IsNullOrEmpty()) return;
            SearchAsync();
        });
        public RelayCommand MoreCommand => new(() =>
        {
            Page += 1;
            if (Page <= Total)
                SearchAsync();
        });
        public RelayCommand<SearchElementResult> PreivewCommand =>new (Next);
        #endregion


    }
}
