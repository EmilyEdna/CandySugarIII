using System.Runtime.InteropServices;
using XExten.Advance.NetFramework.Enums;

namespace CandySugar.Manga.ViewModels
{
    public class IndexViewModel : PropertyChangedBase
    {
        private object LockObject = new object();
        public IndexViewModel()
        {
            GenericDelegate.SearchAction = new(SearchHandler);
            OnInit();
        }

        #region Field
        private string Keyword=string.Empty;
        private int SearchPageIndex = 1;
        private int SearchTotal;
        private string CategoryRoute;
        private int CategoryPageIndex = 1;
        private int CategoryTotal;
        /// <summary>
        /// 侧边栏开关状态 1 开 2关
        /// </summary>
        public int SliderStatus = 2;
        #endregion

        #region Property
        private ObservableCollection<MangaInitCategoryResult> _Category;
        /// <summary>
        /// 分类
        /// </summary>
        public ObservableCollection<MangaInitCategoryResult> Category
        {
            get => _Category;
            set => SetAndNotify(ref _Category, value);
        }

        private ObservableCollection<MangaCategoryElementResult> _CateResult;
        /// <summary>
        /// 分类结果
        /// </summary>
        public ObservableCollection<MangaCategoryElementResult> CateResult
        {
            get => _CateResult;
            set => SetAndNotify(ref _CateResult, value);
        }

        private ObservableCollection<MangaChapterDetailResult> _Chapter;
        /// <summary>
        /// 章节
        /// </summary>
        public ObservableCollection<MangaChapterDetailResult> Chapter
        {
            get => _Chapter;
            set => SetAndNotify(ref _Chapter, value);
        }
        #endregion

        #region Command
        public RelayCommand<ScrollChangedEventArgs> ScrollCommand => new((obj) =>
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
        });
        public void ActiveCommand(string input)
        {
            Keyword = string.Empty;
            CategoryPageIndex = 1;
            CategoryRoute = input;
            OnCate();
        }

        public void DetailCommand(string input)
        {
            OnDetail(input);
        }

        public void ViewCommand(string input)
        {

        }
        #endregion

        #region Method
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
                    Category = new ObservableCollection<MangaInitCategoryResult>(result.CateInitResults);
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
                    BindingOperations.EnableCollectionSynchronization(CateResult, LockObject);
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
                    WeakReferenceMessenger.Default.Send(new MessageNotify { SliderStatus = 1 });
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
                    BindingOperations.EnableCollectionSynchronization(CateResult, LockObject);
                    Application.Current.Dispatcher.Invoke(() => Model.ForEach(CateResult.Add));
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
            SearchPageIndex = 1;
            Keyword = keyword;
            OnSearch();
        }
        #endregion
    }
}
