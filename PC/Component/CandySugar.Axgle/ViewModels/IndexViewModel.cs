using StackExchange.Redis;

namespace CandySugar.Axgle.ViewModels
{
    public class IndexViewModel : PropertyChangedBase
    {
        private object LockObject = new object();
        private IService<AxgleModel> Service;
        public IndexViewModel()
        {
            Title = ["最新", "热门", "好评", "收藏"];
            Mode = ["Jav", "Skb"];
            PlatformType = PlatformEnum.Jav;
            GenericDelegate.SearchAction = new(SearchHandler);
            Service = IocDependency.Resolve<IService<AxgleModel>>();
            CollectResult = new(Service.QueryAll());
        }

        #region Field
        private int SearchTotal;
        private int SearchPage;
        private int InitPage;
        private int InitTotal;
        private string Keyword;
        private ModeEnum ModeType;
        private PlatformEnum PlatformType;
        #endregion

        #region Property
        private ObservableCollection<string> _Title;
        public ObservableCollection<string> Title
        {
            get => _Title;
            set => SetAndNotify(ref _Title, value);
        }

        private ObservableCollection<string> _Mode;
        public ObservableCollection<string> Mode
        {
            get => _Mode;
            set => SetAndNotify(ref _Mode, value);
        }

        private ObservableCollection<JronElemetInitResult> _Results;
        public ObservableCollection<JronElemetInitResult> Results
        {
            get => _Results;
            set => SetAndNotify(ref _Results, value);
        }

        private ObservableCollection<AxgleModel> _CollectResult;
        public ObservableCollection<AxgleModel> CollectResult
        {
            get => _CollectResult;
            set => SetAndNotify(ref _CollectResult, value);
        }
        #endregion

        #region Command
        /// <summary>
        /// 浏览
        /// </summary>
        /// <param name="element"></param>
        public void WatchCommand(JronElemetInitResult element)
        {
            OnDetail(element);
        }

        /// <summary>
        /// 浏览
        /// </summary>
        /// <param name="element"></param>
        public void PlayCommand(AxgleModel element)
        {
            OnDetail(element.ToMapest<JronElemetInitResult>());
        }

        /// <summary>
        /// 激活模块
        /// </summary>
        /// <param name="param"></param>
        public void ActiveCommand(string param)
        {
            if (param == "Jav")
                PlatformType = PlatformEnum.Jav;
            else
                PlatformType = PlatformEnum.Skb;
            this.Keyword = string.Empty;
            InitPage = SearchPage = 1;
            Results = [];
        }
        /// <summary>
        /// 收藏
        /// </summary>
        /// <param name="element"></param>
        public void CollectCommand(JronElemetInitResult element)
        {
            var Model = element.ToMapest<AxgleModel>();
            Model.Platfrom = PlatformType.AsString();
            Model.PId = Service.Insert(Model);
            CollectResult.Add(Model);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="element"></param>
        public void RemoveCommand(Guid id)
        {
            CollectResult.Remove(CollectResult.First(t => t.PId == id));
            Service.Remove(id);
        }

        public void ChangeCommand(int ActiveAnime)
        {
            this.Keyword = string.Empty;
            InitPage = 1;
            if (ActiveAnime != 4)
                OnInit();
            else
                CollectResult=new(Service.QueryAll());
        }
        /// <summary>
        /// 切换功能
        /// </summary>
        public RelayCommand<object> ChangedCommand => new(item =>
        {
            var Target = ((CandyToggleItem)item);
            if (Target.FindParent<UserControl>() is IndexView View)
            {
                var Temp = Target.Tag.ToString().AsInt();
                if (Temp == 0)
                {
                    ModeType = ModeEnum.Latest;
                    View.ActiveAnime = 1;
                    View.AnimeX1.Begin();
                }
                else if (Temp == 1)
                {
                    ModeType = ModeEnum.Hot;
                    View.ActiveAnime = 2;
                    View.AnimeX2.Begin();
                }
                else if (Temp == 2)
                {
                    ModeType = ModeEnum.Praised;
                    View.ActiveAnime = 3;
                    View.AnimeX3.Begin();
                }
                else
                {
                    View.ActiveAnime = 4;
                    View.AnimeX4.Begin();
                }
            }
        });
        /// <summary>
        /// 加载更多
        /// </summary>
        public RelayCommand<ScrollChangedEventArgs> ScrollCommand => new((obj) =>
        {
            if (this.Keyword.IsNullOrEmpty())
            {
                if (InitPage <= InitTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    InitPage += 1;
                    OnLoadMoreInit();
                }
            }
            else
            {
                if (SearchPage <= SearchTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    SearchPage += 1;
                    OnLoadMoreSearch();
                }

            }
        });
        #endregion

        #region Method

        private void ErrorNotify(string Info = "")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new ScreenNotifyView(Info.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : Info).Show();
            });
        }

        private void OnInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var res = (await JronFactory.Jron(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            Init = new JronInit
                            {
                                Page = InitPage,
                                ModeType = ModeType
                            },
                            PlatformType = PlatformEnum.Jav,
                            JronType = JronEnum.Init
                        };
                    }).RunsAsync()).InitResult;
                    InitTotal = res.Total;
                    Results = new(res.ElementResults);
                    // 这一句很关键，开启集合的异步访问支持
                    BindingOperations.EnableCollectionSynchronization(Results, LockObject);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnLoadMoreInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var res = (await JronFactory.Jron(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            Init = new JronInit
                            {
                                Page = InitPage,
                                ModeType = ModeType
                            },
                            PlatformType = PlatformEnum.Jav,
                            JronType = JronEnum.Init
                        };
                    }).RunsAsync()).InitResult;
                    // 这一句很关键，开启集合的异步访问支持
                    BindingOperations.EnableCollectionSynchronization(Results, LockObject);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        res.ElementResults.ForEach(Results.Add);
                    });
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnSearch()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var res = (await JronFactory.Jron(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            JronType = JronEnum.Search,
                            PlatformType = PlatformType,
                            Search = new JronSearch
                            {
                                Keyword = this.Keyword,
                                Page = SearchPage
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    SearchTotal = res.Total;
                    Results = new(res.ElementResults.ToMapest<List<JronElemetInitResult>>());
                    // 这一句很关键，开启集合的异步访问支持
                    BindingOperations.EnableCollectionSynchronization(Results, LockObject);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnLoadMoreSearch()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var res = (await JronFactory.Jron(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            JronType = JronEnum.Search,
                            PlatformType = PlatformType,
                            Search = new JronSearch
                            {
                                Keyword = this.Keyword,
                                Page = SearchPage
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    // 这一句很关键，开启集合的异步访问支持
                    BindingOperations.EnableCollectionSynchronization(Results, LockObject);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        res.ElementResults.ToMapest<List<JronElemetInitResult>>().ForEach(Results.Add);
                    });
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }


        private void OnDetail(JronElemetInitResult input)
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await JronFactory.Jron(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            JronType = JronEnum.Detail,
                            PlatformType = PlatformType,
                            Play = new JronPlay
                            {
                                Route = input.Route
                            }
                        };
                    }).RunsAsync()).PlayResult.Play;

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        new ScreenWebPlayView
                        {
                            DataContext = new ScreenWebPlayViewModel
                            {
                                Route = result
                            }
                        }.Show();
                    });
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
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
            this.SearchPage = 1;
            OnSearch();
        }
        #endregion
    }
}
