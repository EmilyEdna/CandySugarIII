﻿namespace CandySugar.LightNovel.ViewModels
{
    public partial class ReaderViewModel : BasicObservableObject
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
                BorderHeight = 1400;
                BorderWidth = Picture == null ? SystemParameters.FullPrimaryScreenWidth : 1200;
                MarginThickness = new Thickness(0, 0, 20, 55);
            }
            else
            {
                BorderHeight = 1200;
                BorderWidth = 1000;
                MarginThickness = new Thickness(0, 0, 10, 0);
            }
            if (Picture != null)
                Picture = new(Picture.ToList());
            if (Words != null)
                Words = new(Words.ToList());
        }
        #endregion

        #region  字段
        public ReaderView Views;
        #endregion

        #region 属性
        [ObservableProperty]
        private ObservableCollection<string> _Words;
        [ObservableProperty]
        private ObservableCollection<string> _Picture;
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
                    XLog.Fatal(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void ErrorNotify(string input = "") =>
            Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(input.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : input).Show());
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
