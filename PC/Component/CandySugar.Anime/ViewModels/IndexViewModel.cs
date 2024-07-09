namespace CandySugar.Anime.ViewModels
{
    public partial class IndexViewModel : BasicObservableObject
    {
        public IndexViewModel()
        {
            WindowStateEvent();
            Title = ["全部", "收藏"];
            NavVisible = Visibility.Hidden;
            Service = IocDependency.Resolve<IService<AnimeModel>>();
            GenericDelegate.SearchAction = new(SearchHandler);
            GenericDelegate.WindowStateEvent += WindowStateEvent;
        }

        #region 字段
        private int Total;
        private int Page = 1;
        private string Route;
        private string Keyword;
        private int SearchPage = 1;
        private int SearchTotal;
        private IService<AnimeModel> Service;
        #endregion

        #region 属性
        [ObservableProperty]
        private ObservableCollection<string> _Title;
        [ObservableProperty]
        private ObservableCollection<CartInitElementResult> _InitResult;
        [ObservableProperty]
        private ObservableCollection<AnimeModel> _CollectResult;
        [ObservableProperty]
        private CartDetailRootResult _DetailResult;

        #endregion

        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
                Cols = (int)(GlobalParam.MAXWidth / 240);
            else
                Cols = 5;
            BorderHeight = GlobalParam.MAXHeight;
            BorderWidth = GlobalParam.MAXWidth;
            NavHeight = GlobalParam.NavHeight;
            NavWidth = GlobalParam.NavWidth;
        }
        #endregion

        #region 方法
        public void ChangeActive(int ActiveAnime)
        {
            SearchPage = Page = 1;
            Keyword = string.Empty;
            if (ActiveAnime == 1)
                OnInit();
            else
                CollectResult = new(Service.QueryAll());
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void OnInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await CartFactory.Car(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CartType = CartEnum.Init,
                            Init = new CartInit()
                        };
                    }).RunsAsync()).InitResult;
                    Total = result.Total;
                    InitResult = new(result.ElementResults);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 初始化加载更多
        /// </summary>
        private void OnLoadMoreInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await CartFactory.Car(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CartType = CartEnum.Init,
                            Init = new CartInit
                            {
                                Page = Page
                            }
                        };
                    }).RunsAsync()).InitResult;

                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(InitResult.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 详情
        /// </summary>
        private void OnDetail()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    DetailResult = (await CartFactory.Car(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CartType = CartEnum.Detail,
                            Detail = new CartDetail
                            {
                                Route = Route
                            }
                        };
                    }).RunsAsync()).DetailResult;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 搜索
        /// </summary>
        private void OnSearch()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await CartFactory.Car(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CartType = CartEnum.Search,
                            Search = new CartSearch
                            {
                                Keyword = this.Keyword
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    SearchTotal = result.Total;
                    InitResult = new(result.ElementResults.ToMapest<List<CartInitElementResult>>());
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 获取真实播放地址
        /// </summary>
        private void OnPlay(string args)
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await CartFactory.Car(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CartType = CartEnum.Play,
                            Play = new CartPlay
                            {
                                Route = args
                            }
                        };
                    }).RunsAsync()).PlayResult.PlayRoute;
                    Application.Current.Dispatcher.Invoke(() => new CandyWebPlayControl(result, true).Show());
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 加载更多检索结果
        /// </summary>
        private void OnLoadMoreSearch()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await CartFactory.Car(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CartType = CartEnum.Search,
                            Search = new CartSearch
                            {
                                Keyword = this.Keyword
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ToMapest<List<CartInitElementResult>>().ForEach(InitResult.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void ErrorNotify(string input = "") =>
                    Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(input.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : input).Show());
        #endregion

        #region 命令
        [RelayCommand]
        public void Collect(CartInitElementResult input) 
        {
            Service.Insert(input.ToMapest<AnimeModel>());
            CollectResult = new(Service.QueryAll());
        }

        [RelayCommand]
        public void Remove(Guid id)
        {
            Service.Remove(id);
            CollectResult = new(Service.QueryAll());
        }

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
            }
        }

        [RelayCommand]
        public void Close()
        {
            NavVisible = Visibility.Hidden;
            DetailResult = null;
        }

        [RelayCommand]
        public void Scroll(ScrollChangedEventArgs obj)
        {
            if (this.Keyword.IsNullOrEmpty())
            {
                if (Page <= Total && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    Page += 1;
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
        }

        [RelayCommand]
        public void Detail(string route)
        {
            NavVisible = Visibility.Visible;
            Route = route;
            OnDetail();
        }

        [RelayCommand]
        public void Watch(CartDetailElementResult element) => OnPlay(element.Play);
        #endregion

        #region ExternalCalls
        /// <summary>
        /// 检索数据
        /// </summary>
        /// <param name="keyword"></param>
        private void SearchHandler(string keyword)
        {
            this.Keyword = keyword;
            SearchPage = 1;
            if (!this.Keyword.IsNullOrEmpty())
                OnSearch();
            else
                OnInit();
        }
        #endregion
    }
}
