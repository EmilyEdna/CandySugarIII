using CandySugar.Com.Library;
using CandySugar.Com.Pages.ChildViews.Lights;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sdk.Component.Lovel.sdk;
using Sdk.Component.Lovel.sdk.ViewModel;
using Sdk.Component.Lovel.sdk.ViewModel.Enums;
using Sdk.Component.Lovel.sdk.ViewModel.Request;
using Sdk.Component.Lovel.sdk.ViewModel.Response;
using System.Collections.ObjectModel;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages.ViewModels
{
    public partial class LightViewModel: ObservableObject
    {
        public LightViewModel()
        {
            QueryIndex = CateIndex = 1;
            Application.Current.Dispatcher.DispatchAsync(InitAsync);
        }

        #region Field
        private int QueryTotal;
        private int QueryIndex;
        private int CateTotal;
        private int CateIndex;
        private string CateRoute;
        #endregion

        #region Property
        [ObservableProperty]
        private string _QueryKey;
        [ObservableProperty]
        private ObservableCollection<LovelInitResult> _InitResult;
        [ObservableProperty]
        private ObservableCollection<LovelCategoryElementResult> _CateResult;
        #endregion

        #region Method
        private async void InitAsync()
        {
            try
            {
                var result = (await LovelFactory.Lovel(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        LovelType = LovelEnum.Init,
                        CacheSpan = 5,
                        Login = new()
                    };
                }).RunsAsync()).InitResults;
                InitResult = new ObservableCollection<LovelInitResult>(result);
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }

        private async void CategoryAsync()
        {
            try
            {
                var result = (await LovelFactory.Lovel(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        CacheSpan = 5,
                        LovelType = LovelEnum.Category,
                        Category = new LovelCategory
                        {
                            Page = CateIndex,
                            Route = CateRoute
                        }
                    };
                }).RunsAsync()).CategoryResult;
                if (CateIndex <= 1)
                {
                    CateTotal = result.Total;
                    CateResult = new ObservableCollection<LovelCategoryElementResult>(result.ElementResults);
                }
                else
                    result.ElementResults.ForEach(CateResult.Add);
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }

        private async void SearchAsync()
        {
            try
            {
                var result = (await LovelFactory.Lovel(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        LovelType = LovelEnum.Search,
                        CacheSpan = 5,
                        Search = new LovelSearch
                        {
                            Page = QueryIndex,
                            SearchType = LovelSearchEnum.ArticleName,
                            KeyWord = QueryKey
                        },
                    };
                }).RunsAsync()).SearchResult;
                var Model = result.ElementResults.ToMapest<List<LovelCategoryElementResult>>();
                if (QueryIndex <= 1)
                {
                    QueryTotal = result.Total;
                    CateResult = new ObservableCollection<LovelCategoryElementResult>(Model);
                }
                else Model.ForEach(CateResult.Add);
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }

        private async void Next(string Name, string Route, string Cover)
        {
            await Shell.Current.GoToAsync($"{Extend.RouteMap[nameof(ChaptersView)]}?Name={Name}&Route={Route}&Cover={Cover}");
        }
        #endregion

        #region Command
        public RelayCommand QueryCommand => new(() =>
        {
            if (QueryKey.IsNullOrEmpty()) return;
            QueryIndex = 1;
            Application.Current.Dispatcher.DispatchAsync(SearchAsync);
        });
        public RelayCommand<string> CatalogCommand => new(Input =>
        {
            CateRoute = Input;
            Application.Current.Dispatcher.DispatchAsync(CategoryAsync);
        });
        public RelayCommand MoreCommand => new(() =>
        {
            if (QueryKey.IsNullOrEmpty())
            {
                CateIndex += 1;
                if (CateIndex <= CateTotal)
                    Application.Current.Dispatcher.DispatchAsync(CategoryAsync);
            }
            else
            {
                QueryIndex += 1;
                if (QueryIndex <= QueryTotal)
                    Application.Current.Dispatcher.DispatchAsync(SearchAsync);
            }
        });
        public RelayCommand<LovelCategoryElementResult> ChapterCommand => new(input => Next(input.BookName, input.DetailAddress, input.Cover));
        
        #endregion
    }
}
