namespace CandySugar.LightNovel.ViewModels
{
    public class ReaderViewModel : PropertyChangedBase
    {
        public ReaderViewModel()
        {
            OnInitContent();
        }

        #region Property
        private ObservableCollection<string> _Words;
        /// <summary>
        /// 文本内容
        /// </summary>
        public ObservableCollection<string> Words
        {
            get => _Words;
            set => SetAndNotify(ref _Words, value);
        }

        private ObservableCollection<string> _Picture;
        /// <summary>
        /// 图片
        /// </summary>
        public ObservableCollection<string> Picture
        {
            get => _Picture;
            set => SetAndNotify(ref _Picture, value);
        }
        #endregion

        #region Method
        /// <summary>
        /// 初始化内容
        /// </summary>
        private void OnInitContent()
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
                                ChapterRoute = ModuleEnv.GlobalTempParam.ToString()
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

        #region Command
        public void BackCommand()
        {
            WeakReferenceMessenger.Default.Send(new MessageNotify
            {
                NotifyType = NotifyType.ChangeControl,
                ControlType = 1
            });
        }
        #endregion
    }
}
