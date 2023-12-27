using System.IO;
using XExten.Advance.JsonDbFramework;

namespace CandySugar.Axgle.ViewModels
{
    public class IndexViewModel : PropertyChangedBase
    {
        private object LockObject = new object();
        private JsonDbHandle<AxgleCategoryElementResult> JsonHandler;
        private string DbPath = Path.Combine(CommonHelper.DownloadPath, "Axgle", $"Axgle.{FileTypes.Dat}");
        public IndexViewModel()
        {
            Title = ["常规", "收藏"];
            GenericDelegate.SearchAction = new(SearchHandler);
            JsonHandler = new JsonDbContext(DbPath).LoadInMemory<AxgleCategoryElementResult>();
            var LocalDATA = JsonHandler.GetAll();
            CollectResult = new ObservableCollection<AxgleCategoryElementResult>();
            if (LocalDATA != null)
            {
                LocalDATA.ForEach(CollectResult.Add);
            }
            OnInit();
        }

        #region Field
        private int CId;
        private int Total;
        private int PageIndex;
        private string Keyword;
        #endregion


        #region Property
        private ObservableCollection<string> _Title;
        public ObservableCollection<string> Title
        {
            get => _Title;
            set => SetAndNotify(ref _Title, value);
        }

        private ObservableCollection<AxgleInitResult> _InitResult;
        public ObservableCollection<AxgleInitResult> InitResult
        {
            get => _InitResult;
            set => SetAndNotify(ref _InitResult, value);
        }
        private ObservableCollection<AxgleCategoryElementResult> _CateResult;
        public ObservableCollection<AxgleCategoryElementResult> CateResult
        {
            get => _CateResult;
            set => SetAndNotify(ref _CateResult, value);
        }

        private ObservableCollection<AxgleCategoryElementResult> _CollectResult;
        public ObservableCollection<AxgleCategoryElementResult> CollectResult
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
        public void WatchCommand(AxgleCategoryElementResult element)
        {
            OnDetail(element.Play);
        }
        /// <summary>
        /// 激活模块
        /// </summary>
        /// <param name="cid"></param>
        public void ActiveCommand(string cid)
        {
            PageIndex = 0;
            CId = cid.AsInt();
            this.Keyword = string.Empty;
            OnCategory();
        }
        /// <summary>
        /// 收藏
        /// </summary>
        /// <param name="element"></param>
        public void CollectCommand(AxgleCategoryElementResult element) 
        {
            CollectResult.Add(element);
            JsonHandler.Insert(element).ExuteInsert().SaveChange(); ;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="element"></param>
        public void RemoveCommand(AxgleCategoryElementResult element)
        {
            CollectResult.Remove(element);
            JsonHandler.Delete(element).ExuteInsert().SaveChange();
        }
        /// <summary>
        /// 切换功能
        /// </summary>
        public RelayCommand<object> ChangedCommand => new(item => {
            var Target = ((CandyToggleItem)item);
            if (Target.FindParent<UserControl>() is IndexView View)
            {
                if (Target.Tag.ToString().AsInt() == 0)
                    View.AnimeX1.Begin();
                else
                    View.AnimeX2.Begin();
            }
        });
        /// <summary>
        /// 加载更多
        /// </summary>
        public RelayCommand<ScrollChangedEventArgs> ScrollCommand => new((obj) =>
        {
            if (PageIndex <= Total && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
            {
                PageIndex += 1;
                if (this.Keyword.IsNullOrEmpty())
                    OnLoadMoreCategory();
                else
                    OnLoadMoreSearch();
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
                    var res = (await AxgleFactory.Axgle(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            AxgleType = AxgleEnum.Init,
                        };
                    }).RunsAsync()).InitResults;
                    InitResult = new ObservableCollection<AxgleInitResult>(res);
                    // 这一句很关键，开启集合的异步访问支持
                    BindingOperations.EnableCollectionSynchronization(InitResult, LockObject);
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
                    var res = (await AxgleFactory.Axgle(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            AxgleType = AxgleEnum.Search,
                            Search = new AxgleSearch
                            {
                                KeyWord = this.Keyword,
                                Page = PageIndex
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    Total = res.Total;
                    CateResult = new ObservableCollection<AxgleCategoryElementResult>(res.ElementResult.ToMapest<List<AxgleCategoryElementResult>>());
                    // 这一句很关键，开启集合的异步访问支持
                    BindingOperations.EnableCollectionSynchronization(CateResult, LockObject);
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
                    var res = (await AxgleFactory.Axgle(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            AxgleType = AxgleEnum.Search,
                            Search = new AxgleSearch
                            {
                                KeyWord = this.Keyword,
                                Page = PageIndex
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    res.ElementResult.ToMapest<List<AxgleCategoryElementResult>>().ForEach(CateResult.Add);
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
                    var res = (await AxgleFactory.Axgle(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            AxgleType = AxgleEnum.Category,
                            Category = new AxgleCategory
                            {
                                Desc = AxgleDescEnum.MostViewed,
                                CId = CId,
                                Page = PageIndex
                            }
                        };
                    }).RunsAsync()).CategoryResult;
                    Total = res.Total;
                    CateResult = new ObservableCollection<AxgleCategoryElementResult>(res.ElementResult);
                    // 这一句很关键，开启集合的异步访问支持
                    BindingOperations.EnableCollectionSynchronization(CateResult, LockObject);
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
                    var res = (await AxgleFactory.Axgle(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            AxgleType = AxgleEnum.Category,
                            Category = new AxgleCategory
                            {
                                Desc = AxgleDescEnum.MostViewed,
                                CId = CId,
                                Page = PageIndex
                            }
                        };
                    }).RunsAsync()).CategoryResult;
                    res.ElementResult.ForEach(CateResult.Add);
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
            Task.Run(async () => {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var res = (await AxgleFactory.Axgle(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            AxgleType = AxgleEnum.Detail,
                           Detail = new AxgleDetail { 
                            FrameURL = input
                           }
                        };
                    }).RunsAsync()).DetailResult.Route;
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
            OnSearch();
        }
        #endregion
    }
}
