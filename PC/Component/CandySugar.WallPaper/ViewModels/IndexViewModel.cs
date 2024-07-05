using CandyControls;

namespace CandySugar.WallPaper.ViewModels
{
    public partial class IndexViewModel : BasicObservableObject
    {

        public IndexViewModel()
        {
            Builder = [];
            Platform = PlatformEnum.Wallhaven;
            Title = ["常规", "一般", "可疑", "收藏"];
            MenuData = new() { { "WallHaven", "1" }, { "Konachan", "2" }, { "下载选中", "3" }, { "删除选中", "4" }, { "无声相册", "5" }, { "音乐相册", "6" } };
            Service = IocDependency.Resolve<IService<WallModel>>();
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
        }

        #region
        private IService<WallModel> Service;
        private PlatformEnum Platform;
        private GradEnum Grad;
        private List<WallModel> Builder;
        private int InitTotal;
        private int InitPage;
        private int SearchPage;
        private int SearchTotal;
        private string Keyword;
        #endregion

        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
            {
                Cols = (int)(GlobalParam.MAXWidth / 320);
                MarginThickness = new Thickness(0, 0, 60, 70);
            }
            else
            {
                Cols = (int)(GlobalParam.MAXWidth / 320);
                MarginThickness = new Thickness(0, 0, 60, 15);
            }
            BorderHeight = GlobalParam.MAXHeight;
            BorderWidth = GlobalParam.MAXWidth;
        }
        #endregion

        #region 属性
        [ObservableProperty]
        private ObservableCollection<string> _Title;
        [ObservableProperty]
        private Dictionary<string, string> _MenuData;
        [ObservableProperty]
        private ObservableCollection<WallElementResult> _Result;
        [ObservableProperty]
        private ObservableCollection<WallModel> _CollectResult;
        #endregion

        #region 命令 
        [RelayCommand]
        public void Check(WallModel param) => Builder.Add(param);
        [RelayCommand]
        public void UnCheck(WallModel param)=> Builder.Remove(param);
        [RelayCommand]
        public void Changed(object item)
        {
            var Target = (CandyToggleItem)item;
            if (Target.FindParent<UserControl>() is IndexView View)
            {
                var Index = Target.Tag.ToString().AsInt();

                if (Index == 0)
                {
                    Grad = GradEnum.SFW;
                    View.ActiveAnime = 1;
                    View.AnimeX1.Begin();
                }
                if (Index == 1)
                {
                    Grad = GradEnum.Sketchy;
                    View.ActiveAnime = 2;
                    View.AnimeX2.Begin();
                }
                if (Index == 2)
                {
                    Grad = GradEnum.NSFW;
                    View.ActiveAnime = 3;
                    View.AnimeX3.Begin();
                }
                if (Index == 3)
                {
                    View.ActiveAnime = 4;
                    View.AnimeX4.Begin();
                }
            }
        }
        [RelayCommand]
        public void Active(object input)
        {
            var param = input.ToMapest<AnonymousWater>();
            var value = param.SelectValue.AsString().AsInt();
            if (value == 1)
            {
                Platform = PlatformEnum.Wallhaven;
                Result = [];
            }
            if (value == 2)
            {
                Platform = PlatformEnum.Konachan;
                Result = [];
            }
            if (Builder.Count <= 0) return;
        /*    if (value == 3)
                DownSelectPicture();
            if (value == 4)
                RemoveSelectPicture();
            if (value == 5)
                BuilderVideoPicture();
            if (value == 6)
                BuilderVideoAudioPicture();*/
        }
        [RelayCommand]
        public void Collect(WallElementResult element)
        {
            Service.Insert(element.ToMapest<WallModel>());
            CollectResult = new(Service.QueryAll());
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
        public void ChangeActive(int ActiveAnime)
        {
            this.InitPage = 1;
            this.Keyword = string.Empty;
            if (ActiveAnime != 4)
                OnInit();
            else
                CollectResult = new(Service.QueryAll());
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
                    var result = (await WallFactory.Wall(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            WallType = WallEnum.Init,
                            PlatformType = Platform,
                            Init = new WallInit
                            {
                                Grad = Grad,
                                Page = 1,
                            }
                        };
                    }).RunsAsync()).Result;
                    InitTotal = result.Total;
                    Result = new ObservableCollection<WallElementResult>(result.ElementResults);
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
                    var result = (await WallFactory.Wall(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            WallType = WallEnum.Init,
                            PlatformType = Platform,
                            Init = new WallInit
                            {
                                Grad = Grad,
                                Page = InitPage,
                            }
                        };
                    }).RunsAsync()).Result;
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(Result.Add));
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
                    var result = (await WallFactory.Wall(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            WallType = WallEnum.Search,
                            PlatformType = Platform,
                            Search = new WallSearch
                            {
                                Grad = Grad,
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
                    var result = (await WallFactory.Wall(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            WallType = WallEnum.Search,
                            PlatformType = Platform,
                            Search = new WallSearch
                            {
                                Grad = Grad,
                                Page = SearchPage,
                                Keyword = Keyword
                            }
                        };
                    }).RunsAsync()).Result;
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(Result.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        #endregion
    }
}
