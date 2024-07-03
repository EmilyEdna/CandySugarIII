namespace CandySugar.LightNovel.ViewModels
{
    public partial class ReaderViewModel : ObservableObject
    {
        public ReaderViewModel()
        {
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
            OnContent();
        }
        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
            {
                Height = 1400;
                Width = SystemParameters.FullPrimaryScreenWidth;
                MarginThickness = new Thickness(0, 0, 20, 55);
            }
            else
            {
                Height = 1200;
                Width = 1000;
                MarginThickness = new Thickness(0, 0, 10, 0);
            }
            if (Picture != null)
            {
                var temp = Picture.ToList();
                Picture = new(temp);
            }
        }
        #endregion

        #region  字段
        public ReaderView Views;
        #endregion

        #region 属性
        [ObservableProperty]
        private Thickness _MarginThickness;
        [ObservableProperty]
        private ObservableCollection<string> _Words;
        [ObservableProperty]
        private ObservableCollection<string> _Picture;
        [ObservableProperty]
        private double _Width;
        [ObservableProperty]
        private double _Height;
        #endregion

        #region 方法
        /// <summary>
        /// 初始化内容
        /// </summary>
        private async void OnContent()
        {
            try
            {
                var Proxy = Module.IocModule.Proxy;
                var result = (await LovelFactory.Lovel(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        ProxyIP = Proxy.IP,
                        ProxyPort = Proxy.Port,
                        CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                        LovelType = LovelEnum.Content,
                        Content = new LovelContent
                        {
                            ChapterRoute = Module.Param.ToString()
                        }
                    };
                }).RunsAsync()).ContentResult;
                if (result.Content != null || result.Image != null)
                {
                    if (result.Content != null)
                    {
                        if (result.Content.Equals("因版权问题，文库不再提供该小说的阅读！"))
                        {
                            ErrorNotify("因版权问题，请前往下载!");
                            return;
                        }
                        Words = new ObservableCollection<string>(result.Content);
                    }
                    else
                        Picture = new ObservableCollection<string>(result.Image ?? new List<string>());
                }
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
        public void Back(string input)
        {
            ((MainViewModel)Views.FindParent<UserControl>("Main").DataContext).Changed(false);
        }
        #endregion
    }
}
