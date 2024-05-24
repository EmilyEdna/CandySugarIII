using System.DirectoryServices;

namespace CandySugar.NHViewer.ViewModels
{
    public class IndexViewModel : PropertyChangedBase
    {
        private object LockObject = new object();
        public IndexViewModel()
        {
            GenericDelegate.SearchAction = new(SearchHandler);
            Title = ["全部", "喜爱"];
            OnInit();
        }

        #region Field
        private int Total;
        private int PageIndex;
        private string Keyword;
        #endregion

        #region Property
        private ObservableCollection<string> _Title;
        public ObservableCollection<string> Title
        {
            get => _Title;
            set => SetAndNotify(ref _Title, value);
        }
        private ObservableCollection<InitElementResult> _Results;
        public ObservableCollection<InitElementResult> Results
        {
            get => _Results;
            set => SetAndNotify(ref _Results, value);
        }
        #endregion

        #region Command
        public RelayCommand<object> ChangedCommand => new((item) =>
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
        });

        /// <summary>
        /// 加载更多
        /// </summary>
        public RelayCommand<ScrollChangedEventArgs> ScrollCommand => new((obj) =>
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
        });

        public void ChangeCommand(int ActiveAnime)
        {

        }

        public void CollectCommand(InitElementResult input) 
        { 
        
        }

        #endregion

        #region Method
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
                            Init = new PandaInit()
                        };
                    }).RunsAsync()).InitResult;
                    Total = result.TotalPage;
                    Results = new ObservableCollection<InitElementResult>(result.Results);
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
                            Init = new PandaInit
                            {
                                Page = PageIndex
                            }
                        };
                    }).RunsAsync()).InitResult;
                    BindingOperations.EnableCollectionSynchronization(Results, LockObject);
                    Application.Current.Dispatcher.Invoke(() => result.Results.ForEach(Results.Add));
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
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            PandaType = PandaEnum.Search,
                            Search = new PandaSearch
                            {
                                Keyword = Keyword
                            }
                        };
                    }).RunsAsync()).InitResult;
                    Total = result.TotalPage;
                    Results = new ObservableCollection<InitElementResult>(result.Results);
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
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            PandaType = PandaEnum.Search,
                            Search = new PandaSearch
                            {
                                Keyword = Keyword,
                                Page = PageIndex
                            }
                        };
                    }).RunsAsync()).InitResult;
                    BindingOperations.EnableCollectionSynchronization(Results, LockObject);
                    Application.Current.Dispatcher.Invoke(() => result.Results.ForEach(Results.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }


        private void ErrorNotify(string Info = "")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new ScreenNotifyView(Info.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : Info).Show();
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
            if (!this.Keyword.IsNullOrEmpty()) OnitSearch();
            else OnInit();
        }
        #endregion
    }
}
