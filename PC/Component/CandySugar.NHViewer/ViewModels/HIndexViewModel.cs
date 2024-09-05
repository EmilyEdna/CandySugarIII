using CandySugar.Com.Data.Entity.HitomiEntity;
using Sdk.Component.Vip.Panda.sdk.ViewModel.Response;
using XExten.Advance.NetFramework;

namespace CandySugar.NHViewer.ViewModels
{
    public partial class HIndexViewModel : BasicObservableObject
    {

        public HIndexViewModel()
        {
            Title = ["全部", "喜爱"];
            NavVisible = Visibility.Hidden;
            Service = IocDependency.Resolve<IService<HitomiModel>>();
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
        }

        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
                Cols = (int)(GlobalParam.MAXWidth / 200);
            else
                Cols = 5;
            BorderWidth = GlobalParam.MAXWidth;
            BorderHeight = GlobalParam.MAXHeight;
            NavHeight = GlobalParam.NavHeight;
            NavWidth = GlobalParam.NavWidth;
        }
        //private void ReceiveProcess(double item, double num)
        //{
        //    if (IsPreview == true) return;
        //    if (item == double.Parse((100 / num).ToString("F2")))
        //        Counts += item;
        //    if (Math.Ceiling(Counts) >= 100)
        //    {
        //        Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(CommonHelper.DownloadFinishInformation, true, Catalog).Show());
        //        IsDown = false;
        //    }
        //}
        #endregion

        #region 字段
        public HIndexView Views;

        private int Total;
        private int PageIndex;
        private string Keyword;
        //private bool IsDown;
        private IService<HitomiModel> Service;
        #endregion

        #region 属性
        [ObservableProperty]
        private ObservableCollection<string> _Title;
        [ObservableProperty]
        private ObservableCollection<HitomiModel> _CollectResult;
        [ObservableProperty]
        private ObservableCollection<InitHElementResult> _Results;
        [ObservableProperty]
        private HitomiModel _Result;
        #endregion

        #region 命令
        [RelayCommand]
        public void Changed(object item)
        {
            var Target = ((CandyToggleItem)item);
            if (Target.FindParent<UserControl>() is HIndexView View)
            {
                var Index = Target.Tag.ToString().AsInt();

                if (Index == 0)
                {
                    View.ActiveAnime = 1;
                    View.AnimeX1.Begin();
                }
                if (Index == 1)
                {
                    View.ActiveAnime = 2;
                    View.AnimeX2.Begin();
                }
            }
        }

        [RelayCommand]
        public void Scroll(ScrollChangedEventArgs obj)
        {
            if (PageIndex <= Total && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
            {
                PageIndex += 1;
                OnLoadMoreInit();
            }
        }

        [RelayCommand]
        public void Watch(HitomiModel model)
        {
            Module.Param =  new Dictionary<string, List<string>> { { model.ReaderReferer, model.OriginImages } };
            ((MainViewModel)Views.FindParent<UserControl>("Main").DataContext).HChanged(true);
        }

        [RelayCommand]
        public async Task View(InitHElementResult result)
        {
            await LoadImg(result);

            Module.Param = new Dictionary<string, List<string>> { { Result.ReaderReferer, Result.OriginImages } };
            ((MainViewModel)Views.FindParent<UserControl>("Main").DataContext).HChanged(true);
        }

        [RelayCommand]
        public void Remove(Guid id)
        {
            Service.Remove(id);
            CollectResult = new(Service.QueryAll());
        }

        [RelayCommand]
        public async Task Collect(InitHElementResult input)
        {
            await LoadImg(input);
        }
        #endregion

        #region  方法

        public void ChangeActive(int ActiveAnime)
        {
            PageIndex = 1;
            Keyword = string.Empty;
            if (ActiveAnime == 1)
                OnInit();
            else
                CollectResult = new(Service.QueryAll());
        }
        private async Task LoadImg(InitHElementResult Results)
        {
            try
            {
                var Proxy = Module.IocModule.Proxy;
                var result = (await PandaFactory.Panda(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        ProxyIP = Proxy.IP,
                        ProxyPort = Proxy.Port,
                        CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                        PandaType = PandaEnum.Detail,
                        PlatformType = PlatformEnum.HI,
                        Detail = new PandaDetail
                        {
                            Id = Results.CId,
                            Js = Results.FileJs
                        }
                    };
                }).RunsAsync()).DetailResult;
                Result = new HitomiModel
                {
                    Cover = Results.AVIFBase64,
                    Title = Results.Title,
                    CId = Results.CId,
                    ReaderReferer= result.ReaderReferer,
                    OriginImages = result.AvifRoute
                };
                Service.Insert(Result);
                CollectResult = new(Service.QueryAll());
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "");
                ErrorNotify();
            }

        }

        public async void OnInit()
        {
            await Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await PandaFactory.Panda(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            PandaType = PandaEnum.Init,
                            PlatformType = PlatformEnum.HI,
                            Init = new PandaInit()
                        };
                    }).RunsAsync()).InitResult;
                    Total = result.TotalPage;

                    foreach (var item in result.HResults)
                    {
                        var Avif = (await NetFactoryExtension.Resolve<INetFactory>()
                              .AddNode(opt => opt.Node = item.AVIFBase64Route)
                              .AddHeader(opt =>
                              {
                                  opt.Key = ConstDefault.Referer;
                                  opt.Value = item.CoverReferer;
                              }).Build(opt =>
                              {
                                  opt.UseCache = true;
                                  opt.CacheSpan = ComponentBinding.OptionObjectModels.Cache;
                              }).RunBytes()).FirstOrDefault();

                        item.AVIFBase64 = Convert.ToBase64String(SyncStatic.ConvertBytesImage(Avif));
                    }
                    Results = new(result.HResults);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        public async void OnLoadMoreInit()
        {
            await Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await PandaFactory.Panda(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            PandaType = PandaEnum.Init,
                            PlatformType = PlatformEnum.HI,
                            Init = new PandaInit
                            {
                                Page = PageIndex
                            }
                        };
                    }).RunsAsync()).InitResult;

                    foreach (var item in result.HResults)
                    {
                        var Avif = (await NetFactoryExtension.Resolve<INetFactory>()
                              .AddNode(opt => opt.Node = item.AVIFBase64Route)
                              .AddHeader(opt =>
                              {
                                  opt.Key = ConstDefault.Referer;
                                  opt.Value = item.CoverReferer;
                              }).Build(opt =>
                              {
                                  opt.UseCache = true;
                                  opt.CacheSpan = ComponentBinding.OptionObjectModels.Cache;
                              }).RunBytes()).FirstOrDefault();

                        item.AVIFBase64 = item.AVIFBase64 = Convert.ToBase64String(SyncStatic.ConvertBytesImage(Avif));
                    }

                    Application.Current.Dispatcher.Invoke(() => result.HResults.ForEach(Results.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void ErrorNotify(string input = "") =>
              Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(input.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : input).Show());
        #endregion
    }
}
