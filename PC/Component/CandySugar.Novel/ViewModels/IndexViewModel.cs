using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CandySugar.Com.Library.FileWrite;
using CandySugar.Com.Options.ComponentGeneric;
using ImTools;


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
        #endregion

        #region Property
        private ObservableCollection<NovelSearchElementResult> _SearchResult;
        public ObservableCollection<NovelSearchElementResult> SearchResult
        {
            get => _SearchResult;
            set => SetAndNotify(ref _SearchResult, value);
        }
        private ObservableCollection<NovelInitResult> _InitResult;
        public ObservableCollection<NovelInitResult> InitResult
        {
            get => _InitResult;
            set => SetAndNotify(ref _InitResult, value);
        }
        private ObservableCollection<NovelCategoryElementResult> _CateElementResult;
        public ObservableCollection<NovelCategoryElementResult> CateElementResult
        {
            get => _CateElementResult;
            set => SetAndNotify(ref _CateElementResult, value);
        }
        private ObservableCollection<NovelDetailResult> _ChapterResult;
        public ObservableCollection<NovelDetailResult> ChapterResult
        {
            get => _ChapterResult;
            set => SetAndNotify(ref _ChapterResult, value);
        }

        #endregion

        #region Method
        private void OnInitSearch()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await NovelFactory.Novel(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            NovelType = NovelEnum.Search,
                            Search = new NovelSearch
                            {
                                Keyword = Keyword,
                                Page = 1
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    SearchTotal=result.Total;
                    SearchResult = new ObservableCollection<NovelSearchElementResult>(result.ElementResults);
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
                    var result = (await NovelFactory.Novel(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            NovelType = NovelEnum.Init
                        };
                    }).RunsAsync()).InitResults;
                    InitResult = new ObservableCollection<NovelInitResult>(result);
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
                    var result = (await NovelFactory.Novel(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            NovelType = NovelEnum.Category,
                            Category = new NovelCategory
                            {
                                Page = 1,
                                Route = CategoryRoute
                            }
                        };
                    }).RunsAsync()).CategoryResult;
                    CateTotal = result.Total;
                    CateElementResult = new ObservableCollection<NovelCategoryElementResult>(result.ElementResults);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void OnInitChapter(string route)
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await NovelFactory.Novel(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            NovelType = NovelEnum.Detail,
                            Detail = new NovelDetail
                            {
                                Route = route
                            }
                        };
                    }).RunsAsync()).DetailResults;
                    ChapterResult = new ObservableCollection<NovelDetailResult>(result);
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
                    var result = (await NovelFactory.Novel(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            NovelType = NovelEnum.Category,
                            Category = new NovelCategory
                            {
                                Page = 1,
                                Route = CategoryRoute
                            }
                        };
                    }).RunsAsync()).CategoryResult;
                    BindingOperations.EnableCollectionSynchronization(CateElementResult, LockObject);
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(CateElementResult.Add));
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
                    var result = (await NovelFactory.Novel(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            NovelType = NovelEnum.Search,
                            Search = new NovelSearch
                            {
                                Keyword = Keyword,
                                Page = SearchPageIndex
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    BindingOperations.EnableCollectionSynchronization(SearchResult, LockObject);
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(SearchResult.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
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

        /// <summary>
        /// 加载更多
        /// </summary>
        public RelayCommand<ScrollChangedEventArgs> ScrollCommand => new((obj) =>
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

        public void ChapterCommand(string route)
        {
            if (SliderStatus == 1)
                WeakReferenceMessenger.Default.Send(new MessageNotify { SliderStatus = 2 });
            OnInitChapter(route);
        }

        public void ViewCommand(string route)
        {
            WeakReferenceMessenger.Default.Send(new MessageNotify
            {
                NotifyType = NotifyType.ChangeControl,
                ControlType = 2,
                ControlParam = route
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
