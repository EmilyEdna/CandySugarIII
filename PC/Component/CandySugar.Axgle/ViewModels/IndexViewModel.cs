using CandyControls;
using CandySugar.Com.Options.Anonymous;

namespace CandySugar.Axgle.ViewModels
{
    public partial class IndexViewModel : ObservableObject
    {
        public IndexViewModel()
        {
            PlatformType = PlatformEnum.Jav;
            Title = ["最新", "热门", "好评", "收藏"];
            MenuData = new() { { "Jav", "1" }, { "Skb", "2" } };
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
                NavHeight = 530;
                MarginThickness = new Thickness(0, 0, 15, 70);
            }
            else
            {
                NavHeight = 400;
                MarginThickness = new Thickness(0, 0, 15, 15);
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
        private PlatformEnum PlatformType;
        private IService<AxgleModel> Service;
        #endregion

        #region 属性
        [ObservableProperty]
        private double _NavHeight;
        [ObservableProperty]
        private Thickness _MarginThickness;
        [ObservableProperty]
        private ObservableCollection<string> _Title;
        [ObservableProperty]
        private Dictionary<string, string> _MenuData;
        [ObservableProperty]
        private ObservableCollection<JronElemetInitResult> _Results;
        [ObservableProperty]
        private ObservableCollection<AxgleModel> _CollectResult;

        #endregion

        #region 方法

        private void ErrorNotify(string input = "") =>
                   Application.Current.Dispatcher.Invoke(() => new ScreenNotifyView(input.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : input).Show());

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
                                ModeType = ModeType
                            },
                            PlatformType = PlatformType,
                            JronType = JronEnum.Init
                        };
                    }).RunsAsync()).InitResult;
                    InitTotal = res.Total;
                    Results = new(res.ElementResults);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
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
                                ModeType = ModeType
                            },
                            PlatformType = PlatformType,
                            JronType = JronEnum.Init
                        };
                    }).RunsAsync()).InitResult;
                    Application.Current.Dispatcher.Invoke(() => res.ElementResults.ForEach(Results.Add));
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
                                Page = SearchPage
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    SearchTotal = res.Total;
                    Results = new(res.ElementResults.ToMapest<List<JronElemetInitResult>>());
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
                                Page = SearchPage
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    Application.Current.Dispatcher.Invoke(() => res.ElementResults.ToMapest<List<JronElemetInitResult>>().ForEach(Results.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
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
                    }).RunsAsync()).PlayResult.Play;

                    Application.Current.Dispatcher.Invoke(() => new CandyWebPlayControl(result).Show());
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        #endregion

        #region 命令
        [RelayCommand]
        public void Active(object input)
        {
           var param = input.ToMapest<AnonymousWater>();
            if (param.SelectName == "Jav")
                PlatformType = PlatformEnum.Jav;
            else
                PlatformType = PlatformEnum.Skb;
            this.Keyword = string.Empty;
            InitPage = SearchPage = 1;
            Results = [];
        }
        [RelayCommand]
        public void Changed(object item)
        {
            var Target = ((CandyToggleItem)item);
            if (Target.FindParent<UserControl>() is IndexView View)
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
                else if (Temp == 2)
                {
                    ModeType = ModeEnum.Praised;
                    View.ActiveAnime = 3;
                    View.AnimeX3.Begin();
                }
                else
                {
                    View.ActiveAnime = 4;
                    View.AnimeX4.Begin();
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
        public void Collect(JronElemetInitResult element)
        {
            var Model = element.ToMapest<AxgleModel>();
            Model.Platfrom = PlatformType.AsString();
            Model.PId = Service.Insert(Model);
            CollectResult.Add(Model);
        }
        [RelayCommand]
        public void Watch(JronElemetInitResult element) => OnDetail(element);
        [RelayCommand]
        public void Remove(Guid id)
        {
            CollectResult.Remove(CollectResult.First(t => t.PId == id));
            Service.Remove(id);
        }
        [RelayCommand]
        public void Play(AxgleModel element) => OnDetail(element.ToMapest<JronElemetInitResult>());
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

