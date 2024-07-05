namespace CandySugar.Movie.ViewModels
{
    public partial class IndexViewModel : BasicObservableObject
    {
        public IndexViewModel()
        {
            Title = ["电影", "剧集"];
            Platform = PlatformEnum.Film;
            NavVisible = Visibility.Collapsed;
            InitKey = new Dictionary<string, string> { { "T", "" }, { "Y", "" }, { "P", "" }, { "C", "" } };
            WindowStateEvent();
            GenericDelegate.WindowStateEvent += WindowStateEvent;
        }

        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
                Cols = (int)(GlobalParam.MAXWidth/210);
            if (GlobalParam.WindowState == WindowState.Normal)
                Cols = 5;
            BorderWidth = GlobalParam.MAXWidth;
            BorderHeight = GlobalParam.MAXHeight;
            NavHeight = GlobalParam.NavHeight;
        }
        #endregion

        #region 字段
        public PlatformEnum Platform;
        private string Route;
        private int Total;
        private int PageIndex = 1;
        private Dictionary<string, string> InitKey;
        #endregion

        #region 属性
        [ObservableProperty]
        private ObservableCollection<string> _Title;
        [ObservableProperty]
        private ObservableCollection<string> _Year;
        [ObservableProperty]
        private ObservableCollection<string> _Plot;
        [ObservableProperty]
        private ObservableCollection<string> _Country;
        [ObservableProperty]
        private ObservableCollection<MovieInitElementResult> _InitResult;
        [ObservableProperty]
        private MovieDetailRootResult _DetailResult;
        #endregion

        #region 方法
        public void ChangeActive(int ActiveAnime)
        {
            InitKey = new Dictionary<string, string> { { "T", "" }, { "Y", "" }, { "P", "" }, { "C", "" } };
            if (ActiveAnime == 1) Platform = PlatformEnum.Film;
            else Platform = PlatformEnum.Video;
            OnInit();
        }

        private void OnInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await MovieFactory.Movie(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MovieType = MovieEnum.Init,
                            Init = new MovieInit
                            {
                                PlatformType = Platform,
                                InitKey = InitKey
                            }
                        };
                    }).RunsAsync()).InitResult;
                    if (InitKey["P"].IsNullOrEmpty())
                    {
                        Year = new ObservableCollection<string>(result.Year);
                        Plot = new ObservableCollection<string>(result.Plot);
                        Country = new ObservableCollection<string>(result.Country);
                        InitResult = new ObservableCollection<MovieInitElementResult>(result.ElementResults);
                        Total = result.Total;
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(InitResult.Add));
                    }
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void OnDetail()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    DetailResult = (await MovieFactory.Movie(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MovieType = MovieEnum.Detail,
                            Detail = new MovieDetail
                            {
                                Route = Route
                            }
                        };
                    }).RunsAsync()).DetailResult;
                    NavVisible = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void ErrorNotify(string input = "") =>
                                   Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(input.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : input).Show());
        #endregion

        #region 命令
        [RelayCommand]
        public void Changed(object item)
        {
            PageIndex = 1;
            InitKey["P"] = InitKey["T"] = InitKey["Y"] = InitKey["C"] = string.Empty;
            var Target = ((CandyToggleItem)item);
            if (Target.FindParent<UserControl>() is IndexView View)
            {
                if (Target.Tag.ToString().AsInt() == 0)
                {
                    View.ActiveAnime = 1;
                    View.AnimeX1.Begin();
                }
                else
                {
                    View.ActiveAnime = 2;
                    View.AnimeX2.Begin();
                }
            }
        }
        [RelayCommand]
        public void PlotChanged(object item)
        {
            var Target = ((CandyToggleItem)item);
            InitKey["T"] = Target.Content.ToString();
            InitKey["P"] = string.Empty;
            OnInit();
        }
        [RelayCommand]
        public void YearChanged(object item)
        {
            var Target = ((CandyToggleItem)item);
            InitKey["Y"] = Target.Content.ToString();
            InitKey["P"] = string.Empty;
            OnInit();
        }
        [RelayCommand]
        public void CountryChanged(object item)
        {
            var Target = ((CandyToggleItem)item);
            InitKey["C"] = Target.Content.ToString();
            InitKey["P"] = string.Empty;
            OnInit();
        }
        [RelayCommand]
        public void Scroll(ScrollChangedEventArgs obj)
        {
            if (PageIndex <= Total && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
            {
                PageIndex += 1;
                InitKey["P"] = PageIndex.AsString();
                OnInit();
            }
        }
        [RelayCommand]
        public void Detail(MovieInitElementResult input)
        {
            this.Route = input.Route;
            OnDetail();
        }
        [RelayCommand]
        public void Close()
        {
            NavVisible = Visibility.Collapsed;
            DetailResult = null;
        }
        [RelayCommand]
        public void Watch(string input) => new CandyWebPlayControl(input, false).Show();
        #endregion
    }
}
