using CandySugar.Novel.Models;

namespace CandySugar.Novel.ViewModels
{
    public class ReaderViewModel : PropertyChangedBase
    {
        private ContentDataModel DataModel;
        public ReaderViewModel()
        {
            DataModel = (ContentDataModel)ModuleEnv.GlobalTempParam;
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

        #region method
        public void KeyHandler(int cat)
        {
            if (cat == 0) BackCommand();
            if (cat == -1)
            {
                if (DataModel.Index + cat < 0) return;
                else
                {
                    DataModel.Index += cat;
                    DataModel.Current = DataModel.Chapters[DataModel.Index].Route;
                    OnInitContent();
                }
            }
            if (cat == 1) 
            {
                if (DataModel.Index + cat >= DataModel.Chapters.Count) return;
                else
                {
                    DataModel.Index += cat;
                    DataModel.Current = DataModel.Chapters[DataModel.Index].Route;
                    OnInitContent();
                }
            }
        }
        #endregion
    }
}
