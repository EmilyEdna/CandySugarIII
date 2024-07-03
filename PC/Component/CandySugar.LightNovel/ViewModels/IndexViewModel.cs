namespace CandySugar.LightNovel.ViewModels
{
    public partial class IndexViewModel : ObservableObject
    {
        public IndexViewModel()
        {
            GenericDelegate.SearchAction = new(SearchHandler);
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            ChapterVisibility = Visibility.Collapsed;
            WindowStateEvent();
            OnInit();
        }
        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
            {
                MarginThickness = new Thickness(0, 0, 60, 70);
                NavHeight = (NavHeight == 0 ? 350 : NavHeight) * 2.5;
            }
            else
            {
                MarginThickness = new Thickness(0, 0, 60, 15);
                NavHeight = 350;
            }
        }
        #endregion

        #region 字段
        private int CateTotal;
        private int CatePage = 1;
        private int SearchPage = 1;
        private int SearchTotal;
        private string CateRoute;
        private string Keyword;
        /// <summary>
        /// 操作类型 1:分类 2:查询
        /// </summary>
        private int HandleType = 1;
        public IndexView Views;
        #endregion

        #region 属性
        [ObservableProperty]
        private Dictionary<string, string> _MenuData;
        [ObservableProperty]
        private ObservableCollection<LovelCategoryElementResult> _Category;
        [ObservableProperty]
        private Thickness _MarginThickness;
        [ObservableProperty]
        private ObservableCollection<LovelViewResult> _ViewResult;
        [ObservableProperty]
        private Visibility _ChapterVisibility;
        [ObservableProperty]
        private double _NavHeight;
        #endregion

        #region 命令
        [RelayCommand]
        public void Active(object input)
        {
            HandleType = 1;
            Keyword = string.Empty;
            string Route = input.ToMapest<AnonymousWater>().SelectValue.AsString();
            if (!Route.IsNullOrEmpty())
            {
                CateRoute = Route;
                OnCategory();
            }
        }
        [RelayCommand]
        public void Scroll(ScrollChangedEventArgs obj)
        {
            if (HandleType == 1)
                if (CatePage <= CateTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    CatePage += 1;
                    OnLoadMoreCategory();
                }
            if (HandleType == 2)
                if (SearchPage <= SearchTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    SearchPage += 1;
                    OnLoadMoreSearch();
                }
        }
        [RelayCommand]
        public void Chapter(string Route) => OnChapter(Route);
        [RelayCommand]
        public void Close()
        {
            ChapterVisibility = Visibility.Collapsed;
            ViewResult = [];
        }
        [RelayCommand]
        public void View(LovelViewResult view)
        {
            if (view.IsDown)
            {
                new ScreenNotifyView("后台下载中请稍后!").Show();
                OnDownload(view.ChapterRoute, view.BookName);
            }
            else
            {
                Module.Param = view.ChapterRoute;
                ((MainViewModel)Views.FindParent<UserControl>("Main").DataContext).Changed(true);
            }
        }
        #endregion

        #region 方法
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
                    var result = (await LovelFactory.Lovel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            LovelType = LovelEnum.Init,
                            Login = new()
                        };
                    }).RunsAsync()).InitResults;
                    MenuData = result.ToDictionary(t => t.Name, t => t.Route);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 初始化分类
        /// </summary>
        private void OnCategory()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await LovelFactory.Lovel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            LovelType = LovelEnum.Category,
                            Category = new LovelCategory
                            {
                                Page = 1,
                                Route = CateRoute
                            }
                        };
                    }).RunsAsync()).CategoryResult;
                    CateTotal = result.Total;
                    Category = new ObservableCollection<LovelCategoryElementResult>(result.ElementResults);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 加载更多分类
        /// </summary>
        private void OnLoadMoreCategory()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await LovelFactory.Lovel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            LovelType = LovelEnum.Category,
                            Category = new LovelCategory
                            {
                                Page = CatePage,
                                Route = CateRoute
                            }
                        };
                    }).RunsAsync()).CategoryResult;
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(Category.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 初始化搜索
        /// </summary>
        private void OnSearch()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await LovelFactory.Lovel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            LovelType = LovelEnum.Search,
                            Search = new LovelSearch
                            {
                                Page = 1,
                                SearchType = LovelSearchEnum.ArticleName,
                                KeyWord = Keyword
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    SearchTotal = result.Total;
                    var info = result.ElementResults.ToMapest<List<LovelCategoryElementResult>>();
                    Category = new(info);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 加载更多搜索
        /// </summary>
        private void OnLoadMoreSearch()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await LovelFactory.Lovel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            LovelType = LovelEnum.Search,
                            Search = new LovelSearch
                            {
                                Page = SearchPage,
                                SearchType = LovelSearchEnum.ArticleName,
                                KeyWord = Keyword
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ToMapest<List<LovelCategoryElementResult>>().ForEach(Category.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 初始化章节
        /// </summary>
        /// <param name="ChapterRoute"></param>
        private void OnChapter(string ChapterRoute)
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var ChapterResult = (await LovelFactory.Lovel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            LovelType = LovelEnum.Detail,
                            Detail = new LovelDetail
                            {
                                Route = ChapterRoute
                            }
                        };
                    }).RunsAsync()).DetailResult;
                    await Task.Delay(1000);
                    var result = (await LovelFactory.Lovel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            LovelType = LovelEnum.View,
                            View = new LovelView
                            {
                                Route = ChapterResult.Route
                            }
                        };
                    }).RunsAsync()).ViewResult;
                    ViewResult = new ObservableCollection<LovelViewResult>(result);
                    ChapterVisibility = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 初始化后台下载
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bookName"></param>
        private void OnDownload(string id, string bookName)
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await LovelFactory.Lovel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            LovelType = LovelEnum.Download,
                            Down = new LovelDown
                            {
                                BookName = bookName,
                                UId = id.AsInt()
                            }
                        };
                    }).RunsAsync()).DownResult.Bytes;
                    result.FileCreate(bookName, FileTypes.Txt, "LightNovel", (catalog, fileName) =>
                    {
                        new ScreenDownNofityView(CommonHelper.DownloadFinishInformation, catalog).Show();
                    });
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void ErrorNotify()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new ScreenNotifyView(CommonHelper.ComponentErrorInformation).Show();
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
            HandleType = 2;
            SearchPage = 1;
            Keyword = keyword;
            OnSearch();
        }
        #endregion
    }
}
