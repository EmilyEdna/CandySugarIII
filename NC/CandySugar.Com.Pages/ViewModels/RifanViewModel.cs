using CandySugar.Com.Library.BaseViewModel;
using CandySugar.Com.Library.Extends;
using CandySugar.Com.Pages.Views.RifanViews;
using Sdk.Component.Vip.Anime.sdk;
using Sdk.Component.Vip.Anime.sdk.ViewModel;
using Sdk.Component.Vip.Anime.sdk.ViewModel.Enums;
using Sdk.Component.Vip.Anime.sdk.ViewModel.Request;
using Sdk.Component.Vip.Anime.sdk.ViewModel.Response;
using System.Collections.ObjectModel;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages.ViewModels
{
    public class RifanViewModel : BaseVMModule
    {

        public RifanViewModel(string QueryKey, BaseVMService baseServices) : base(baseServices)
        {

            Keyword = QueryKey;
            if (!QueryKey.IsNullOrEmpty())
                OnAllInit();
        }

        public override void OnAppearing()
        {
            AllPageIndex = RifanPageIndex = MotionPageIndex = CubicPageIndex = CosplayPageIndex = 1;
        }

        #region Field
        private string Keyword;

        private int AllTotal;
        private int RifanTotal;
        private int MotionTotal;
        private int CubicTotal;
        private int CosplayTotal;

        private int AllPageIndex;
        private int RifanPageIndex;
        private int MotionPageIndex;
        private int CubicPageIndex;
        private int CosplayPageIndex;
        #endregion

        #region Property
        private ObservableCollection<SearchElementResult> _AllResult;
        public ObservableCollection<SearchElementResult> AllResult
        {
            get => _AllResult;
            set => SetProperty(ref _AllResult, value);
        }

        private ObservableCollection<SearchElementResult> _RifanResult;
        public ObservableCollection<SearchElementResult> RifanResult
        {
            get => _RifanResult;
            set => SetProperty(ref _RifanResult, value);
        }

        private ObservableCollection<SearchElementResult> _MotionResult;
        public ObservableCollection<SearchElementResult> MotionResult
        {
            get => _MotionResult;
            set => SetProperty(ref _MotionResult, value);
        }

        private ObservableCollection<SearchElementResult> _CubicResult;
        public ObservableCollection<SearchElementResult> CubicResult
        {
            get => _CubicResult;
            set => SetProperty(ref _CubicResult, value);
        }
        private ObservableCollection<SearchElementResult> _CosplayResult;
        public ObservableCollection<SearchElementResult> CosplayResult
        {
            get => _CosplayResult;
            set => SetProperty(ref _CosplayResult, value);
        }

        private ObservableCollection<SearchElementResult> _CollectResult;
        public ObservableCollection<SearchElementResult> CollectResult
        {
            get => _CollectResult;
            set => SetProperty(ref _CollectResult, value);
        }

        #endregion

        #region Method
        private void ChangeMethod(dynamic input)
        {
            var param = (int)input;
            if (param == 1) OnAllInit();
            if (param == 2) OnRifanInit();
            if (param == 3) OnCubicInit();
            if (param == 4) OnMotionInit();
            if (param == 5) OnCosplayInit();
        }
        private void LoadMethod(string param)
        {
            if (param.Equals("1"))
            {
                AllPageIndex += 1;
                if (AllPageIndex > AllTotal) return;
                OnLoadMoreAllInit();
            }
            if (param.Equals("2"))
            {
                RifanPageIndex += 1;
                if (RifanPageIndex > RifanTotal) return;
                OnLoadMoreRifanInit();
            }
            if (param.Equals("3"))
            {
                CubicPageIndex += 1;
                if (CubicPageIndex > CubicTotal) return;
                OnLoadlMoreCubicInit();
            }
            if (param.Equals("4"))
            {
                MotionPageIndex += 1;
                if (MotionPageIndex > MotionTotal) return;
                OnLoadMoreMotionInit();
            }
            if (param.Equals("5"))
            {
                CosplayPageIndex += 1;
                if (CosplayPageIndex > CosplayTotal) return;
                OnLoadMoreCosplayInit();
            }
        }
        #endregion

        #region Command
        public DelegateCommand<dynamic> ChangeCommand => new(ChangeMethod);
        public DelegateCommand<string> LoadCommand => new(LoadMethod);
        public DelegateCommand<SearchElementResult> WatchCommand => new((element)=>Nav.NavigateAsync(new Uri(nameof(RifanInfo), UriKind.Relative), new NavigationParameters { { "Param", element }}));
        #endregion

        #region ExternalMethod
        private void OnAllInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await AnimeFactory.Anime(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            AnimeType = AnimeEnum.Search,
                            Search = new AnimeSearch
                            {
                                Page = 1,
                                Keyword = Keyword,
                                SearchType = SearchEnum.All
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    AllTotal = result.Total;
                    AllResult = new ObservableCollection<SearchElementResult>(result.Results);
                }
                catch (Exception ex)
                {
                    ex.Message.Info();
                }
            });
        }

        private void OnRifanInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await AnimeFactory.Anime(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            AnimeType = AnimeEnum.Search,
                            Search = new AnimeSearch
                            {
                                Page = 1,
                                Keyword = Keyword,
                                SearchType = SearchEnum.Rifan
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    RifanTotal = result.Total;
                    RifanResult = new ObservableCollection<SearchElementResult>(result.Results);
                }
                catch (Exception ex)
                {
                    ex.Message.Info();
                }
            });
        }

        private void OnMotionInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await AnimeFactory.Anime(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            AnimeType = AnimeEnum.Search,
                            Search = new AnimeSearch
                            {
                                Page = 1,
                                Keyword = Keyword,
                                SearchType = SearchEnum.Montion
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    MotionTotal = result.Total;
                    MotionResult = new ObservableCollection<SearchElementResult>(result.Results);
                }
                catch (Exception ex)
                {
                    ex.Message.Info();
                }
            });
        }

        private void OnCubicInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await AnimeFactory.Anime(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            AnimeType = AnimeEnum.Search,
                            Search = new AnimeSearch
                            {
                                Page = 1,
                                Keyword = Keyword,
                                SearchType = SearchEnum.Cubic
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    CubicTotal = result.Total;
                    CubicResult = new ObservableCollection<SearchElementResult>(result.Results);
                }
                catch (Exception ex)
                {
                    ex.Message.Info();
                }
            });
        }

        private void OnCosplayInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await AnimeFactory.Anime(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            AnimeType = AnimeEnum.Search,
                            Search = new AnimeSearch
                            {
                                Page = 1,
                                Keyword = Keyword,
                                SearchType = SearchEnum.Cosplay
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    CosplayTotal = result.Total;
                    CosplayResult = new ObservableCollection<SearchElementResult>(result.Results);
                }
                catch (Exception ex)
                {
                    ex.Message.Info();
                }
            });
        }
        #endregion

        #region ExternalMoreMethod
        private void OnLoadMoreAllInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await AnimeFactory.Anime(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            AnimeType = AnimeEnum.Search,
                            Search = new AnimeSearch
                            {
                                Page = AllPageIndex,
                                Keyword = Keyword,
                                SearchType = SearchEnum.All
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    Application.Current.Dispatcher.Dispatch(() => result.Results.ForEach(item =>
                    {
                        if (!result.Results.Any(t => t.Name == item.Name))
                            AllResult.Add(item);
                    }));
                }
                catch (Exception ex)
                {
                    ex.Message.Info();
                }
            });
        }

        private void OnLoadMoreRifanInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await AnimeFactory.Anime(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            AnimeType = AnimeEnum.Search,
                            Search = new AnimeSearch
                            {
                                Page = RifanPageIndex,
                                Keyword = Keyword,
                                SearchType = SearchEnum.Rifan
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    Application.Current.Dispatcher.Dispatch(() => result.Results.ForEach(item =>
                    {
                        if (!result.Results.Any(t => t.Name == item.Name))
                            RifanResult.Add(item);
                    }));
                }
                catch (Exception ex)
                {
                    ex.Message.Info();
                }
            });
        }

        private void OnLoadMoreMotionInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await AnimeFactory.Anime(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            AnimeType = AnimeEnum.Search,
                            Search = new AnimeSearch
                            {
                                Page = MotionPageIndex,
                                Keyword = Keyword,
                                SearchType = SearchEnum.Montion
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    Application.Current.Dispatcher.Dispatch(() => result.Results.ForEach(item =>
                    {
                        if (!result.Results.Any(t => t.Name == item.Name))
                            MotionResult.Add(item);
                    }));
                }
                catch (Exception ex)
                {
                    ex.Message.Info();
                }
            });
        }

        private void OnLoadlMoreCubicInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await AnimeFactory.Anime(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            AnimeType = AnimeEnum.Search,
                            Search = new AnimeSearch
                            {
                                Page = CubicPageIndex,
                                Keyword = Keyword,
                                SearchType = SearchEnum.Cubic
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    Application.Current.Dispatcher.Dispatch(() => result.Results.ForEach(item =>
                    {
                        if (!result.Results.Any(t => t.Name == item.Name))
                            CubicResult.Add(item);
                    }));
                }
                catch (Exception ex)
                {
                    ex.Message.Info();
                }
            });
        }

        private void OnLoadMoreCosplayInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await AnimeFactory.Anime(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            AnimeType = AnimeEnum.Search,
                            Search = new AnimeSearch
                            {
                                Page = CosplayPageIndex,
                                Keyword = Keyword,
                                SearchType = SearchEnum.Cosplay
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    Application.Current.Dispatcher.Dispatch(() => result.Results.ForEach(item =>
                    {
                        if (!result.Results.Any(t => t.Name == item.Name))
                            CosplayResult.Add(item);
                    }));
                }
                catch (Exception ex)
                {
                    ex.Message.Info();
                }
            });
        }
        #endregion
    }
}
