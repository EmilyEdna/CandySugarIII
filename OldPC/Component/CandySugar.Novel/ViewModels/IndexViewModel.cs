﻿using CandySugar.Novel.Models;
using System.Collections.Generic;
using XExten.Advance.NetFramework.Enums;

namespace CandySugar.Novel.ViewModels
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
        /// <summary>
        /// 操作类型 1:分类 2:查询
        /// </summary>
        private int HandleType = 1;
        /// <summary>
        /// 侧边栏开关状态 1 开 2关
        /// </summary>
        public int SliderStatus = 2;
        private string Keyword;
        private string CategoryRoute;
        private int CateTotal;
        private int CatePageIndex = 1;
        private int SearchTotal;
        private int SearchPageIndex = 1;
        private NovelDetailRootResult RootDetail;
        private int RootChapterTotal;
        private int RootChapterPageIndex = 1;
        #endregion

        #region Property
        private PlatformEnum _Platform;
        public PlatformEnum Platform
        {
            get => _Platform;
            set => SetAndNotify(ref _Platform, value);
        }
        private ObservableCollection<NovelSearchElementResult> _SearchResult;
        public ObservableCollection<NovelSearchElementResult> SearchResult
        {
            get => _SearchResult;
            set => SetAndNotify(ref _SearchResult, value);
        }
        private NovelInitRootResult _InitResult;
        public NovelInitRootResult InitResult
        {
            get => _InitResult;
            set => SetAndNotify(ref _InitResult, value);
        }
        private ObservableCollection<NovelCategoryElementResult> _CategoryResult;
        public ObservableCollection<NovelCategoryElementResult> CategoryResult
        {
            get => _CategoryResult;
            set => SetAndNotify(ref _CategoryResult, value);
        }
        private ObservableCollection<NovelDetailElementResult> _DetailResult;
        public ObservableCollection<NovelDetailElementResult> DetailResult
        {
            get => _DetailResult;
            set => SetAndNotify(ref _DetailResult, value);
        }
        #endregion

        #region Method
        private void OnInitSearch()
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
                    InitResult = (await NovelFactory.Novel(opt =>
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
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void OnInitCategory()
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
                            Category = new NovelCategory { Route = CategoryRoute }
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
        private void OnInitChapter(Dictionary<string, object> element)
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
                    WeakReferenceMessenger.Default.Send(new MessageNotify { SliderStatus = 1 });
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
                                Route = CategoryRoute
                            }
                        };
                    }).RunsAsync()).CategoryResult;
                    BindingOperations.EnableCollectionSynchronization(CategoryResult, LockObject);
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
                    BindingOperations.EnableCollectionSynchronization(SearchResult, LockObject);
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
                BindingOperations.EnableCollectionSynchronization(DetailResult, LockObject);
                Application.Current.Dispatcher.Invoke(() => result.ForEach(DetailResult.Add));
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "");
                ErrorNotify();
            }
        }
        private void ErrorNotify(string Info = "")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new ScreenNotifyView(Info.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : Info).Show();
            });
        }
        #endregion

        #region Command

        public RelayCommand<ScrollChangedEventArgs> ChapterScrollCommand => new(obj =>
        {
            if (!RootDetail.BookCode.IsNullOrEmpty() && RootDetail.NovelPlatformType == PlatformEnum.Pendown)
            {
                if (RootChapterPageIndex <= RootChapterTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    RootChapterPageIndex += 1;
                    Application.Current.Dispatcher.InvokeAsync(OnLoadMoreChapter);
                }
            }
        });

        /// <summary>
        /// 加载更多
        /// </summary>
        public RelayCommand<ScrollChangedEventArgs> ScrollCommand => new(obj =>
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
        });

        public void ActiveCommand(string route)
        {
            HandleType = 1;
            CategoryRoute = route;
            OnInitCategory();
        }

        public void ChapterCommand(Dictionary<string, object> element)
        {
            if (SliderStatus == 1)
                WeakReferenceMessenger.Default.Send(new MessageNotify { SliderStatus = 2 });
            OnInitChapter(element);
        }

        public void ViewCommand(Dictionary<string, object> element)
        {
            ContentDataModel Model = new ContentDataModel();
            Model.Platform = Enum.Parse<PlatformEnum>(element["Key1"].AsString());
            Model.Current = element["Key2"].AsString();
            Model.Chapters = DetailResult.ToList();
            Model.Index = DetailResult.ToList().FindIndex(0, t => t.Route == Model.Current);

            WeakReferenceMessenger.Default.Send(new MessageNotify
            {
                NotifyType = NotifyType.ChangeControl,
                ControlType = 2,
                ControlParam = Model
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
            SearchPageIndex = CatePageIndex = 1;
            HandleType = 2;
            OnInitSearch();
        }
        #endregion
    }
}
