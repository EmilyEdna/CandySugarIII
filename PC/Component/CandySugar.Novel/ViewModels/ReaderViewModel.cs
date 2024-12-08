namespace CandySugar.Novel.ViewModels
{
    public partial class ReaderViewModel : BasicObservableObject
    {

        public ReaderViewModel()
        {
            DataModel = Module.Param.ToMapest<ContentDataModel>();
            Service = IocDependency.Resolve<IService<NovelModel>>();
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
            OnContent();
        }

        #region  字段
        private IService<NovelModel> Service;
        private ContentDataModel DataModel;
        public ReaderView Views;
        #endregion

        #region 属性
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

            BorderWidth = GlobalParam.MAXWidth;
            if (Element != null)
            {
                var Name = Element.ChapterName;
                var Words = Element.Content;
                Element = new()
                {
                    ChapterName = Name,
                    Content = Words
                };
            }
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
                    RemoveAdd();
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
                    RemoveAdd();
                    OnContent();
                }
            }
            else
                ((MainViewModel)Views.FindParent<UserControl>("Main").DataContext).Changed(false);
        }
        #endregion

        #region 方法
        private void RemoveAdd()
        {
            var Entity = Service.QueryAll().FirstOrDefault(t => t.Detail.ToMd5() == DataModel.MD5);
            Service.Remove(Entity.PId);
            Entity.Current = DataModel.Index;
            Entity.Chapter = DataModel.Chapters[DataModel.Index].Chapter;
            Entity.Route = DataModel.Current;
            Service.Insert(Entity);
        }
        #endregion
    }
}
