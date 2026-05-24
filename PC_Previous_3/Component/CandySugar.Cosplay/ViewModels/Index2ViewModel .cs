namespace CandySugar.Cosplay.ViewModels
{
    public partial class Index2ViewModel : BasicObservableObject
    {

        public Index2ViewModel()
        {
            PlatformType = PlatformEnum.Lab;
            Title = ["常规", "写真"];
            Service = IocDependency.Resolve<IService<CosplayModel>>();
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
        }

        #region 事件
        private void WindowStateEvent()
        {
            BorderHeight = GlobalParam.MAXHeight;
            BorderWidth = GlobalParam.MAXWidth;
            if (GlobalParam.WindowState == WindowState.Maximized)
            {
                Cols = (int)(GlobalParam.MAXWidth / 360);
            }
            else
            {
                Cols = (int)(GlobalParam.MAXWidth / 360);
                BorderWidth -= 60;
            }
        }
        #endregion

        #region 字段
        private IService<CosplayModel> Service;
        private List<CosplayModel> Builder;

        private PlatformEnum PlatformType;
        private int GeneralTotal;
        private int GeneralPage = 1;
        private int RealyTotal;
        private int RealyPage = 1;
        /// <summary>
        /// 1：常规 2：写真 3：收藏
        /// </summary>
        private int ChangeType = 1;
        #endregion

        #region 属性
        [ObservableProperty]
        private ObservableCollection<string> _Title;
        [ObservableProperty]
        private ObservableCollection<CosplayInitElementResult> _CosResult;
        [ObservableProperty]
        private ObservableCollection<CosplayInitElementResult> _RealResult;
        #endregion

        #region 命令
        [RelayCommand]
        public void Changed(object item)
        {
            var Target = (CandyToggleItem)item;
            if (Target.FindParent<UserControl>() is Index2View View)
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
            if (ChangeType == 1)
            {
                if (GeneralPage <= GeneralTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    GeneralPage += 1;
                    OnLoadMoreCosInit();
                }
            }
            if (ChangeType == 2)
            {
                if (RealyPage <= RealyTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    RealyPage += 1;
                    OnLoadMoreRealyInit();
                }
            }
        }

        [RelayCommand]
        public void Collect(CosplayInitElementResult element)
        {
            Service.Insert(element.ToMapest<CosplayModel>());
            GenericDelegate.ChangeContentAction?.Invoke(string.Empty);
        }
        #endregion

        #region 方法
        private void ErrorNotify(string input = "")
            => CandyNotify.Error(input.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : input);

        public void ChangeActive(int ActiveAnime)
        {
            ChangeType = ActiveAnime;
            if (ChangeType == 1 && CosResult == null)
                OnCosInit();
            if (ChangeType == 2 && RealResult == null)
                OnRealyInit();
        }

        private void OnCosInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await CosplayFactory.Cosplay(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CosplayType = CosplayEnum.Init,
                            PlatformType = PlatformType,
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
                    XLog.Fatal(ex, "");
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
                    var result = (await CosplayFactory.Cosplay(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CosplayType = CosplayEnum.Init,
                            PlatformType = PlatformType,
                            Init = new CosplayInit
                            {
                                Page = GeneralPage
                            }
                        };
                    }).RunsAsync()).InitResult;
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(CosResult.Add));
                }
                catch (Exception ex)
                {
                    XLog.Fatal(ex, "");
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
                    var result = (await CosplayFactory.Cosplay(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CosplayType = CosplayEnum.Catagory,
                            PlatformType = PlatformType,
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
                    XLog.Fatal(ex, "");
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
                    var result = (await CosplayFactory.Cosplay(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CosplayType = CosplayEnum.Catagory,
                            PlatformType = PlatformType,
                            Category = new CosplayCategory
                            {
                                Page = RealyPage
                            }
                        };
                    }).RunsAsync()).InitResult;
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(RealResult.Add));
                }
                catch (Exception ex)
                {
                    XLog.Fatal(ex, "");
                    ErrorNotify();
                }
            });
        }
        #endregion
    }
}
