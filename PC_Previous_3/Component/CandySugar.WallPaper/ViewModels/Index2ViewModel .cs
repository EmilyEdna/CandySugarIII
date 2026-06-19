using CandyControls;

namespace CandySugar.WallPaper.ViewModels
{
    public partial class Index2ViewModel : BasicObservableObject
    {

        public Index2ViewModel()
        {
            PlatformType = PlatformEnum.Konachan;
            Title = ["常规", "一般", "可疑"];
            Service = IocDependency.Resolve<IService<WallModel>>();
            GenericDelegate.SearchAction = new(SearchHandler);
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
        }

        #region 字段
        private IService<WallModel> Service;
        private PlatformEnum PlatformType;
        private GradEnum GradType;
        private int InitTotal;
        private int InitPage;
        private int SearchPage;
        private int SearchTotal;
        private string Keyword;
        #endregion

        #region 事件
        private void WindowStateEvent()
        {
            BorderHeight = GlobalParam.MAXHeight;
            BorderWidth = GlobalParam.MAXWidth;
            if (GlobalParam.WindowState == WindowState.Maximized)
            {
                Cols = (int)(GlobalParam.MAXWidth / 360);
            }
            else
            {
                Cols = (int)(GlobalParam.MAXWidth / 360);
                BorderWidth -= 60;
            }
        }
        #endregion

        #region 属性
        [ObservableProperty]
        private ObservableCollection<string> _Title;
        [ObservableProperty]
        private ObservableCollection<WallElementResult> _Result;
        #endregion

        #region 命令 
        [RelayCommand]
        public void Changed(object item)
        {
            var Target = (CandyToggleItem)item;
            if (Target.FindParent<UserControl>() is Index2View View)
            {
                var Index = Target.Tag.ToString().AsInt();

                if (Index == 0)
                {
                    GradType = GradEnum.SFW;
                    View.AnimeX1.Begin();
                }
                if (Index == 1)
                {
                    GradType = GradEnum.Sketchy;
                    View.AnimeX2.Begin();
                }
                if (Index == 2)
                {
                    GradType = GradEnum.NSFW;
                    View.AnimeX3.Begin();
                }
            }
        }
        [RelayCommand]
        public void Collect(WallElementResult element)
        {
            Service.Insert(element.ToMapest<WallModel>());
            GenericDelegate.ChangeContentAction?.Invoke(string.Empty);
        }

        [RelayCommand]
        public void Scroll(ScrollChangedEventArgs obj)
        {
            if (Keyword.IsNullOrEmpty())
            {
                if (InitPage <= InitTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    InitPage += 1;
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
        #endregion

        #region 方法
        public void ChangeActive()
        {
            this.InitPage = 1;
            this.Keyword = string.Empty;
            OnInit();
        }

        private void ErrorNotify(string input = "")
            => CandyNotify.Error(input.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : input);

        private void OnInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await WallFactory.Wall(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            WallType = WallEnum.Init,
                            PlatformType = PlatformType,
                            Init = new WallInit
                            {
                                Grad = GradType,
                                Page = 1,
                            }
                        };
                    }).RunsAsync()).Result;
                    InitTotal = result.Total;
                    Result = new ObservableCollection<WallElementResult>(result.ElementResults);
                }
                catch (Exception ex)
                {
                    XLog.Fatal(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnLoadMoreInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await WallFactory.Wall(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            WallType = WallEnum.Init,
                            PlatformType = PlatformType,
                            Init = new WallInit
                            {
                                Grad = GradType,
                                Page = InitPage,
                            }
                        };
                    }).RunsAsync()).Result;
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(Result.Add));
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
                    var result = (await WallFactory.Wall(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            WallType = WallEnum.Search,
                            PlatformType = PlatformType,
                            Search = new WallSearch
                            {
                                Grad = GradType,
                                Page = 1,
                                Keyword = Keyword
                            }
                        };
                    }).RunsAsync()).Result;
                    SearchTotal = result.Total;
                    Result = new ObservableCollection<WallElementResult>(result.ElementResults);
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
                    var result = (await WallFactory.Wall(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            WallType = WallEnum.Search,
                            PlatformType = PlatformType,
                            Search = new WallSearch
                            {
                                Grad = GradType,
                                Page = SearchPage,
                                Keyword = Keyword
                            }
                        };
                    }).RunsAsync()).Result;
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(Result.Add));
                }
                catch (Exception ex)
                {
                    XLog.Fatal(ex, "");
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
        protected override void SearchHandler(string keyword)
        {
            this.Keyword = keyword;
            SearchPage = 1;
            OnSearch();
        }
        #endregion
    }
}
