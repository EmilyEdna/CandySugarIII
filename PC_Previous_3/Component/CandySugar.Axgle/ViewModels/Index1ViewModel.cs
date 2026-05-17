namespace CandySugar.Axgle.ViewModels
{
    public partial class Index1ViewModel : BasicObservableObject
    {
        public Index1ViewModel()
        {
            CollectResult = [];
            PlatformType = PlatformEnum.Skb;
            Title = ["最新", "热门", "好评"];
            Service = IocDependency.Resolve<IService<AxgleModel>>();
            GenericDelegate.SearchAction = new(SearchHandler);
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();

        }

        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
            {
                Cols = (int)(GlobalParam.MAXWidth / 360);
                MarginThickness = new Thickness(0, 0, 25, 20);
            }
            else
            {
                Cols = (int)(GlobalParam.MAXWidth / 360);
                MarginThickness = new Thickness(0, 0, 25, 15);
            }
            BorderHeight = GlobalParam.MAXHeight;
            BorderWidth = GlobalParam.MAXWidth;
        }
        #endregion

        #region 字段
        private int SearchTotal;
        private int SearchPage;
        private int InitPage;
        private int InitTotal;
        private string Keyword;
        private ModeEnum ModeType;
        private IService<AxgleModel> Service;
        private PlatformEnum PlatformType;
        #endregion

        #region 属性
        [ObservableProperty]
        private ObservableCollection<string> _Title;
        [ObservableProperty]
        private Dictionary<string, string> _MenuData;
        [ObservableProperty]
        private ObservableCollection<MissElemetInitResult> _Results;
        [ObservableProperty]
        private ObservableCollection<AxgleModel> _CollectResult;

        #endregion

        #region 方法

        private void ErrorNotify(string input = "")
            => CandyNotify.Error(input.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : input);

        public void ChangeActive(int ActiveAnime)
        {
            this.Keyword = string.Empty;
            InitPage = 1;
            if (ActiveAnime != 4)
                OnInit();
            else
                CollectResult = new(Service.QueryAll());
        }

        private void OnInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var res = (await MissFactory.Miss(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            Init = new MissInit
                            {
                                Page = InitPage,
                                ModeType = ModeType
                            },
                            PlatformType = PlatformType,
                            FuncType = FuncEnum.Init
                        };
                    }).RunsAsync()).InitResult;
                    InitTotal = res.Total;
                    Results = new(res.ElementResults);
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
                    var res = (await MissFactory.Miss(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            Init = new MissInit
                            {
                                Page = InitPage,
                                ModeType = ModeType
                            },
                            PlatformType = PlatformType,
                            FuncType = FuncEnum.Init
                        };
                    }).RunsAsync()).InitResult;
                    Application.Current.Dispatcher.Invoke(() => res.ElementResults.ForEach(Results.Add));
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
                    var res = (await MissFactory.Miss(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            FuncType = FuncEnum.Search,
                            PlatformType = PlatformType,
                            Search = new MissSearch
                            {
                                Keyword = this.Keyword,
                                Page = SearchPage
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    SearchTotal = res.Total;
                    Results = new(res.ElementResults.ToMapest<List<MissElemetInitResult>>());
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
                    var res = (await MissFactory.Miss(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            FuncType = FuncEnum.Search,
                            PlatformType = PlatformType,
                            Search = new MissSearch
                            {
                                Keyword = this.Keyword,
                                Page = SearchPage
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    Application.Current.Dispatcher.Invoke(() => res.ElementResults.ToMapest<List<MissElemetInitResult>>().ForEach(Results.Add));
                }
                catch (Exception ex)
                {
                    XLog.Fatal(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnDetail(MissElemetInitResult input)
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await MissFactory.Miss(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            FuncType = FuncEnum.Detail,
                            PlatformType = input.Route.Contains("javbangers") ? PlatformEnum.Jav : PlatformEnum.Skb,
                            Play = new MissPlay
                            {
                                Route = input.Route
                            }
                        };
                    }).RunsAsync()).PlayResult.Play;

                    Application.Current.Dispatcher.Invoke(() => new CandyWebPlayControl(result, true,false).Show());
                }
                catch (Exception ex)
                {
                    XLog.Fatal(ex, "");
                    ErrorNotify();
                }
            });
        }
        #endregion

        #region 命令
        [RelayCommand]
        public void Changed(object item)
        {
            var Target = ((CandyToggleItem)item);
            if (Target.FindParent<UserControl>() is Index1View View)
            {
                var Temp = Target.Tag.ToString().AsInt();
                if (Temp == 0)
                {
                    ModeType = ModeEnum.Latest;
                    View.ActiveAnime = 1;
                    View.AnimeX1.Begin();
                }
                else if (Temp == 1)
                {
                    ModeType = ModeEnum.Hot;
                    View.ActiveAnime = 2;
                    View.AnimeX2.Begin();
                }
                else
                {
                    ModeType = ModeEnum.Praised;
                    View.ActiveAnime = 3;
                    View.AnimeX3.Begin();
                }
            }
        }
        [RelayCommand]
        public void Scroll(ScrollChangedEventArgs obj)
        {
            if (this.Keyword.IsNullOrEmpty())
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
        [RelayCommand]
        public void Collect(MissElemetInitResult element)
        {
            var Model = element.ToMapest<AxgleModel>();
            Model.Platfrom = PlatformType.AsString();
            Service.Insert(Model);
            CollectResult = new(Service.QueryAll());
        }
        [RelayCommand]
        public void Watch(MissElemetInitResult element)
          =>OnDetail(element);
        [RelayCommand]
        public void Remove(Guid id)
        {
            Service.Remove(id);
            CollectResult = new(Service.QueryAll());
        }
        [RelayCommand]
        public void Play(AxgleModel element)
        {
            if (element.Platfrom == "A24")
                Application.Current.Dispatcher.Invoke(() => new CandyWebPlayControl(element.Route, false,true).Show());
            else
                OnDetail(element.ToMapest<MissElemetInitResult>());
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
            this.SearchPage = 1;
            OnSearch();
        }
        #endregion
    }
}

