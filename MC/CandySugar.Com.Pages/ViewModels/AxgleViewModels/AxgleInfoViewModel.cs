using CandySugar.Com.Library.BaseViewModel;
using CandySugar.Com.Library.Extends;
using CandySugar.Com.Pages.Views.AxgleViews;
using CandySugar.Com.Service;
using CommunityToolkit.Mvvm.Input;
using Sdk.Component.Vip.Axgle.sdk;
using Sdk.Component.Vip.Axgle.sdk.ViewModel;
using Sdk.Component.Vip.Axgle.sdk.ViewModel.Enums;
using Sdk.Component.Vip.Axgle.sdk.ViewModel.Request;
using Sdk.Component.Vip.Axgle.sdk.ViewModel.Response;
using System.Collections.ObjectModel;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages.ViewModels.AxgleViewModels
{
    public class AxgleInfoViewModel : BaseVMModule
    {
        public AxgleInfoViewModel(BaseVMService baseServices) : base(baseServices)
        {
            SearchPageIndex = InfoPageIndex = 1;
        }

        public override void Initialize(INavigationParameters parameters)
        {
            var target = parameters.GetValue<dynamic>("Param");
            if (target is string)
            {
                Keyword = target;
                OnSearch();
            }
            else
            {
                Keyword = string.Empty;
                Init = target;
                OnInfo();
            }
        }

        #region Filed
        private AxgleInitResult Init;

        private string Keyword;
        private int SearchPageIndex;
        private int InfoPageIndex;

        private int SearchTotal;
        private int InfoTotal;
        #endregion

        #region Property
        private ObservableCollection<AxgleSearchElementResult> _SearchResult;
        public ObservableCollection<AxgleSearchElementResult> SearchResult
        {
            get => _SearchResult;
            set => SetProperty(ref _SearchResult, value);
        }
        #endregion

        #region ExternalMethod
        private async void OnSearch()
        {
            try
            {
                var result = (await AxgleFactory.Axgle(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        AxgleType = AxgleEnum.Search,
                        Search = new AxgleSearch
                        {
                            KeyWord = Keyword,
                            Page = SearchPageIndex
                        }
                    };
                }).RunsAsync()).SearchResult;
                SearchTotal = result.Total;
                SearchResult = new ObservableCollection<AxgleSearchElementResult>(result.ElementResult);
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }

        private async void OnInfo()
        {
            try
            {
                var result = (await AxgleFactory.Axgle(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        AxgleType = AxgleEnum.Category,
                        Category = new AxgleCategory
                        {
                            CId = Init.AId.AsInt(),
                            Page = InfoPageIndex,
                            PageSize = 15
                        }
                    };
                }).RunsAsync()).CategoryResult;
                InfoTotal = result.Total;
                SearchResult = new ObservableCollection<AxgleSearchElementResult>(result.ElementResult.ToMapest<List<AxgleSearchElementResult>>());
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }

        private async void OnPlay(AxgleSearchElementResult element)
        {
            var result = (await AxgleFactory.Axgle(opt =>
            {
                opt.RequestParam = new Input
                {
                    AxgleType = AxgleEnum.Detail,
                    Detail = new AxgleDetail
                    {
                        FrameURL = element.Play
                    }
                };
            }).RunsAsync()).DetailResult;
            Insert(element, result.Route);
            await Nav.NavigateAsync(new Uri(nameof(AxglePlay), UriKind.Relative), new NavigationParameters { { "Param", result.Route } });
        }
        #endregion

        #region ExternalMoreMethod
        private async void OnSearchMore()
        {
            try
            {
                var result = (await AxgleFactory.Axgle(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        AxgleType = AxgleEnum.Search,
                        Search = new AxgleSearch
                        {
                            KeyWord = Keyword,
                            Page = SearchPageIndex
                        }
                    };
                }).RunsAsync()).SearchResult;
                result.ElementResult.ForEach(SearchResult.Add);
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }

        private async void OnInfoMore()
        {
            try
            {
                var result = (await AxgleFactory.Axgle(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        AxgleType = AxgleEnum.Category,
                        Category = new AxgleCategory
                        {
                            CId = Init.AId.AsInt(),
                            Page = InfoPageIndex,
                            PageSize = 15
                        }
                    };
                }).RunsAsync()).CategoryResult;
                result.ElementResult.ToMapest<List<AxgleSearchElementResult>>().ForEach(SearchResult.Add);
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }

        private void OnLoadMore()
        {
            if (Keyword.IsNullOrEmpty())
            {
                InfoPageIndex += 1;
                if (InfoPageIndex > InfoTotal) return;
                OnInfoMore();
            }
            else
            {
                SearchPageIndex += 1;
                if (SearchPageIndex > SearchTotal) return;
                OnSearchMore();
            }
        }
        #endregion

        #region Method
        private async void Insert(AxgleSearchElementResult result, string Route)
        {
            await Container.Resolve<ICandyService>().Add(new CollectModel
            {
                Category = 1,
                Cover = result.Preview,
                Name = result.Title,
                Route = Route,
            });
        }
        #endregion

        #region Command
        public RelayCommand LoadCommand => new(OnLoadMore);

        public RelayCommand BackCommand => new(() => Nav.GoBackAsync());

        public RelayCommand<AxgleSearchElementResult> WatchCommand => new(OnPlay);
        #endregion
    }
}
