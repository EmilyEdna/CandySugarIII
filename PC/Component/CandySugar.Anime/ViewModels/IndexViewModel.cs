using CandySugar.Com.Controls.ExtenControls;
using CommunityToolkit.Mvvm.Input;

namespace CandySugar.Anime.ViewModels
{
    public partial class IndexViewModel : ObservableObject
    {

        public IndexViewModel()
        {
            OnInit();
            WindowStateEvent();
            CollectVisibility= Visibility.Collapsed;
            GenericDelegate.SearchAction = new(SearchHandler);
            GenericDelegate.WindowStateEvent += WindowStateEvent;
        }

        #region 字段
        private int Total;
        private int PageIndex = 1;
        private string Route;
        private string Keyword;
        private int SearchPageIndex = 1;
        private int SearchTotal;
        #endregion

        #region 属性
        [ObservableProperty]
        private ObservableCollection<CartInitElementResult> _InitResult;
        [ObservableProperty]
        private CartDetailRootResult _DetailResult;
        [ObservableProperty]
        private double _NavHeight;
        [ObservableProperty]
        private Visibility _CollectVisibility;
        #endregion

        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
            {
                NavHeight = (NavHeight == 0 ? 350 : NavHeight) * 2.5;
            }
            else
            {
                NavHeight = 350;
            }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化
        /// </summary>
        private async void OnInit()
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
                InitResult = new(result.ElementResults);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "");
                ErrorNotify();
            }
        }
        /// <summary>
        /// 初始化加载更多
        /// </summary>
        private async void OnLoadMoreInit()
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

                result.ElementResults.ForEach(InitResult.Add);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "");
                ErrorNotify();
            }
        }
        /// <summary>
        /// 详情
        /// </summary>
        private async void OnDetail()
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
        }
        /// <summary>
        /// 搜索
        /// </summary>
        private async void OnSearch()
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
                InitResult = new(result.ElementResults.ToMapest<List<CartInitElementResult>>());
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "");
                ErrorNotify();
            }
        }
        /// <summary>
        /// 获取真实播放地址
        /// </summary>
        private async void OnPlay(string args)
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
                            Route = args
                        }
                    };
                }).RunsAsync()).PlayResult.PlayRoute;
                Application.Current.Dispatcher.Invoke(() => new CandyWebPlayControl(result).Show());
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "");
                ErrorNotify();
            }
        }
        /// <summary>
        /// 加载更多检索结果
        /// </summary>
        private async void OnLoadMoreSearch()
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
                result.ElementResults.ToMapest<List<CartInitElementResult>>().ForEach(InitResult.Add);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "");
                ErrorNotify();
            }
        }
        private void ErrorNotify(string input = "") =>
                    Application.Current.Dispatcher.Invoke(() => new ScreenNotifyView(input.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : input).Show());
        #endregion

        #region 命令
        [RelayCommand]
        public void Close()
        {
            CollectVisibility = Visibility.Collapsed;
            DetailResult = null;
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
                if (SearchPageIndex <= SearchTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    SearchPageIndex += 1;
                    OnLoadMoreSearch();
                }
            }
        }

        [RelayCommand]
        public void Detail(string route)
        {
            CollectVisibility = Visibility.Visible;
            Route = route;
            OnDetail();
        }

        [RelayCommand]
        public void Watch(CartDetailElementResult element) => OnPlay(element.Play);
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
            else
                OnInit();
        }
        #endregion
    }
}
