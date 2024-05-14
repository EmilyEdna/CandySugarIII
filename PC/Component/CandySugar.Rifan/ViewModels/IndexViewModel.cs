namespace CandySugar.Rifan.ViewModels
{
    public class IndexViewModel : PropertyChangedBase
    {
        private object LockObject = new object();
        private IService<RifanModel> Service;
        public IndexViewModel()
        {
            Title = ["All", "Rifan", "3D", "Motion", "Cosplay", "Collect"];
            GenericDelegate.SearchAction = new(SearchHandler);
            var LocalDATA = Service.QueryAll();
            CollectResult = [];
            if (LocalDATA != null)
            {
                LocalDATA.ForEach(CollectResult.Add);
            }
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

        /// <summary>
        /// 1：全部 2：里番 3：Montion 4：3D 5：Cosplay
        /// </summary>
        private int ChangeType = 1;
        #endregion

        #region Property
        private ObservableCollection<string> _Title;
        public ObservableCollection<string> Title
        {
            get => _Title;
            set => SetAndNotify(ref _Title, value);
        }

        private ObservableCollection<SearchElementResult> _AllResult;
        public ObservableCollection<SearchElementResult> AllResult
        {
            get => _AllResult;
            set => SetAndNotify(ref _AllResult, value);
        }

        private ObservableCollection<SearchElementResult> _RifanResult;
        public ObservableCollection<SearchElementResult> RifanResult
        {
            get => _RifanResult;
            set => SetAndNotify(ref _RifanResult, value);
        }

        private ObservableCollection<SearchElementResult> _MotionResult;
        public ObservableCollection<SearchElementResult> MotionResult
        {
            get => _MotionResult;
            set => SetAndNotify(ref _MotionResult, value);
        }

        private ObservableCollection<SearchElementResult> _CubicResult;
        public ObservableCollection<SearchElementResult> CubicResult
        {
            get => _CubicResult;
            set => SetAndNotify(ref _CubicResult, value);
        }
        private ObservableCollection<SearchElementResult> _CosplayResult;
        public ObservableCollection<SearchElementResult> CosplayResult
        {
            get => _CosplayResult;
            set => SetAndNotify(ref _CosplayResult, value);
        }

        private ObservableCollection<RifanModel> _CollectResult;
        public ObservableCollection<RifanModel> CollectResult
        {
            get => _CollectResult;
            set => SetAndNotify(ref _CollectResult, value);
        }

        private ObservableCollection<SearchElementResult> _LinkResult;
        public ObservableCollection<SearchElementResult> LinkResult
        {
            get => _LinkResult;
            set => SetAndNotify(ref _LinkResult, value);
        }

        private ObservableCollection<PlayInfo> _Current;
        public ObservableCollection<PlayInfo> Current
        {
            get => _Current;
            set => SetAndNotify(ref _Current, value);
        }

        private Dictionary<string, SearchEnum> _Tags;
        public Dictionary<string, SearchEnum> Tags
        {
            get => _Tags;
            set => SetAndNotify(ref _Tags, value);
        }
        private string _Compnay;
        public string Company 
        {
            get => _Compnay;
            set => SetAndNotify(ref _Compnay, value);
        }
        #endregion

        #region Method

        private void ErrorNotify(string Info = "")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new ScreenNotifyView(Info.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : Info).Show();
            });
        }

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
                    BindingOperations.EnableCollectionSynchronization(AllResult, LockObject);
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
                    BindingOperations.EnableCollectionSynchronization(RifanResult, LockObject);
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
                    BindingOperations.EnableCollectionSynchronization(MotionResult, LockObject);
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
                    BindingOperations.EnableCollectionSynchronization(CubicResult, LockObject);
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
                    BindingOperations.EnableCollectionSynchronization(CosplayResult, LockObject);
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
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        #endregion

        #region Command
        public RelayCommand<object> ChangedCommand => new((item) =>
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
        });

        public void ChangeCommand(int ActiveAnime)
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
        }

        /// <summary>
        /// 加载更多
        /// </summary>
        public RelayCommand<ScrollChangedEventArgs> ScrollCommand => new((obj) =>
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
        });

        /// <summary>
        /// 收藏
        /// </summary>
        /// <param name="param"></param>
        public void CollectCommand(SearchElementResult element)
        {
            var Model = element.ToMapest<RifanModel>();
            Model.Id = Service.Insert(Model);
            CollectResult.Add(Model);
        }

        /// <summary>
        /// 观看
        /// </summary>
        /// <param name="param"></param>
        public void WatchCommand(SearchElementResult element)
        {
            OnWatchInit(element);
            WeakReferenceMessenger.Default.Send(new MessageNotify());
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="param"></param>
        public void RemoveCommand(Guid id)
        {
            CollectResult.Remove(CollectResult.First(t=>t.Id==id));
            Service.Remove(id);
        }

        /// <summary>
        /// 获取该公司所有作品
        /// </summary>
        /// <param name="input"></param>
        public void LinkCompanyCommand(string input)
        {
            AllPageIndex = RifanPageIndex = MotionPageIndex = CubicPageIndex = CosplayPageIndex = 1;
            this.ChangeType = 1;
            this.Keyword = input;
            OnAllInit();
        }

        /// <summary>
        /// 根据标签查询
        /// </summary>
        /// <param name="element"></param>
        public void LinkSearchCommand(KeyValuePair<string,SearchEnum> element)
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
        #endregion

        #region ExternalCalls
        /// <summary>
        /// 检索数据
        /// </summary>
        /// <param name="keyword"></param>
        private void SearchHandler(string keyword)
        {
            this.Keyword = keyword;
            AllPageIndex = RifanPageIndex = MotionPageIndex = CubicPageIndex = CosplayPageIndex = 1;
            if (ChangeType == 1)
                OnAllInit();
            if (ChangeType == 2)
                OnRifanInit();
            if (ChangeType == 3)
                OnMotionInit();
            if (ChangeType == 4)
                OnCubicInit();
            if (ChangeType == 5)
                OnCosplayInit();
        }
        #endregion
    }
}
