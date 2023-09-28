using System.Collections.ObjectModel;
using CandySugar.Com.Library;
using CandySugar.Com.Pages.ChildViews.Novels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sdk.Component.Novel.sdk;
using Sdk.Component.Novel.sdk.ViewModel;
using Sdk.Component.Novel.sdk.ViewModel.Enums;
using Sdk.Component.Novel.sdk.ViewModel.Request;
using Sdk.Component.Novel.sdk.ViewModel.Response;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages.ViewModels
{
    public partial class NovelViewModel : ObservableObject
    {
        public NovelViewModel()
        {
            QueryIndex = CateIndex = 1;
            InitAsync();
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
        private NovelInitRootResult _InitResult;
        [ObservableProperty]
        private ObservableCollection<NovelCategoryElementResult> _CateResult;
        #endregion

        #region Method
        private async void InitAsync()
        {
            try
            {
                var result = (await NovelFactory.Novel(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        PlatformType = PlatformEnum.Pencil,
                        CacheSpan = 5,
                        NovelType = NovelEnum.Init
                    };
                }).RunsAsync()).InitResult;
                InitResult = result;
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
                var result = (await NovelFactory.Novel(opt =>
                  {
                      opt.RequestParam = new Input
                      {
                          PlatformType = PlatformEnum.Pencil,
                          CacheSpan = 5,
                          NovelType = NovelEnum.Category,
                          Category = new NovelCategory
                          {
                              Page = CateIndex,
                              Route = CateRoute
                          }
                      };
                  }).RunsAsync()).CategoryResult;
                if (CateIndex <= 1)
                {
                    CateTotal = result.Total;
                    CateResult = new ObservableCollection<NovelCategoryElementResult>(result.ElementResults);
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
                var result = (await NovelFactory.Novel(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        PlatformType = PlatformEnum.Pencil,
                        CacheSpan = 5,
                        NovelType = NovelEnum.Search,
                        Search = new NovelSearch
                        {
                            Page = QueryIndex,
                            SearchKey = QueryKey
                        }
                    };
                }).RunsAsync()).SearchResult;
                var Model = result.ElementResults.ToMapest<List<NovelCategoryElementResult>>();
                if (QueryIndex <= 1)
                {
                    QueryTotal = result.Total;
                    CateResult = new ObservableCollection<NovelCategoryElementResult>(Model);
                }
                else Model.ForEach(CateResult.Add);
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }
        private async void Next(string Name, string Route, int Type)
        {
            await Shell.Current.GoToAsync($"{Extend.RouteMap[nameof(ChapterView)]}?Type={Type}&Name={Name}&Route={Route}");
        }
        #endregion

        #region Command
        public RelayCommand<string> CatalogCommand => new(Input =>
        {
            CateRoute = Input;
            CategoryAsync();
        });
        public RelayCommand MoreCommand => new(() =>
        {
            if (QueryKey.IsNullOrEmpty())
            {
                CateIndex += 1;
                if (CateIndex <= CateTotal)
                    CategoryAsync();
            }
            else
            {
                QueryIndex += 1;
                if (QueryIndex <= QueryTotal)
                    SearchAsync();
            }
        });
        public RelayCommand QueryCommand => new(() =>
        {
            if (QueryKey.IsNullOrEmpty()) return;
            SearchAsync();
        });
        public RelayCommand<NovelCategoryElementResult> TypeOneCommand => new(input => Next(input.BookName, input.Route, 1));
        public RelayCommand<NovelCategoryElementResult> TypeTwoCommand => new(input => Next(input.BookName, input.Route, 2));
        #endregion

    }
}
