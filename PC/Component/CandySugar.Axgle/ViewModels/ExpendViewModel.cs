using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace CandySugar.Axgle.ViewModels
{
    public partial class ExpendViewModel : BasicObservableObject
    {
        public ExpendViewModel()
        {
            PlatformType = PlatformEnum.A24;
            ModeType = ModeEnum.ReleaseDate;
            Close = Visibility.Collapsed;
            Title = ["无码", "有码"];
            Service = IocDependency.Resolve<IService<AxgleModel>>();
            GenericDelegate.SearchAction = new(SearchHandler);
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
            InitDict();
            OnInit();
        }

        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
            {
                Cols = (int)(GlobalParam.MAXWidth / 360);
                MarginThickness = new Thickness(0, 0, 15, 20);
            }
            else
            {
                Cols = (int)(GlobalParam.MAXWidth / 360);
                MarginThickness = new Thickness(0, 0, 15, 15);
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
        private PlatformEnum PlatformType;
        private IService<AxgleModel> Service;
        private Dictionary<string, Dictionary<string,string>> TagDict;
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
        private ObservableCollection<JronElemetInitResult> _Results;
        [ObservableProperty]
        private ObservableCollection<AxgleModel> _CollectResult;
        [ObservableProperty]
        private ObservableCollection<JronRelatedElementResult> _Link;
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

        private void ErrorNotify(string input = "") =>
                   Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(input.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : input).Show());

        private void OnInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var res = (await JronFactory.Jron(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            Init = new JronInit
                            {
                                Page = InitPage,
                                ModeType = ModeType,
                                Tag = Tag
                            },
                            PlatformType = PlatformType,
                            JronType = JronEnum.Init
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
                    var Proxy = Module.IocModule.Proxy;
                    var res = (await JronFactory.Jron(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            Init = new JronInit
                            {
                                Page = InitPage,
                                ModeType = ModeType,
                                Tag = Tag
                            },
                            PlatformType = PlatformType,
                            JronType = JronEnum.Init
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
                    var Proxy = Module.IocModule.Proxy;
                    var res = (await JronFactory.Jron(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            JronType = JronEnum.Search,
                            PlatformType = PlatformType,
                            Search = new JronSearch
                            {
                                Keyword = this.Keyword,
                                Page = SearchPage,
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    SearchTotal = res.Total;
                    Results = new(res.ElementResults.ToMapest<List<JronElemetInitResult>>());
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
                    var res = (await JronFactory.Jron(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            JronType = JronEnum.Search,
                            PlatformType = PlatformType,
                            Search = new JronSearch
                            {
                                Keyword = this.Keyword,
                                Page = SearchPage,
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    Application.Current.Dispatcher.Invoke(() => res.ElementResults.ToMapest<List<JronElemetInitResult>>().ForEach(Results.Add));
                }
                catch (Exception ex)
                {
                    XLog.Fatal(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnDetail(JronElemetInitResult input)
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await JronFactory.Jron(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            JronType = JronEnum.Detail,
                            PlatformType = PlatformType,
                            Play = new JronPlay
                            {
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
            OnDetail(element.ToMapest<JronElemetInitResult>());
            while (this.Plays == null)
            {
                Task.Delay(10).Wait();
            }
            this.Plays.ForDicEach((Key, Value, Index) =>
            {
                Model.Title += $"-{Index + 1}";
                Model.Route = Value;
                Service.Insert(Model);
            });
        }
        [RelayCommand]
        public void View(string input)
            => new CandyWebPlayControl(input, true).Show();
        [RelayCommand]
        public void Back()
            => GenericDelegate.ChangeContentAction?.Invoke(string.Empty);
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
            => OnDetail(input.ToMapest<JronElemetInitResult>());
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
        private void SearchHandler(string keyword)
        {
            this.Keyword = keyword;
            this.SearchPage = 1;
            OnSearch();
        }
        #endregion
    }
}
