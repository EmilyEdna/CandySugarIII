using System.Collections.Generic;

namespace CandySugar.Novel.ViewModels
{
    public class ReaderViewModel : PropertyChangedBase
    {
        public ReaderViewModel()
        {
            OnInitContent();
        }

        #region Property
        private NovelContentElementResult _Element;
        /// <summary>
        /// 文本内容
        /// </summary>
        public NovelContentElementResult Element
        {
            get => _Element;
            set => SetAndNotify(ref _Element, value);
        }
        #endregion

        #region Method
        /// <summary>
        /// 初始化内容
        /// </summary>
        private void OnInitContent()
        {
            var Param = ((Dictionary<string, object>)ModuleEnv.GlobalTempParam);
            var Platform = Enum.Parse<PlatformEnum>(Param["Key1"].AsString());
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    Element = (await NovelFactory.Novel(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            PlatformType = Platform,
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            NovelType = NovelEnum.Content,
                            Content = new NovelContent
                            {
                                Route = Param["Key2"].ToString()
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
