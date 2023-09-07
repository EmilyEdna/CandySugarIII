namespace CandySugar.Novel.ViewModels
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
        #endregion

        #region Method
        /// <summary>
        /// 初始化内容
        /// </summary>
        private void OnInitContent( )
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await NovelFactory.Novel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            NovelType = NovelEnum.Content,
                            Content = new NovelContent
                            {
                                Route = ModuleEnv.GlobalTempParam.ToString()
                            }
                        };
                    }).RunsAsync()).ContentResult;

                    Words = new ObservableCollection<string>(result.Content);
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
