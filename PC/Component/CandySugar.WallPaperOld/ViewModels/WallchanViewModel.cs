using Input = Sdk.Component.Vip.Image.sdk.ViewModel.Input;
using QueryEnum = Sdk.Component.Vip.Wallkon.sdk.ViewModel.Enums.QueryEnum;

namespace CandySugar.WallPaper.ViewModels
{
    public class WallchanViewModel : PropertyChangedBase
    {
        private object LockObject = new object();
        public List<WallModel> Builder;
        private IService<WallModel> Service;
        public WallchanViewModel()
        {
            Builder = [];
            CollectResult = [];
            Title = ["常规", "一般", "可疑", "收藏"];
            GenericDelegate.SearchAction = new(SearchHandler);
            Service = IocDependency.Resolve<IService<WallModel>>();
            var LocalDATA = Service.QueryAll().Where(t => t.Platform == 2).ToList();
            LocalDATA?.ForEach(CollectResult.Add);
        }

        #region Field
        private int Limit = 12;
        private string Keyword;
        private int GeneralTotal;
        private int GeneralPageIndex = 1;
        private int OrdinaryTotal;
        private int OrdinaryPageIndex = 1;
        private int FishyTotal;
        private int FishyPageIndex = 1;
        /// <summary>
        /// 1：常规 2：一般 3：可疑 4：收藏
        /// </summary>
        private int ChangeType = 1;
        #endregion

        #region Property
        private ObservableCollection<string> _Title;
        public ObservableCollection<string> Title
        {
            get => _Title;
            set => SetAndNotify(ref _Title, value);
        }

        private ObservableCollection<WallkonElementResult> _GeneralResult;
        public ObservableCollection<WallkonElementResult> GeneralResult
        {
            get => _GeneralResult;
            set => SetAndNotify(ref _GeneralResult, value);
        }

        private ObservableCollection<WallkonElementResult> _OrdinaryResult;
        public ObservableCollection<WallkonElementResult> OrdinaryResult
        {
            get => _OrdinaryResult;
            set => SetAndNotify(ref _OrdinaryResult, value);
        }

        private ObservableCollection<WallkonElementResult> _FishyResult;
        public ObservableCollection<WallkonElementResult> FishyResult
        {
            get => _FishyResult;
            set => SetAndNotify(ref _FishyResult, value);
        }
        private ObservableCollection<WallModel> _CollectResult;
        public ObservableCollection<WallModel> CollectResult
        {
            get => _CollectResult;
            set => SetAndNotify(ref _CollectResult, value);
        }
        #endregion

        #region Method
        private void OnGeneralInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await WallkonFactory.Image(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            WallkonType = WallkonEnum.Search,
                            Search = new WallkonSearch
                            {
                                Limit = this.Limit,
                                Page = 1,
                                KeyWord = this.Keyword,
                                QueryType= QueryEnum.SFW
                            }
                        };
                    }).RunsAsync()).GlobalResult;
                    GeneralTotal = result.Total;
                    GeneralResult = new ObservableCollection<WallkonElementResult>(result.Result);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void OnOrdinaryInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await WallkonFactory.Image(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            WallkonType = WallkonEnum.Search,
                            Search = new WallkonSearch
                            {
                                Limit = this.Limit,
                                Page = 1,
                                KeyWord = this.Keyword,
                                QueryType = QueryEnum.Sketchy
                            }

                        };
                    }).RunsAsync()).GlobalResult;
                    OrdinaryTotal = result.Total;
                    OrdinaryResult = new ObservableCollection<WallkonElementResult>(result.Result);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void OnFishyInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await WallkonFactory.Image(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            WallkonType = WallkonEnum.Search,
                            Search = new WallkonSearch
                            {
                                Limit = this.Limit,
                                Page = 1,
                                KeyWord = this.Keyword,
                                QueryType = QueryEnum.NSFW
                            }
                        };
                    }).RunsAsync()).GlobalResult;
                    FishyTotal = result.Total;
                    FishyResult = new ObservableCollection<WallkonElementResult>(result.Result);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void OnLoadMoreGeneralInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await WallkonFactory.Image(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            WallkonType = WallkonEnum.Search,
                            Search = new WallkonSearch
                            {
                                Limit = this.Limit,
                                Page = GeneralPageIndex,
                                KeyWord = this.Keyword,
                                QueryType = QueryEnum.SFW
                            }
                        };
                    }).RunsAsync()).GlobalResult;
                    BindingOperations.EnableCollectionSynchronization(GeneralResult, LockObject);
                    Application.Current.Dispatcher.Invoke(() => result.Result.ForEach(GeneralResult.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void OnLoadMoreOrdinaryInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await WallkonFactory.Image(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            WallkonType = WallkonEnum.Search,
                            Search = new WallkonSearch
                            {
                                Limit = this.Limit,
                                Page = OrdinaryPageIndex,
                                KeyWord = this.Keyword,
                                QueryType = QueryEnum.Sketchy
                            }
                        };
                    }).RunsAsync()).GlobalResult;
                    BindingOperations.EnableCollectionSynchronization(OrdinaryResult, LockObject);
                    Application.Current.Dispatcher.Invoke(() => result.Result.ForEach(OrdinaryResult.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void OnLoadMoreFishyInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await WallkonFactory.Image(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            WallkonType = WallkonEnum.Search,
                            Search = new WallkonSearch
                            {
                                Limit = this.Limit,
                                Page = FishyPageIndex,
                                KeyWord = this.Keyword,
                                QueryType = QueryEnum.NSFW
                            }
                        };
                    }).RunsAsync()).GlobalResult;
                    BindingOperations.EnableCollectionSynchronization(FishyResult, LockObject);
                    Application.Current.Dispatcher.Invoke(() => result.Result.ForEach(FishyResult.Add));
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

        #region Command

        public RelayCommand<object> ChangedCommand => new((item) =>
        {
            var Target = ((CandyToggleItem)item);
            if (Target.FindParent<UserControl>() is WallchanView View)
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
                if (Index == 2)
                {
                    View.ActiveAnime = 3;
                    View.AnimeX3.Begin();
                }
                if (Index == 3)
                {
                    View.ActiveAnime = 4;
                    View.AnimeX4.Begin();
                }           
            }
        });

        /// <summary>
        /// 加载更多
        /// </summary>
        public RelayCommand<ScrollChangedEventArgs> ScrollCommand => new((obj) =>
        {
            if (ChangeType == 1)
            {
                if (GeneralPageIndex <= GeneralTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    GeneralPageIndex += 1;
                    OnLoadMoreGeneralInit();
                }
            }
            if (ChangeType == 2)
            {
                if (OrdinaryPageIndex <= OrdinaryTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    OrdinaryPageIndex += 1;
                    OnLoadMoreOrdinaryInit();
                }
            }
            if (ChangeType == 3)
            {
                if (FishyPageIndex <= FishyTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    FishyPageIndex += 1;
                    OnLoadMoreFishyInit();
                }
            }
        });
        /// <summary>
        /// 切换
        /// </summary>
        /// <param name="type"></param>
        public void ChangeCommand(int type)
        {
            ChangeType = type;
            if (ChangeType == 1 && GeneralResult == null)
                OnGeneralInit();
            if (ChangeType == 2 && OrdinaryResult == null)
                OnOrdinaryInit();
            if (ChangeType == 3 && FishyResult == null)
                OnFishyInit();
        }
        /// <summary>
        /// 收藏
        /// </summary>
        /// <param name="element"></param>
        public void CollectCommand(WallkonElementResult element)
        {
            var Model = element.ToMapest<WallModel>();
            Model.Platform = 2;
            Model.PId = Service.Insert(Model);
            CollectResult.Add(Model);
        }

        public void CheckCommand(WallModel element)
        {
            Builder.Add(element);
            GenericDelegate.HandleAction?.Invoke(Builder);
        }
        public void UnCheckCommand(WallModel element)
        {
            Builder.Remove(element);
            GenericDelegate.HandleAction?.Invoke(Builder);
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
            GeneralPageIndex = OrdinaryPageIndex = FishyPageIndex = 1;
            if (ChangeType == 1)
                OnGeneralInit();
            if (ChangeType == 2)
                OnOrdinaryInit();
            if (ChangeType == 3)
                OnFishyInit();
        }
        #endregion
    }
}
