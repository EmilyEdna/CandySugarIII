namespace CandySugar.Novel.ViewModels
{
    public partial class ReaderViewModel : ObservableObject
    {

        public ReaderViewModel()
        {
            DataModel = (ContentDataModel)Module.Param;
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
            OnContent();
        }

        #region  字段
        private ContentDataModel DataModel;
        public ReaderView Views;
        #endregion

        #region 属性
        [ObservableProperty]
        private Thickness _MarginThickness;
        [ObservableProperty]
        private NovelContentElementResult _Element;
        #endregion

        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
                MarginThickness = new Thickness(0, 0, 20, 55);
            else
                MarginThickness = new Thickness(0, 0, 10, 0);
        }
        #endregion

        #region 方法
        /// <summary>
        /// 初始化内容
        /// </summary>
        private void OnContent()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    Element = (await NovelFactory.Novel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            PlatformType = DataModel.Platform,
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            NovelType = NovelEnum.Content,
                            Content = new NovelContent
                            {
                                Route = DataModel.Current
                            }
                        };
                    }).RunsAsync()).ContentResult.ElementResult;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void ErrorNotify(string input = "") =>
                    Application.Current.Dispatcher.Invoke(() => new ScreenNotifyView(input.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : input).Show());
        #endregion

        #region 命令
        [RelayCommand]
        public void Handle(string input)
        {
            var Data = input.AsInt();
            if (Data == -1)
            {
                if (DataModel.Index + Data < 0) return;
                else
                {
                    DataModel.Index += Data;
                    DataModel.Current = DataModel.Chapters[DataModel.Index].Route;
                    OnContent();
                }
            }
            else if (Data == 1)
            {
                if (DataModel.Index + Data >= DataModel.Chapters.Count) return;
                else
                {
                    DataModel.Index += Data;
                    DataModel.Current = DataModel.Chapters[DataModel.Index].Route;
                    OnContent();
                }
            }
            else
                ((MainViewModel)Views.FindParent<UserControl>("Main").DataContext).Changed(false);
        }
        #endregion
    }
}
