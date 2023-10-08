using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Data;
using CommunityToolkit.Mvvm.Input;
using XExten.Advance.LinqFramework;

namespace CandySugar.Anime.ViewModels
{
    public class IndexViewModel : PropertyChangedBase
    {
        private object LockObject = new object();
        public IndexViewModel()
        {
            GenericDelegate.SearchAction = new(SearchHandler);
            if (this.Keyword.IsNullOrEmpty())
                OnInit();
        }

        #region Field
        private int Total;
        private int PageIndex = 1;
        private string Route;
        private string Keyword;
        private int SearchPageIndex = 1;
        private int SearchTotal;
        #endregion

        #region Property
        private ObservableCollection<CartInitElementResult> _InitResult;
        public ObservableCollection<CartInitElementResult> InitResult
        {
            get => _InitResult;
            set => SetAndNotify(ref _InitResult, value);
        }

        private CartDetailRootResult _DetailResult;
        public CartDetailRootResult DetailResult
        {
            get => _DetailResult;
            set => SetAndNotify(ref _DetailResult, value);
        }
        #endregion

        #region  Method
        /// <summary>
        /// 初始化
        /// </summary>
        private void OnInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await CartFactory.Car(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CartType = CartEnum.Init,
                            Init = new CartInit()
                        };
                    }).RunsAsync()).InitResult;
                    Total = result.Total;
                    InitResult = new ObservableCollection<CartInitElementResult>(result.ElementResults);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 初始化加载更多
        /// </summary>
        private void OnLoadMoreInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await CartFactory.Car(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CartType = CartEnum.Init,
                            Init = new CartInit
                            {
                                Page = PageIndex
                            }
                        };
                    }).RunsAsync()).InitResult;

                    BindingOperations.EnableCollectionSynchronization(InitResult, LockObject);
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(InitResult.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 详情
        /// </summary>
        private void OnDetail()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    DetailResult = (await CartFactory.Car(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CartType = CartEnum.Detail,
                            Detail = new CartDetail
                            {
                                Route = Route
                            }
                        };
                    }).RunsAsync()).DetailResult;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 搜索
        /// </summary>
        private void OnSearch()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await CartFactory.Car(opt =>
                     {
                         opt.RequestParam = new Input
                         {
                             ProxyIP = Proxy.IP,
                             ProxyPort = Proxy.Port,
                             CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                             CartType = CartEnum.Search,
                             Search = new CartSearch
                             {
                                 Keyword = this.Keyword
                             }
                         };
                     }).RunsAsync()).SearchResult;
                    SearchTotal = result.Total;
                    InitResult = new ObservableCollection<CartInitElementResult>(result.ElementResults.ToMapest<List<CartInitElementResult>>());
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 获取真实播放地址
        /// </summary>
        private void OnPlay(string args) {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await CartFactory.Car(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CartType = CartEnum.Play,
                            Play = new CartPlay
                            { 
                              Route= args
                            }
                        };
                    }).RunsAsync()).PlayResult.PlayRoute;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        WeakReferenceMessenger.Default.Send(new MessageNotify { ControlParam = false });
                        new ScreenWebPlayView
                        {
                            DataContext = new ScreenWebPlayViewModel
                            {
                                Route = result
                            }
                        }.Show();
                    });
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 加载更多检索结果
        /// </summary>
        private void OnLoadMoreSearch()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await CartFactory.Car(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CartType = CartEnum.Search,
                            Search = new CartSearch
                            {
                                Keyword = this.Keyword
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    BindingOperations.EnableCollectionSynchronization(InitResult, LockObject);
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ToMapest<List<CartInitElementResult>>().ForEach(InitResult.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void ErrorNotify()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new ScreenNotifyView(CommonHelper.ComponentErrorInformation).Show();
            });
        }
        #endregion

        #region Command
        /// <summary>
        /// 加载更多
        /// </summary>
        public RelayCommand<ScrollChangedEventArgs> ScrollCommand => new(obj =>
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
                if (SearchPageIndex <= SearchTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    SearchPageIndex += 1;
                    OnLoadMoreSearch();
                }
            }
        });

        public void DetailCommand(string route)
        {
            Route = route;
            OnDetail();
            WeakReferenceMessenger.Default.Send(new MessageNotify { ControlParam = true });
        }

        public void WatchCommand(CartDetailElementResult element) => OnPlay(element.Play);

        #endregion

        #region ExternalCalls
        /// <summary>
        /// 检索数据
        /// </summary>
        /// <param name="keyword"></param>
        private void SearchHandler(string keyword)
        {
            this.Keyword = keyword;
            SearchPageIndex = 1;
            if (!this.Keyword.IsNullOrEmpty())
                OnSearch();
        }
        #endregion
    }
}
