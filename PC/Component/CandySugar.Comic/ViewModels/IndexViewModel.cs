using CandyControls;
using CandySugar.Com.Library.DownPace;
using System.Collections.Generic;
using System.IO;
using XExten.Advance.StaticFramework;

namespace CandySugar.Comic.ViewModels
{
    public partial class IndexViewModel : BasicObservableObject
    {
        public IndexViewModel()
        {
            Title = ["全部", "喜爱"];
            CollectResult = [];
            Watchs = [];
            NavVisible = Visibility.Hidden;
            Service = IocDependency.Resolve<IService<ComicModel>>();
            Catalog = SyncStatic.CreateDir(Path.Combine(CommonHelper.DownloadPath, "Comic"));
            GenericDelegate.SearchAction = new(SearchHandler);
            HttpSchedule.ReceiveAction += ReceiveProcess;
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
        }

        #region 字段
        public IndexView Views;
        private IService<ComicModel> Service;
        private int Total;
        private int PageIndex;
        private string Keyword;
        private string Catalog;
        private string Route;
        private double Counts;
        #endregion


        #region 事件
        private void ReceiveProcess(double item, double num)
        {
            if (item == double.Parse((100 / num).ToString("F2")))
                Counts += item;
            if (Math.Ceiling(Counts) >= 100)
            {
                Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(CommonHelper.DownloadFinishInformation, true, Catalog).Show());
            }
        }

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
        #endregion


        #region 属性
        [ObservableProperty]
        private ObservableCollection<string> _Title;
        [ObservableProperty]
        private ObservableCollection<ComicModel> _CollectResult;
        [ObservableProperty]
        private ObservableCollection<SearchElementResult> _Results;
        [ObservableProperty]
        private ObservableCollection<WatchInfo> _Watchs;
        #endregion

        #region 命令
        [RelayCommand]
        public void View()
        {
            Module.Param = Watchs;
            ((MainViewModel)Views.FindParent<UserControl>("Main").DataContext).Changed(true);
        }
        [RelayCommand]
        public void Down()
        {
            ErrorNotify(CommonHelper.DownloadWait);
            Download();
        }
        [RelayCommand]
        public void Close()
        {
            NavVisible = Visibility.Hidden;
            Watchs = [];
        }
        [RelayCommand]
        public void Remove(Guid id)
        {
            Service.Remove(id);
            CollectResult = new(Service.QueryAll());
        }
        [RelayCommand]
        public void Watch(string route)
        {
            this.Route = route;
            OnViewInit();
        }
        [RelayCommand]
        public void Changed(object item)
        {
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
        public void Scroll(ScrollChangedEventArgs obj)
        {
            if (PageIndex <= Total && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
            {
                PageIndex += 1;
                OnLoadMoreComicInit();
            }
        }
        [RelayCommand]
        public void Collect(SearchElementResult input)
        {
            Service.Insert(input.ToMapest<ComicModel>());
            CollectResult = new(Service.QueryAll());
        }
        #endregion

        #region 方法
        private async void Download()
        {
            if (Watchs.Count > 0)
            {
                Dictionary<string, string> data = new Dictionary<string, string>();

                Watchs.Select(t => t.Route).ForEnumerEach((item, index) =>
                {
                    var fullName = Path.Combine(Catalog, this.Results.FirstOrDefault(t => t.Route == this.Route).Name,$"{index + 1}.{Path.GetExtension(item)}");
                    data.Add(fullName, item);
                });
                await HttpSchedule.HttpDownload(data);
            }
        }

        private void ErrorNotify(string input = "") =>
        Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(input.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : input).Show());

        public void ChangeActive(int ActiveAnime)
        {
            if (ActiveAnime == 1 && Results == null)
                OnComicInit();
            else
                CollectResult = new(Service.QueryAll());
        }

        private void OnComicInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await ComicFactory.Comic(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            ComicType = ComicEnum.Search,
                            Search = new ComicSearch
                            {
                                Keyword = Keyword,
                                Page = 1
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    Total = result.Total;
                    Results = new ObservableCollection<SearchElementResult>(result.Results);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnViewInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await ComicFactory.Comic(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            ComicType = ComicEnum.View,
                            Preview = new ComicPreview
                            {
                                Route = Route
                            }
                        };
                    }).RunsAsync()).ViewResult;
                    result.Views.ForEnumerEach((item, index) =>
                    {
                        Watchs.Add(new WatchInfo
                        {
                            Index = index,
                            Preview = result.Previews[index],
                            Route = result.Views[index]
                        });
                    });
                    NavVisible = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnLoadMoreComicInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await ComicFactory.Comic(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            ComicType = ComicEnum.Search,
                            Search = new ComicSearch
                            {
                                Keyword = Keyword,
                                Page = PageIndex
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    Application.Current.Dispatcher.Invoke(() => result.Results.ForEach(Results.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
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
        private void SearchHandler(string keyword)
        {
            this.Keyword = keyword;
            PageIndex = 1;
            OnComicInit();
        }
        #endregion
    }
}
