namespace CandySugar.NHViewer.ViewModels
{
    public partial class IndexViewModel : BasicObservableObject
    {
        public IndexViewModel()
        {
            Title = ["全部", "喜爱"];
            CollectResult = [];
            NavVisible = Visibility.Hidden;
            Service = IocDependency.Resolve<IService<NHentaiModel>>();
            Catalog = SyncStatic.CreateDir(Path.Combine(CommonHelper.DownloadPath, "NHentai"));
            GenericDelegate.SearchAction = new(SearchHandler);
            HttpSchedule.ReceiveAction += ReceiveProcess;
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
        }
        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
                Cols = (int)(GlobalParam.MAXWidth / 200);
            else
                Cols = 5;
            BorderWidth = GlobalParam.MAXWidth;
            BorderHeight = GlobalParam.MAXHeight;
            NavHeight = GlobalParam.NavHeight;
            NavWidth = GlobalParam.NavWidth;
        }
        private void ReceiveProcess(double item, double num)
        {
            if (IsPreview == true) return;
            if (item == double.Parse((100 / num).ToString("F2")))
                Counts += item;
            if (Math.Ceiling(Counts) >= 100)
            {
                Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(CommonHelper.DownloadFinishInformation,true, Catalog).Show());
                IsDown = false;
            }
        }
        #endregion

        #region 字段
        public IndexView Views;

        private bool IsPreview;
        private int Total;
        private int PageIndex;
        private string Keyword;
        private double Counts;
        private string Catalog;
        private bool IsDown;
        private IService<NHentaiModel> Service;
        #endregion

        #region 属性
        [ObservableProperty]
        private ObservableCollection<string> _Title;
        [ObservableProperty]
        private ObservableCollection<NHentaiModel> _CollectResult;
        [ObservableProperty]
        private ObservableCollection<NHentaiModel> _Results;
        [ObservableProperty]
        private NHentaiModel _Result;
        #endregion

        #region 命令
        [RelayCommand]
        public void Changed(object item)
        {
            var Target = ((CandyToggleItem)item);
            if (Target.FindParent<UserControl>() is IndexView View)
            {
                var Index = Target.Tag.ToString().AsInt();

                if (Index == 0)
                {
                    View.ActiveAnime = 1;
                    View.AnimeX1.Begin();
                }
                if (Index == 1)
                {
                    View.ActiveAnime = 2;
                    View.AnimeX2.Begin();
                }
            }
        }
        [RelayCommand]
        public void Scroll(ScrollChangedEventArgs obj)
        {
            if (this.Keyword.IsNullOrEmpty())
            {
                if (PageIndex <= Total && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    PageIndex += 1;
                    OnLoadMoreInit();
                }
            }
            else
            {
                if (PageIndex <= Total && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    PageIndex += 1;
                    OnLoadMoreSearch();
                }
            }
        }
        [RelayCommand]
        public void Collect(NHentaiModel input)
        {
            Service.Insert(input);
            CollectResult = new(Service.QueryAll());
        }
        [RelayCommand]
        public void Remove(Guid id)
        {
            Service.Remove(id);
            CollectResult = new(Service.QueryAll());
        }
        [RelayCommand]
        public void Close()
        {
            NavVisible = Visibility.Hidden;
        }
        [RelayCommand]
        public void Watch(NHentaiModel input)
        {
            Result = input;
            NavVisible = Visibility.Visible;
        }
        [RelayCommand]
        public void View()
        {
            IsPreview = true;
            Module.Param = Result.OriginImages;
            ((MainViewModel)Views.FindParent<UserControl>("Main").DataContext).NChanged(true);
        }
        [RelayCommand]
        public void DownCommand()
        {
            ErrorNotify(CommonHelper.DownloadWait);
            Download();
        }
        #endregion

        #region 方法
        private async void Download()
        {
            if (Result != null && IsDown == false)
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                for (int index = 0; index < Result.ImageType.Count; index++)
                {
                    var fullName = Path.Combine(Catalog, Result.Name, $"{index + 1}.{Result.ImageType[index]}");

                    data.Add(fullName, Result.OriginImages[index]);
                }
                await HttpSchedule.HttpDownload(data);
                IsDown = true;
                IsPreview = false;
            }
        }

        public void ChangeActive(int ActiveAnime)
        {
            PageIndex = 1;
            Keyword = string.Empty;
            if (ActiveAnime == 1)
                OnInit();
            else
                CollectResult = new(Service.QueryAll());
        }

        public async void OnInit()
        {
            await Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await PandaFactory.Panda(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            PandaType = PandaEnum.Init,
                            PlatformType = PlatformEnum.NH,
                            Init = new PandaInit()
                        };
                    }).RunsAsync()).InitResult;
                    Total = result.TotalPage;
                    var data = result.NResults.ToMapest<List<NHentaiModel>>();
                    Results = new(data);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        public async void OnLoadMoreInit()
        {
            await Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await PandaFactory.Panda(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            PandaType = PandaEnum.Init,
                            PlatformType= PlatformEnum.NH,
                            Init = new PandaInit
                            {
                                Page = PageIndex
                            }
                        };
                    }).RunsAsync()).InitResult;
                    Application.Current.Dispatcher.Invoke(() => result.NResults.ToMapest<List<NHentaiModel>>().ForEach(Results.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        public async void OnitSearch()
        {
            await Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await PandaFactory.Panda(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            PlatformType = PlatformEnum.NH,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            PandaType = PandaEnum.Search,
                            Search = new PandaSearch
                            {
                                Keyword = Keyword
                            }
                        };
                    }).RunsAsync()).InitResult;
                    Total = result.TotalPage;
                    Results = new ObservableCollection<NHentaiModel>(result.NResults.ToMapest<List<NHentaiModel>>());
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        public async void OnLoadMoreSearch()
        {
            await Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await PandaFactory.Panda(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            PlatformType = PlatformEnum.NH,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            PandaType = PandaEnum.Search,
                            Search = new PandaSearch
                            {
                                Keyword = Keyword,
                                Page = PageIndex
                            }
                        };
                    }).RunsAsync()).InitResult;
                    Application.Current.Dispatcher.Invoke(() => result.NResults.ToMapest<List<NHentaiModel>>().ForEach(Results.Add));
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

        #region ExternalCalls
        /// <summary>
        /// 检索数据
        /// </summary>
        /// <param name="keyword"></param>
        private void SearchHandler(string keyword)
        {
            this.Keyword = keyword;
            PageIndex = 1;
            if (!this.Keyword.IsNullOrEmpty()) OnitSearch();
            else OnInit();
        }
        #endregion
    }
}
