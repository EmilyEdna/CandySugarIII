namespace CandySugar.Cosplay.ViewModels
{
    public partial class Index1ViewModel : BasicObservableObject
    {

        public Index1ViewModel()
        {
            PlatformType = PlatformEnum.Land;
            Service = IocDependency.Resolve<IService<CosplayModel>>();
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
            OnCosInit();
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
        private PlatformEnum PlatformType;
        private int GeneralTotal;
        private int GeneralPage = 1;
        #endregion

        #region 属性
        [ObservableProperty]
        private ObservableCollection<CosplayInitElementResult> _CosResult;
        #endregion

        #region 命令
        [RelayCommand]
        public void Scroll(ScrollChangedEventArgs obj)
        {
            if (GeneralPage <= GeneralTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
            {
                GeneralPage += 1;
                OnLoadMoreCosInit();
            }
        }

        [RelayCommand]
        public void Collect(CosplayInitElementResult element)
            => OnCosDetail(element);
        #endregion

        #region 方法
        private void ErrorNotify(string input = "")
            => CandyNotify.Error(input.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : input);

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
            if (CosResult == null) return;
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

        private void OnCosDetail(CosplayInitElementResult input)
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
                            CosplayType = CosplayEnum.Detail,
                            PlatformType = PlatformEnum.Land,
                            Detail = new CosplayDetail
                            {
                                Route = input.Route
                            }
                        };
                    }).RunsAsync()).DetailResult;
                    CosResult.First(t => t.Route.ToMd5() == result.Request.ToMd5()).Images = result.Image;
                    Service.Insert(input.ToMapest<CosplayModel>());
                    GenericDelegate.ChangeContentAction?.Invoke(string.Empty);
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
