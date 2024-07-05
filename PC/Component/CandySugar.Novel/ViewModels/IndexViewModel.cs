namespace CandySugar.Novel.ViewModels
{
    public partial class IndexViewModel : BasicObservableObject
    {
        public IndexViewModel()
        {
            GenericDelegate.SearchAction = new(SearchHandler);
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            NavVisible = Visibility.Collapsed;
            WindowStateEvent();
            OnInit();
        }

        #region 字段
        public IndexView Views;
        private string Keyword;
        private string CateRoute;
        /// <summary>
        /// 操作类型 1:分类 2:查询
        /// </summary>
        private int HandleType = 1;
        private int CateTotal;
        private int CatePageIndex = 1;
        private int SearchTotal;
        private int SearchPageIndex = 1;
        private int RootChapterTotal;
        private int RootChapterPageIndex = 1;
        private NovelDetailRootResult RootDetail;
        #endregion

        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
                MarginThickness = new Thickness(0, 0, 60, 70);
            else
                MarginThickness = new Thickness(0, 0, 60, 15);
            NavHeight = GlobalParam.NavHeight;
            NavWidth= GlobalParam.NavWidth;
        }
        #endregion

        #region 属性
        [ObservableProperty]
        private Dictionary<string, string> _MenuData;
        [ObservableProperty]
        private PlatformEnum _Platform;
        [ObservableProperty]
        private ObservableCollection<NovelSearchElementResult> _SearchResult;
        [ObservableProperty]
        private ObservableCollection<NovelCategoryElementResult> _CategoryResult;
        [ObservableProperty]
        private ObservableCollection<NovelDetailElementResult> _DetailResult;
        #endregion

        #region 方法
        private void OnSearch()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await NovelFactory.Novel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            PlatformType = PlatformEnum.TopPoint,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            NovelType = NovelEnum.Search,
                            Search = new NovelSearch { SearchKey = Keyword }
                        };
                    }).RunsAsync()).SearchResult;
                    SearchTotal = result.Total;
                    var Model = result.ElementResults.ToMapest<List<NovelCategoryElementResult>>();
                    CategoryResult = new ObservableCollection<NovelCategoryElementResult>(Model);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void OnInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var Result = (await NovelFactory.Novel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            PlatformType = PlatformEnum.TopPoint,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            NovelType = NovelEnum.Init
                        };
                    }).RunsAsync()).InitResult;
                    MenuData = Result.ElementResults.ToDictionary(t => t.Name, t => t.Route);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void OnCategory()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await NovelFactory.Novel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            PlatformType = PlatformEnum.TopPoint,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            NovelType = NovelEnum.Category,
                            Category = new NovelCategory
                            {
                                Route = CateRoute
                            }
                        };
                    }).RunsAsync()).CategoryResult;
                    CateTotal = result.Total;
                    CategoryResult = new ObservableCollection<NovelCategoryElementResult>(result.ElementResults);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void OnChapter(Dictionary<string, object> element)
        {
            RootChapterPageIndex = 1;
            var Key = element["Key1"].AsString().AsInt();
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    RootDetail = (await NovelFactory.Novel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            PlatformType = Key == 1 ? PlatformEnum.TopPoint : PlatformEnum.Pendown,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            NovelType = NovelEnum.Detail,
                            Detail = new NovelDetail
                            {
                                BookName = element["Key2"].AsString(),
                                Route = element["Key3"].AsString()
                            }
                        };
                    }).RunsAsync()).DetailResult;
                    RootChapterTotal = RootDetail.Total;
                    Platform = RootDetail.NovelPlatformType.Value;
                    DetailResult = new ObservableCollection<NovelDetailElementResult>(RootDetail.ElementResults);
                    NavVisible = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void OnLoadMoreCategory()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await NovelFactory.Novel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            PlatformType = PlatformEnum.TopPoint,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            NovelType = NovelEnum.Category,
                            Category = new NovelCategory
                            {
                                Page = CatePageIndex,
                                Route = CateRoute
                            }
                        };
                    }).RunsAsync()).CategoryResult;
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(CategoryResult.Add));
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
                    var result = (await NovelFactory.Novel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            PlatformType = PlatformEnum.TopPoint,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            NovelType = NovelEnum.Search,
                            Search = new NovelSearch
                            {
                                SearchKey = Keyword,
                                Page = SearchPageIndex
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    var Model = result.ElementResults.ToMapest<List<NovelCategoryElementResult>>();
                    Application.Current.Dispatcher.Invoke(() => Model.ForEach(CategoryResult.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private async void OnLoadMoreChapter()
        {
            try
            {
                var Proxy = Module.IocModule.Proxy;
                var result = (await NovelFactory.Novel(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        ProxyIP = Proxy.IP,
                        ProxyPort = Proxy.Port,
                        PlatformType = PlatformEnum.Pendown,
                        CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                        NovelType = NovelEnum.Chapter,
                        Chapter = new NovelChapter
                        {
                            BookCode = RootDetail.BookCode,
                            Page = RootChapterPageIndex
                        }
                    };
                }).RunsAsync()).DetailResult.ElementResults;
                Application.Current.Dispatcher.Invoke(() => result.ForEach(DetailResult.Add));
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "");
                ErrorNotify();
            }
        }
        private void ErrorNotify(string input = "") =>
                    Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(input.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : input).Show());
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
        public void Chapter(Dictionary<string, object> element) => OnChapter(element);

        [RelayCommand]
        public void View(Dictionary<string, object> element)
        {
            ContentDataModel Model = new ContentDataModel();
            Model.Platform = Enum.Parse<PlatformEnum>(element["Key1"].AsString());
            Model.Current = element["Key2"].AsString();
            Model.Chapters = DetailResult.ToList();
            Model.Index = DetailResult.ToList().FindIndex(0, t => t.Route == Model.Current);

            Module.Param = Model;
            ((MainViewModel)Views.FindParent<UserControl>("Main").DataContext).Changed(true);
        }

        [RelayCommand]
        public void Close()
        {
            NavVisible = Visibility.Collapsed;
            DetailResult = [];
        }

        [RelayCommand]
        public void ChapterScroll(ScrollChangedEventArgs obj)
        {
            if (!RootDetail.BookCode.IsNullOrEmpty() && RootDetail.NovelPlatformType == PlatformEnum.Pendown)
            {
                if (RootChapterPageIndex <= RootChapterTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    RootChapterPageIndex += 1;
                    Application.Current.Dispatcher.InvokeAsync(OnLoadMoreChapter);
                }
            }
        }

        [RelayCommand]
        public void Scroll(ScrollChangedEventArgs obj)
        {
            if (HandleType == 1)
                if (CatePageIndex <= CateTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    CatePageIndex += 1;
                    OnLoadMoreCategory();
                }
            if (HandleType == 2)
                if (SearchPageIndex <= SearchTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    SearchPageIndex += 1;
                    OnLoadMoreSearch();
                }
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
            SearchPageIndex = CatePageIndex = 1;
            HandleType = 2;
            if (!this.Keyword.IsNullOrEmpty())
                OnSearch();
            else
                OnInit();
        }
        #endregion
    }
}
