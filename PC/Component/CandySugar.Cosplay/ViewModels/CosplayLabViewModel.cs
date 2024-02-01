using System.IO;
using XExten.Advance.JsonDbFramework;

namespace CandySugar.Cosplay.ViewModels
{
    public class CosplayLabViewModel : PropertyChangedBase
    {
        private object LockObject = new object();
        public List<CosplayInitElementResult> Builder;
        private JsonDbHandle<CosplayInitElementResult> JsonHandler;
        private string DbPath = Path.Combine(CommonHelper.DownloadPath, "Cosplay", $"CosplayLab.{FileTypes.Dat}");
        public CosplayLabViewModel()
        {
            Title = ["常规", "写真", "收藏"];
            Builder=new List<CosplayInitElementResult>();
            GenericDelegate.SearchAction = new(SearchHandler);
            JsonHandler = new JsonDbContext(DbPath).LoadInMemory<CosplayInitElementResult>();
            var LocalDATA = JsonHandler.GetAll();
            CollectResult = new ObservableCollection<CosplayInitElementResult>();
            if (LocalDATA != null)
            {
                LocalDATA.ForEach(CollectResult.Add);
            }
        }

        #region Field
        private string Keyword;
        private int GeneralTotal;
        private int GeneralPageIndex = 1;
        private int RealyTotal;
        private int RealyPageIndex = 1;
        /// <summary>
        /// 1：常规 2：写真 3：收藏
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

        private ObservableCollection<CosplayInitElementResult> _RealResult;
        public ObservableCollection<CosplayInitElementResult> RealResult
        {
            get => _RealResult;
            set => SetAndNotify(ref _RealResult, value);
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
            if (Target.FindParent<UserControl>() is CosplayLabView View)
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
            if (ChangeType == 2 && RealResult == null)
                OnRealyInit();
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
            if (ChangeType == 2)
            {
                if (RealyPageIndex <= RealyTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    RealyPageIndex += 1;
                    OnLoadMoreRealyInit();
                }
            }
        });
        /// <summary>
        /// 收藏
        /// </summary>
        /// <param name="element"></param>
        public void CollectCommand(CosplayInitElementResult element)
        {
            CollectResult.Add(element);
            JsonHandler.Insert(element).ExuteInsert().SaveChange();
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
                             PlatformType = PlatformEnum.Lab,
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
                            PlatformType = PlatformEnum.Lab,
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

        private void OnRealyInit()
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
                            CosplayType = CosplayEnum.Catagory,
                            PlatformType = PlatformEnum.Lab,
                            Category = new CosplayCategory
                            {
                                Page = 1
                            }
                        };
                    }).RunsAsync()).InitResult;
                    RealyTotal = result.Total;
                    RealResult = new ObservableCollection<CosplayInitElementResult>(result.ElementResults);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnLoadMoreRealyInit()
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
                            CosplayType = CosplayEnum.Catagory,
                            PlatformType = PlatformEnum.Lab,
                            Category = new CosplayCategory
                            {
                                Page = RealyPageIndex
                            }
                        };
                    }).RunsAsync()).InitResult;
                    BindingOperations.EnableCollectionSynchronization(RealResult, LockObject);
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(RealResult.Add));
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
