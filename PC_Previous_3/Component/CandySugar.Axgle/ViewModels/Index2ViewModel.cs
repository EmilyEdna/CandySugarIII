namespace CandySugar.Axgle.ViewModels
{
    public partial class Index2ViewModel : BasicObservableObject
    {
        public Index2ViewModel()
        {
            ModeType = ModeEnum.ReleaseDate;
            Close = Visibility.Collapsed;
            PlatformType = PlatformEnum.A24;
            Title = ["无码", "有码"];
            Service = IocDependency.Resolve<IService<AxgleModel>>();
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
            InitDict();
            OnInit();
        }

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

        #region 字段
        private int SearchTotal;
        private int SearchPage;
        private int InitPage;
        private int InitTotal;
        private string Keyword;
        private ModeEnum ModeType;
        private IService<AxgleModel> Service;
        private Dictionary<string, Dictionary<string,string>> TagDict;
        private PlatformEnum PlatformType;
        #endregion

        #region 属性
        [ObservableProperty]
        private Visibility _Close;
        [ObservableProperty]
        private string _Tag;
        [ObservableProperty]
        private Dictionary<string, ModeEnum> _TagEnumDict;
        [ObservableProperty]
        private Dictionary<string, string> _Plays;
        [ObservableProperty]
        private ObservableCollection<string> _Title;
        [ObservableProperty]
        private Dictionary<string,string> _Tags;
        [ObservableProperty]
        private ObservableCollection<MissElemetInitResult> _Results;
        [ObservableProperty]
        private ObservableCollection<AxgleModel> _CollectResult;
        [ObservableProperty]
        private ObservableCollection<MissRelatedElementResult> _Link;
        #endregion

        #region 方法

        private void InitDict()
        {
            TagEnumDict = new Dictionary<string, ModeEnum>();
            typeof(ModeEnum).GetEnumNames()
                .ForArrayEach<string>(item =>
                {
                    var Mode = Enum.Parse<ModeEnum>(item);
                    TagEnumDict.Add(Mode.ToDes(), Mode);
                });
        }

        private void ErrorNotify(string input = "")
            => CandyNotify.Error(input.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : input);

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
                                ModeType = ModeType,
                                Tag = Tag
                            },
                            PlatformType = PlatformType,
                            FuncType = FuncEnum.Init
                        };
                    }).RunsAsync()).InitResult;
                    TagDict ??= res.Tags;
                    InitTotal = res.Total;
                    Results = [.. res.ElementResults];
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
                                ModeType = ModeType,
                                Tag = Tag
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
            if (this.Keyword.IsNullOrEmpty())
                return;
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
                                Page = SearchPage,
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
                                Page = SearchPage,
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
                            PlatformType = PlatformType,
                            Play = new MissPlay
                            {
                                DecodeM3u8Key= ComponentBinding.OptionObjectModels.DecodeM3u8Key,
                                DecodeKey = ComponentBinding.OptionObjectModels.DecodeKey,
                                Route = input.Route
                            }
                        };
                    }).RunsAsync()).PlayResult;
                    Plays = result.Plays;
                    Link = [.. result.ElementResults];
                    this.Close = Visibility.Visible;

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
        public void Collect(object element)
        {
            var Model = element.ToMapest<AxgleModel>();
            Model.Platfrom = PlatformType.AsString();
            OnDetail(element.ToMapest<MissElemetInitResult>());
            while (this.Plays == null)
            {
                Task.Delay(10).Wait();
            }
            string Title = Model.Title;
            this.Plays.ForDicEach((Key, Value, Index) =>
            {
                Model.Title = $"{Title}-{Index + 1}";
                Model.Route = Value;
                Service.Insert(Model);
            });
            GenericDelegate.ChangeContentAction?.Invoke(string.Empty);
        }
        [RelayCommand]
        public void View(string input)
            => new CandyWebPlayControl(input, true).Show();
        [RelayCommand]
        public void Selected(string input)
        {
            this.InitPage = 1;
            this.Keyword = string.Empty;
            Tag = input;
            OnInit();
        }
        [RelayCommand]
        public void Watch(object input)
            => OnDetail(input.ToMapest<MissElemetInitResult>());
        [RelayCommand]
        public void DropChanged(ModeEnum input)
        {
            ModeType = input;
            if (this.Keyword.IsNullOrEmpty())
                OnInit();
            else
                OnSearch();
        }
        [RelayCommand]
        public void Changed(object item)
        {
            var Target = ((CandyToggleItem)item);
            var Temp = Target.Tag.ToString().AsInt();
            if (Temp == 0)
                Tags = TagDict.FirstOrDefault().Value;
            else
                Tags = TagDict.LastOrDefault().Value;
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
        public void Trash()
        {
            this.Link = [];
            this.Plays = [];
            this.Close = Visibility.Collapsed;
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
            this.SearchPage = 1;
            OnSearch();
        }
        #endregion
    }
}
