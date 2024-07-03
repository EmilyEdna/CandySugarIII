namespace CandySugar.Manga.ViewModels
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

        #region 字段
        public IndexView Views;
        private string Keyword;
        private int SearchPageIndex = 1;
        private int SearchTotal;
        private string CategoryRoute;
        private int CategoryPageIndex = 1;
        private int CategoryTotal;
        #endregion

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

        #region 属性
        [ObservableProperty]
        private Dictionary<string,string> _MenuData;
        [ObservableProperty]
        private Thickness _MarginThickness;
        [ObservableProperty]
        private Visibility _ChapterVisibility;
        [ObservableProperty]
        private double _NavHeight;
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
                    Log.Logger.Error(ex, "");
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
                                Route = CategoryRoute
                            }
                        };
                    }).RunsAsync();
                    CategoryTotal = result.CategoryResult.Total;
                    CateResult = new ObservableCollection<MangaCategoryElementResult>(result.CategoryResult.ElementResults);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
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
                                Page = CategoryPageIndex,
                                Route = CategoryRoute
                            }
                        };
                    }).RunsAsync();
                    Application.Current.Dispatcher.Invoke(() => result.CategoryResult.ElementResults.ForEach(CateResult.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
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
                    ChapterVisibility = Visibility.Visible;
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
                                Page = SearchPageIndex
                            }
                        };
                    }).RunsAsync();
                    var Model = result.SearchResult.ElementResults.ToMapest<List<MangaCategoryElementResult>>();
                    Application.Current.Dispatcher.Invoke(() => Model.ForEach(CateResult.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
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
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void ErrorNotify(string input = "") =>
                            Application.Current.Dispatcher.Invoke(() => new ScreenNotifyView(input.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : input).Show());
        #endregion

        #region 命令
        [RelayCommand]
        public void Active(string input)
        {
            Keyword = string.Empty;
            CategoryPageIndex = 1;
            CategoryRoute = input;
            OnCate();
        }

        [RelayCommand]
        public void View(string input) => OnContent(input);

        [RelayCommand]
        public void Detail(string input) => OnDetail(input);


        [RelayCommand]
        public void Close()
        {
            ChapterVisibility = Visibility.Collapsed;
            Chapter = [];
        }

        [RelayCommand]
        public void Scroll(ScrollChangedEventArgs obj)
        {
            if (Keyword.IsNullOrEmpty())
            {
                if (CategoryPageIndex <= CategoryTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    CategoryPageIndex += 1;
                    OnLoadMoreCate();
                }
            }
            else
            {
                if (SearchPageIndex <= SearchTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    SearchPageIndex += 1;
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
            SearchPageIndex = 1;
            Keyword = keyword;
            OnSearch();
        }
        #endregion
    }
}
