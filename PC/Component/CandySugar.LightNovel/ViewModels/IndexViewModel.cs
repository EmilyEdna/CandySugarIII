namespace CandySugar.LightNovel.ViewModels
{
    public class IndexViewModel : PropertyChangedBase
    {

        private object lockObject = new object();
        public IndexViewModel()
        {
            GenericDelegate.SearchAction = new(SearchHandler);
            OnInit();

        }

        #region Field
        private int InformationTotal;
        private string InformationRoute;
        private int InformationPageIndex = 1;
        private int SearchTotal;
        private string SearchKeyword;
        private int SearchPageIndex = 1;
        /// <summary>
        /// 操作类型 1:分类 2:查询
        /// </summary>
        private int HandleType = 1;
        /// <summary>
        /// 侧边栏开关状态 1 开 2关
        /// </summary>
        public int SliderStatus = 2;
        #endregion

        #region Property
        private ObservableCollection<LovelViewResult> _ViewResult;
        /// <summary>
        /// 章节结果
        /// </summary>
        public ObservableCollection<LovelViewResult> ViewResult
        {
            get => _ViewResult;
            set => SetAndNotify(ref _ViewResult, value);
        }
        private ObservableCollection<LovelInitResult> _MenuIndex;
        /// <summary>
        /// 分类菜单
        /// </summary>
        public ObservableCollection<LovelInitResult> MenuIndex
        {
            get => _MenuIndex;
            set => SetAndNotify(ref _MenuIndex, value);
        }
        private ObservableCollection<LovelCategoryElementResult> _InformationElement;
        /// <summary>
        /// 分类结果
        /// </summary>
        public ObservableCollection<LovelCategoryElementResult> InformationElement
        {
            get => _InformationElement;
            set => SetAndNotify(ref _InformationElement, value);
        }
        #endregion

        #region Command
        public void ViewCommand(LovelViewResult view)
        {
            if (view.IsDown)
            {
                new ScreenNotifyView("后台下载中请稍后!").Show();
                OnDownload(view.ChapterRoute, view.BookName);
            }
            else
                WeakReferenceMessenger.Default.Send(new MessageNotify
                {
                    NotifyType = NotifyType.ChangeControl,
                    ControlType = 2,
                    ControlParam= view.ChapterRoute
                });
        }
        public void ActiveCommand(string route)
        {
            HandleType = 1;
            InformationPageIndex = 1;
            InformationRoute = route;
            OnInitInformation();
        }
        public void ChapterCommand(string chapter)
        {
            if (SliderStatus == 1)
                WeakReferenceMessenger.Default.Send(new MessageNotify { SliderStatus = 2 });
            OnInitChapter(chapter);
        }
        public RelayCommand<ScrollChangedEventArgs> ScrollCommand => new((obj) =>
        {
            if (HandleType == 1)
                if (InformationPageIndex <= InformationTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    InformationPageIndex += 1;
                    OnLoadMoreInformation();
                }
            if (HandleType == 2)
                if (SearchPageIndex <= SearchTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    SearchPageIndex += 1;
                    OnLoadMoreSearch();
                }
        });
        #endregion

        #region Method
        /// <summary>
        /// 初始化
        /// </summary>
        private void OnInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await LovelFactory.Lovel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            LovelType = LovelEnum.Init,
                            Login = new()
                        };
                    }).RunsAsync()).InitResults;
                    MenuIndex = new ObservableCollection<LovelInitResult>(result);
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
        /// <param name="route"></param>
        private void OnInitInformation()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await LovelFactory.Lovel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            LovelType = LovelEnum.Category,
                            Category = new LovelCategory
                            {
                                Page = 1,
                                Route = InformationRoute
                            }
                        };
                    }).RunsAsync()).CategoryResult;
                    InformationTotal = result.Total;
                    InformationElement = new ObservableCollection<LovelCategoryElementResult>(result.ElementResults);
                    // 这一句很关键，开启集合的异步访问支持
                    BindingOperations.EnableCollectionSynchronization(InformationElement, lockObject);
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
        private void OnLoadMoreInformation()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await LovelFactory.Lovel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            LovelType = LovelEnum.Category,
                            Category = new LovelCategory
                            {
                                Page = InformationPageIndex,
                                Route = InformationRoute
                            }
                        };
                    }).RunsAsync()).CategoryResult;
                    lock (lockObject)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            result.ElementResults.ForEach(index =>
                            {
                                InformationElement.Add(index);
                            });
                        });
                    }
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
        private void OnInitSearch()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await LovelFactory.Lovel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            LovelType = LovelEnum.Search,
                            Search = new LovelSearch
                            {
                                Page = 1,
                                SearchType = LovelSearchEnum.ArticleName,
                                KeyWord = SearchKeyword
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    SearchTotal = result.Total;
                    InformationElement = new ObservableCollection<LovelCategoryElementResult>(result.ElementResults.ToMapest<List<LovelCategoryElementResult>>());
                    // 这一句很关键，开启集合的异步访问支持
                    BindingOperations.EnableCollectionSynchronization(InformationElement, lockObject);
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
                    var result = (await LovelFactory.Lovel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            LovelType = LovelEnum.Search,
                            Search = new LovelSearch
                            {
                                Page = SearchPageIndex,
                                SearchType = LovelSearchEnum.ArticleName,
                                KeyWord = SearchKeyword
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    lock (lockObject)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            result.ElementResults.ForEach(index =>
                            {
                                InformationElement.Add(index.ToMapest<LovelCategoryElementResult>());
                            });
                        });
                    }
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
        private void OnInitChapter(string ChapterRoute)
        {
            Task.Run(async () =>
            {
                try
                {
                    var ChapterResult = (await LovelFactory.Lovel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
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
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            LovelType = LovelEnum.View,
                            View = new LovelView
                            {
                                Route = ChapterResult.Route
                            }
                        };
                    }).RunsAsync()).ViewResult;
                    ViewResult = new ObservableCollection<LovelViewResult>(result);
                    WeakReferenceMessenger.Default.Send(new MessageNotify { SliderStatus = 1 });
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
                    var result = (await LovelFactory.Lovel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            LovelType = LovelEnum.Download,
                            Down = new LovelDown
                            {
                                BookName = bookName,
                                UId = id.AsInt()
                            }
                        };
                    }).RunsAsync()).DownResult.Bytes;
                    result.FileCreate(bookName, FileTypes.Txt, "LightNovel", (catalog,fileName) =>
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
            SearchPageIndex = 1;
            SearchKeyword = keyword;
            OnInitSearch();
        }
        #endregion
    }
}
