namespace CandySugar.Manga.ViewModels
{
    public partial class IndexViewModel : BasicObservableObject
    {

        public IndexViewModel()
        {
            GenericDelegate.SearchAction = new(SearchHandler);
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            NavVisible = Visibility.Hidden;
            WindowStateEvent();
            OnInit();
        }

        #region 字段
        public IndexView Views;
        private string Keyword;
        private int SearchPage = 1;
        private int SearchTotal;
        private string CateRoute;
        private int CatePage = 1;
        private int CateTotal;
        #endregion

        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
                MarginThickness = new Thickness(0, 0, 60, 70);
            else
                MarginThickness = new Thickness(0, 0, 60, 60);
            BorderHeight = GlobalParam.MAXHeight;
            NavHeight = GlobalParam.NavHeight;
            NavWidth = GlobalParam.NavWidth;
        }
        #endregion

        #region 属性
        [ObservableProperty]
        private Dictionary<string, string> _MenuData;
        [ObservableProperty]
        private ObservableCollection<MangaCategoryElementResult> _CateResult;
        [ObservableProperty]
        private ObservableCollection<MangaChapterDetailResult> _Chapter;
        #endregion

        #region 方法
        private void OnInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = await MangaFactory.Manga(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MangaType = MangaEnum.Init,
                        };
                    }).RunsAsync();
                    MenuData = result.CateInitResults.ToDictionary(t => t.Type, t => t.Route);
                }
                catch (Exception ex)
                {
                    XLog.Fatal(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnCate()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = await MangaFactory.Manga(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MangaType = MangaEnum.Category,
                            Category = new MangaCategory
                            {
                                Page = 1,
                                Route = CateRoute
                            }
                        };
                    }).RunsAsync();
                    CateTotal = result.CategoryResult.Total;
                    CateResult = new ObservableCollection<MangaCategoryElementResult>(result.CategoryResult.ElementResults);
                }
                catch (Exception ex)
                {
                    XLog.Fatal(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnLoadMoreCate()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = await MangaFactory.Manga(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MangaType = MangaEnum.Category,
                            Category = new MangaCategory
                            {
                                Page = CatePage,
                                Route = CateRoute
                            }
                        };
                    }).RunsAsync();
                    Application.Current.Dispatcher.Invoke(() => result.CategoryResult.ElementResults.ForEach(CateResult.Add));
                }
                catch (Exception ex)
                {
                    XLog.Fatal(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnDetail(string input)
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = await MangaFactory.Manga(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MangaType = MangaEnum.Detail,
                            Detail = new MangaDetail
                            {
                                Route = input
                            }
                        };
                    }).RunsAsync();
                    Chapter = new ObservableCollection<MangaChapterDetailResult>(result.ChapterResults);
                    NavVisible = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    XLog.Fatal(ex, "");
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
                    var result = await MangaFactory.Manga(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MangaType = MangaEnum.Search,
                            Search = new MangaSearch
                            {
                                KeyWord = Keyword,
                                Page = 1
                            }
                        };
                    }).RunsAsync();
                    SearchTotal = result.SearchResult.Total;
                    var Model = result.SearchResult.ElementResults.ToMapest<List<MangaCategoryElementResult>>();
                    CateResult = new ObservableCollection<MangaCategoryElementResult>(Model);
                }
                catch (Exception ex)
                {
                    XLog.Fatal(ex, "");
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
                    var result = await MangaFactory.Manga(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MangaType = MangaEnum.Search,
                            Search = new MangaSearch
                            {
                                KeyWord = Keyword,
                                Page = SearchPage
                            }
                        };
                    }).RunsAsync();
                    var Model = result.SearchResult.ElementResults.ToMapest<List<MangaCategoryElementResult>>();
                    Application.Current.Dispatcher.Invoke(() => Model.ForEach(CateResult.Add));
                }
                catch (Exception ex)
                {
                    XLog.Fatal(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnContent(string input)
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = await MangaFactory.Manga(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MangaType = MangaEnum.Content,
                            Content = new MangaContent
                            {
                                Route = input
                            }
                        };
                    }).RunsAsync();
                    var bytes = await MangaFactory.Manga(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MangaType = MangaEnum.Download,
                            Down = new MangaBytes
                            {
                                CacheKey = result.ContentResult.CacheKey,
                                Route = result.ContentResult.Route
                            }
                        };
                    }).RunsAsync();

                    Module.Param = bytes.DwonResult.Bytes;
                    Application.Current.Dispatcher.Invoke(() => ((MainViewModel)Views.FindParent<UserControl>("Main").DataContext).Changed(true));
                }
                catch (Exception ex)
                {
                    XLog.Fatal(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void ErrorNotify(string input = "") =>
                            Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(input.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : input).Show());
        #endregion

        #region 命令
        [RelayCommand]
        public void Active(object input)
        {
            Keyword = string.Empty;
            CatePage = 1;
            var Route = input.ToMapest<AnonymousWater>().SelectValue.AsString();
            if (!Route.IsNullOrEmpty())
            {
                CateRoute = Route;
                OnCate();
            }
        }

        [RelayCommand]
        public void View(string input) => OnContent(input);

        [RelayCommand]
        public void Detail(string input) => OnDetail(input);


        [RelayCommand]
        public void Close()
        {
            NavVisible = Visibility.Hidden;
            Chapter = [];
        }

        [RelayCommand]
        public void Scroll(ScrollChangedEventArgs obj)
        {
            if (Keyword.IsNullOrEmpty())
            {
                if (CatePage <= CateTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    CatePage += 1;
                    OnLoadMoreCate();
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
        #endregion

        #region ExternalCalls
        /// <summary>
        /// 检索数据
        /// </summary>
        /// <param name="keyword"></param>
        private void SearchHandler(string keyword)
        {
            SearchPage = 1;
            Keyword = keyword;
            OnSearch();
        }
        #endregion
    }
}
