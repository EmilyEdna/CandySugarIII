using NPOI.Util;

namespace CandySugar.LightNovel.ViewModels
{
    public partial class ReaderViewModel : ObservableObject
    {
        public ReaderViewModel()
        {
            OnContent();
        }
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
                    ///novel/2/2003/70892.htm
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
                                ChapterRoute = "/novel/2/2003/70892.htm"//Module.Param.ToString()
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
            });
        }

        private void ErrorNotify(string input = "")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new ScreenNotifyView(input.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : input).Show();
            });
        }
        #endregion
    }
}
