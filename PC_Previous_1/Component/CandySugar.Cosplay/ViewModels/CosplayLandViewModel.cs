namespace CandySugar.Cosplay.ViewModels
{
    public class CosplayLandViewModel : PropertyChangedBase
    {
        private object LockObject = new object();
        public List<CosplayInitElementResult> Builder;
        private IService<CosplayModel> Service;
        public CosplayLandViewModel()
        {
            Title = ["常规", "收藏"];
            Builder = [];
            GenericDelegate.SearchAction = new(SearchHandler);
            Service = IocDependency.Resolve<IService<CosplayModel>>();
            var LocalDATA = Service.QueryAll().Where(t => t.Platform == 2).ToList();
            CollectResult = [];
            LocalDATA?.ToMapest<List<CosplayInitElementResult>>().ForEach(CollectResult.Add);
        }

        #region Field
        private string Keyword;
        private int GeneralTotal;
        private int GeneralPageIndex = 1;
        /// <summary>
        /// 1：常规  2：收藏
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

        private ObservableCollection<CosplayInitElementResult> _CosResult;
        public ObservableCollection<CosplayInitElementResult> CosResult
        {
            get => _CosResult;
            set => SetAndNotify(ref _CosResult, value);
        }

        private ObservableCollection<CosplayInitElementResult> _CollectResult;
        public ObservableCollection<CosplayInitElementResult> CollectResult
        {
            get => _CollectResult;
            set => SetAndNotify(ref _CollectResult, value);
        }
        #endregion

        #region Command
        public RelayCommand<object> ChangedCommand => new((item) =>
        {
            var Target = ((CandyToggleItem)item);
            if (Target.FindParent<UserControl>() is CosplayLandView View)
            {
                var Index = Target.Tag.ToString().AsInt();

                if (Index == 0)
                {
                    View.ActiveAnime = 1;
                    View.AnimeY1.Begin();
                }
                if (Index == 1)
                {
                    View.ActiveAnime = 2;
                    View.AnimeY2.Begin();
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
            if (ChangeType == 1 && CosResult == null)
                OnCosInit();
        }
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
                    OnLoadMoreCosInit();
                }
            }
        });
        /// <summary>
        /// 收藏
        /// </summary>
        /// <param name="element"></param>
        public void CollectCommand(CosplayInitElementResult element)
        {
            OnCosDetail(element, () =>
            {
                BindingOperations.EnableCollectionSynchronization(CollectResult, LockObject);
                Application.Current.Dispatcher.Invoke(() => CollectResult.Add(element));
                Service.Insert(element.ToMapest<CosplayModel>());
            });
        }
        public void CheckCommand(CosplayInitElementResult input)
        {
            Builder.Add(input);
            GenericDelegate.HandleAction?.Invoke(Builder);
        }
        public void UnCheckCommand(CosplayInitElementResult input)
        {
            Builder.Remove(input);
            GenericDelegate.HandleAction?.Invoke(Builder);
        }
        #endregion

        #region Method
        private void ErrorNotify(string Info = "")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new ScreenNotifyView(Info.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : Info).Show();
            });
        }

        private void OnCosInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await CosplayFactory.Cosplay(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CosplayType = CosplayEnum.Init,
                            PlatformType = PlatformEnum.Land,
                            Init = new CosplayInit
                            {
                                Page = 1
                            }
                        };
                    }).RunsAsync()).InitResult;
                    GeneralTotal = result.Total;
                    CosResult = new ObservableCollection<CosplayInitElementResult>(result.ElementResults);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnLoadMoreCosInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await CosplayFactory.Cosplay(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CosplayType = CosplayEnum.Init,
                            PlatformType = PlatformEnum.Land,
                            Init = new CosplayInit
                            {
                                Page = GeneralPageIndex
                            }
                        };
                    }).RunsAsync()).InitResult;
                    BindingOperations.EnableCollectionSynchronization(CosResult, LockObject);
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(CosResult.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnCosDetail(CosplayInitElementResult input, Action action)
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await CosplayFactory.Cosplay(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CosplayType = CosplayEnum.Detail,
                            PlatformType = PlatformEnum.Land,
                            Detail = new CosplayDetail
                            {
                                Route = input.Route
                            }
                        };
                    }).RunsAsync()).DetailResult;
                    CosResult.First(t => t.Route.ToMd5() == result.Request.ToMd5()).Images = result.Image;
                    action.Invoke();
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
        }
        #endregion
    }
}
