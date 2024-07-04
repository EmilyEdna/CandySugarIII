namespace CandySugar.Rifan.ViewModels
{
    public partial class IndexViewModel : BasicObservableObject
    {

        public IndexViewModel()
        {
            NavVisible = Visibility.Collapsed;
            Title = ["All", "Rifan", "3D", "Motion", "Cosplay", "Collect"];
            Service = IocDependency.Resolve<IService<RifanModel>>();
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
        }

        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
                Cols = (int)(GlobalParam.MAXWidth / 200);
            else
                Cols = 5;
            BorderWidth = GlobalParam.MAXWidth;
            BorderHeight = GlobalParam.MAXHeight;
            NavLength = GlobalParam.NavLength;
        }
        #endregion

        #region 字段
        private IService<RifanModel> Service;
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

        /// <summary>
        /// 1：全部 2：里番 3：Montion 4：3D 5：Cosplay
        /// </summary>
        private int ChangeType = 1;
        #endregion

        #region 属性
        [ObservableProperty]
        private ObservableCollection<string> _Title;
        [ObservableProperty]
        private ObservableCollection<SearchElementResult> _AllResult;
        [ObservableProperty]
        private ObservableCollection<SearchElementResult> _RifanResult;
        [ObservableProperty]
        private ObservableCollection<SearchElementResult> _MotionResult;
        [ObservableProperty]
        private ObservableCollection<SearchElementResult> _CubicResult;
        [ObservableProperty]
        private ObservableCollection<SearchElementResult> _CosplayResult;
        [ObservableProperty]
        private ObservableCollection<RifanModel> _CollectResult;
        [ObservableProperty]
        private ObservableCollection<SearchElementResult> _LinkResult;
        [ObservableProperty]
        private ObservableCollection<PlayInfo> _Current;
        [ObservableProperty]
        private Dictionary<string, SearchEnum> _Tags;
        [ObservableProperty]
        private string _Company;
        #endregion

        #region 命令
        [RelayCommand]
        public void LinkCompany(string input)
        {
            AllPageIndex = RifanPageIndex = MotionPageIndex = CubicPageIndex = CosplayPageIndex = 1;
            this.ChangeType = 1;
            this.Keyword = input;
            OnAllInit();
        }

        [RelayCommand]
        public void LinkSearch(KeyValuePair<string, SearchEnum> element)
        {
            AllPageIndex = RifanPageIndex = MotionPageIndex = CubicPageIndex = CosplayPageIndex = 1;
            var Change = (int)element.Value;
            this.Keyword = element.Key.ToString();
            if (Change == 2)
                OnRifanInit();
            if (Change == 3)
                OnMotionInit();
            if (Change == 4)
                OnCubicInit();
            if (Change == 5)
                OnCosplayInit();
        }
        [RelayCommand]
        public void Play(PlayInfo input)
        {
            new CandyVlcPlayView($"{input.Name}_{input.Clarity}", input.Route).Show();
        }
        [RelayCommand]
        public void Close() => NavVisible = Visibility.Collapsed;
        [RelayCommand]
        public void Changed(object item)
        {
            var Target = ((CandyToggleItem)item);
            if (Target.FindParent<UserControl>() is IndexView View)
            {
                var Index = Target.Tag.ToString().AsInt();

                if (Index == 0)
                {
                    View.ActiveAnime = 1;
                    View.AnimeX1.Begin();
                }
                if (Index == 1)
                {
                    View.ActiveAnime = 2;
                    View.AnimeX2.Begin();
                }
                if (Index == 2)
                {
                    View.ActiveAnime = 3;
                    View.AnimeX3.Begin();
                }
                if (Index == 3)
                {
                    View.ActiveAnime = 4;
                    View.AnimeX4.Begin();
                }
                if (Index == 4)
                {
                    View.ActiveAnime = 5;
                    View.AnimeX5.Begin();
                }
                if (Index == 5)
                {
                    View.ActiveAnime = 6;
                    View.AnimeX6.Begin();
                }
            }
        }
        [RelayCommand]
        public void Scroll(ScrollChangedEventArgs obj)
        {
            if (ChangeType == 1)
            {
                if (AllPageIndex <= AllTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    AllPageIndex += 1;
                    OnLoadMoreAllInit();
                }
            }
            if (ChangeType == 2)
            {
                if (RifanPageIndex <= RifanTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    RifanPageIndex += 1;
                    OnLoadMoreRifanInit();
                }
            }
            if (ChangeType == 3)
            {
                if (MotionPageIndex <= MotionTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    MotionPageIndex += 1;
                    OnLoadMoreMotionInit();
                }
            }
            if (ChangeType == 4)
            {
                if (CubicPageIndex <= CubicTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    CubicPageIndex += 1;
                    OnLoadlMoreCubicInit();
                }
            }
            if (ChangeType == 5)
            {
                if (CosplayPageIndex <= CosplayTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    CosplayPageIndex += 1;
                    OnLoadMoreCosplayInit();
                }
            }
        }
        [RelayCommand]
        public void Collect(SearchElementResult element)
        {
            var Model = element.ToMapest<RifanModel>();
            Model.PId = Service.Insert(Model);
            CollectResult.Add(Model);
        }
        [RelayCommand]
        public void Watch(SearchElementResult element) => OnWatchInit(element);
        [RelayCommand]
        public void Remove(Guid id)
        {
            CollectResult.Remove(CollectResult.First(t => t.PId == id));
            Service.Remove(id);
        }
        #endregion

        #region 方法

        public void ChangeActive(int ActiveAnime)
        {
            ChangeType = ActiveAnime;
            if (ChangeType == 1 && AllResult == null)
                OnAllInit();
            if (ChangeType == 2 && RifanResult == null)
                OnRifanInit();
            if (ChangeType == 3 && MotionResult == null)
                OnMotionInit();
            if (ChangeType == 4 && CubicResult == null)
                OnCubicInit();
            if (ChangeType == 5 && CosplayResult == null)
                OnCosplayInit();
            if (ChangeType == 6)
                CollectResult = new(Service.QueryAll());
        }

        private void ErrorNotify(string input = "") =>
                 Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(input.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : input).Show());

        private void OnAllInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await AnimeFactory.Anime(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
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
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnRifanInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await AnimeFactory.Anime(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
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
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnMotionInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await AnimeFactory.Anime(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
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
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnCubicInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await AnimeFactory.Anime(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
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
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnCosplayInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await AnimeFactory.Anime(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
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
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnLoadMoreAllInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await AnimeFactory.Anime(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            AnimeType = AnimeEnum.Search,
                            Search = new AnimeSearch
                            {
                                Page = AllPageIndex,
                                Keyword = Keyword,
                                SearchType = SearchEnum.All
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    Application.Current.Dispatcher.Invoke(() => result.Results.ForEach(item =>
                    {
                        if (!result.Results.Any(t => t.Name == item.Name))
                            AllResult.Add(item);
                    }));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnLoadMoreRifanInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await AnimeFactory.Anime(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            AnimeType = AnimeEnum.Search,
                            Search = new AnimeSearch
                            {
                                Page = RifanPageIndex,
                                Keyword = Keyword,
                                SearchType = SearchEnum.Rifan
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    Application.Current.Dispatcher.Invoke(() => result.Results.ForEach(item =>
                    {
                        if (!result.Results.Any(t => t.Name == item.Name))
                            RifanResult.Add(item);
                    }));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnLoadMoreMotionInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await AnimeFactory.Anime(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            AnimeType = AnimeEnum.Search,
                            Search = new AnimeSearch
                            {
                                Page = MotionPageIndex,
                                Keyword = Keyword,
                                SearchType = SearchEnum.Montion
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    Application.Current.Dispatcher.Invoke(() => result.Results.ForEach(item =>
                    {
                        if (!result.Results.Any(t => t.Name == item.Name))
                            MotionResult.Add(item);
                    }));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnLoadlMoreCubicInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await AnimeFactory.Anime(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            AnimeType = AnimeEnum.Search,
                            Search = new AnimeSearch
                            {
                                Page = CubicPageIndex,
                                Keyword = Keyword,
                                SearchType = SearchEnum.Cubic
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    Application.Current.Dispatcher.Invoke(() => result.Results.ForEach(item =>
                    {
                        if (!result.Results.Any(t => t.Name == item.Name))
                            CubicResult.Add(item);
                    }));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnLoadMoreCosplayInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await AnimeFactory.Anime(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            AnimeType = AnimeEnum.Search,
                            Search = new AnimeSearch
                            {
                                Page = CosplayPageIndex,
                                Keyword = Keyword,
                                SearchType = SearchEnum.Cosplay
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    Application.Current.Dispatcher.Invoke(() => result.Results.ForEach(item =>
                    {
                        if (!result.Results.Any(t => t.Name == item.Name))
                            CosplayResult.Add(item);
                    }));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnWatchInit(SearchElementResult element)
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await AnimeFactory.Anime(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            AnimeType = AnimeEnum.Watch,
                            Watch = new AnimeWatch
                            {
                                Route = element.Route
                            }
                        };
                    }).RunsAsync()).WatchResult;
                    Current = new ObservableCollection<PlayInfo>(result.Current.Select(t => new PlayInfo
                    {
                        Clarity = $"{t.Key}P",
                        Route = t.Value,
                        Name = element.Name
                    }));
                    Company = result.Company;
                    Tags = result.CurrentTag;
                    LinkResult = new ObservableCollection<SearchElementResult>(result.Results.ToMapest<List<SearchElementResult>>());
                    NavVisible = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        #endregion

    }
}
