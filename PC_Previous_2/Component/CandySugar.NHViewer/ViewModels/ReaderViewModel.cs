using CandySugar.NHViewer.Model;
using XExten.Advance.NetFramework;
using XExten.Advance.NetFramework.Options;

namespace CandySugar.NHViewer.ViewModels
{
    public partial class ReaderViewModel : BasicObservableObject
    {
        public ReaderViewModel()
        {
            Picture = [];
            if (Module.Param is List<string>)
            {
                ((List<string>)Module.Param)?.ForEnumerEach((item, index) =>
                {
                    Picture.Add(new WatchInfo
                    {
                        Index = index,
                        Route = item,
                    });
                });
                ReaderReferer = string.Empty;
                PlatformEnum = PlatformEnum.NH;
            }
            else
            {
                var Param = ((Dictionary<string, List<string>>)Module.Param).FirstOrDefault();
                ReaderReferer = Param.Key;
                PlatformEnum = PlatformEnum.HI;
                Param.Value?.ForEnumerEach((item, index) =>
                {
                    Picture.Add(new WatchInfo
                    {
                        Index = index,
                        Route = item,
                    });
                });
            }

            LoadAvifBase64(Picture.FirstOrDefault());
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
        }

        #region 字段
        private PlatformEnum PlatformEnum;
        private string ReaderReferer;
        public ReaderView Views;
        #endregion

        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
            {
                BorderHeight = 1400;
                BorderWidth = 1200;
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
        }
        #endregion

        #region 属性
        [ObservableProperty]
        private ObservableCollection<WatchInfo> _Picture;
        [ObservableProperty]
        private WatchInfo _Current;
        #endregion

        #region 命令
        [RelayCommand]
        public void Handle(string input)
        {
            var Data = input.AsInt();
            if (Data == -1)
            {
                if (Current.Index + Data < 0) return;
                LoadAvifBase64(Picture.ElementAtOrDefault(Current.Index + Data));

            }
            else if (Data == 1)
            {
                if (Current.Index + Data >= Picture.Count) return;
                LoadAvifBase64(Picture.ElementAtOrDefault(Current.Index + Data));
            }
            else
            {
                var VM = ((MainViewModel)Views.FindParent<UserControl>("Main").DataContext);
                if (PlatformEnum == PlatformEnum.NH) VM.NChanged(false);
                else VM.HChanged(false);
            }
        }
        #endregion

        #region 方法
        private async void LoadAvifBase64(WatchInfo watchInfo)
        {
            if (PlatformEnum == PlatformEnum.HI)
            {

                var Nodes = new List<DefaultNodes> {
                     new DefaultNodes{ Node =string.Format(watchInfo.Route, "a") },
                     new DefaultNodes{ Node =string.Format(watchInfo.Route, "b") }
                };

                if (watchInfo.Route.Contains("https://") || watchInfo.Route.Contains("http://"))
                {
                    var bytes = await NetFactoryExtension.Resolve<INetFactory>().AddHeader(opt =>
                     {
                         opt.Key = ConstDefault.Referer;
                         opt.Value = ReaderReferer;
                     }).AddNode(Nodes)
                     .Build(opt =>
                     {
                         opt.UseCache = true;
                         opt.CacheSpan = ComponentBinding.OptionObjectModels.Cache;
                     }).RunBytes();
                    if (bytes.FirstOrDefault().Length > 1000)
                        watchInfo.Route = Convert.ToBase64String(bytes.FirstOrDefault());
                    else
                        watchInfo.Route = Convert.ToBase64String(bytes.LastOrDefault());
                }
                Current = watchInfo;
            }
        }
        #endregion
    }
}
