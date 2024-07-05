namespace CandySugar.Cosplay.ViewModels
{
    public partial class IndexViewModel : BasicObservableObject
    {

        public IndexViewModel()
        {
            Builder = [];
            CollectResult = [];
            PlatformType = PlatformEnum.Land;
            Title = PlatformType == PlatformEnum.Land ? ["常规", "收藏"] : ["常规", "写真", "收藏"];
            MenuData = new() { { "Lab", "1" }, { "Land", "2" }, { "下载选中", "3" }, { "删除选中", "4" }, { "无声相册", "5" }, { "音乐相册", "6" } };
            Service = IocDependency.Resolve<IService<CosplayModel>>();
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
        }

        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
            {
                Cols = (int)(GlobalParam.MAXWidth / 320);
                MarginThickness = new Thickness(0, 0, 60, 70);
            }
            else
            {
                Cols = (int)(GlobalParam.MAXWidth / 320);
                MarginThickness = new Thickness(0, 0, 60, 15);
            }
            BorderHeight = GlobalParam.MAXHeight;
            BorderWidth = GlobalParam.MAXWidth;
        }
        #endregion

        #region 字段
        private IService<CosplayModel> Service;
        private List<CosplayModel> Builder;

        private PlatformEnum PlatformType;
        private int GeneralTotal;
        private int GeneralPage = 1;
        private int RealyTotal;
        private int RealyPage = 1;
        /// <summary>
        /// 1：常规 2：写真 3：收藏
        /// </summary>
        private int ChangeType = 1;
        #endregion

        #region 属性
        [ObservableProperty]
        private ObservableCollection<string> _Title;
        [ObservableProperty]
        private Dictionary<string, string> _MenuData;
        [ObservableProperty]
        private ObservableCollection<CosplayInitElementResult> _CosResult;
        [ObservableProperty]
        private ObservableCollection<CosplayInitElementResult> _RealResult;
        [ObservableProperty]
        private ObservableCollection<CosplayModel> _CollectResult;
        #endregion

        #region 命令
        [RelayCommand]
        public void Changed(object item)
        {
            var Target = (CandyToggleItem)item;
            if (Target.FindParent<UserControl>() is IndexView View)
            {
                var Index = Target.Tag.ToString().AsInt();
                if (PlatformType == PlatformEnum.Lab)
                {
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
                    if (Index == 2)
                    {
                        View.ActiveAnime = 3;
                        View.AnimeX3.Begin();
                    }
                }
                else
                {
                    if (Index == 0)
                    {
                        View.ActiveAnime = 1;
                        View.AnimeX1.Begin();
                    }
                    if (Index == 1)
                    {
                        View.ActiveAnime = 3;
                        View.AnimeX3.Begin();
                    }
                }
            }
        }
        [RelayCommand]
        public void Scroll(ScrollChangedEventArgs obj)
        {
            if (ChangeType == 1)
            {
                if (GeneralPage <= GeneralTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    GeneralPage += 1;
                    OnLoadMoreCosInit();
                }
            }
            if (ChangeType == 2)
            {
                if (RealyPage <= RealyTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    RealyPage += 1;
                    OnLoadMoreRealyInit();
                }
            }
        }

        [RelayCommand]
        public void Collect(CosplayInitElementResult element)
        {
            if (element.Platform == PlatformEnum.Land)
                OnCosDetail(element);
            else
            {
                Service.Insert(element.ToMapest<CosplayModel>());
                CollectResult = new(Service.QueryAll());
            }
        }

        [RelayCommand]
        public void Check(CosplayModel input) => Builder.Add(input);

        [RelayCommand]
        public void UnCheck(CosplayModel input) => Builder.Remove(input);

        [RelayCommand]
        public void Active(object input)
        {
            var param = input.ToMapest<AnonymousWater>();
            var value = param.SelectValue.AsString().AsInt();
            if (value == 1)
            {
                PlatformType = PlatformEnum.Lab;
                Title = ["常规", "写真", "收藏"];
                GeneralPage = 1;
                CosResult = null;
            }
             if (value == 2)
            {
                CosResult = null;
                GeneralPage = 1;
                if (ChangeType == 2) ChangeType = 1;
                PlatformType = PlatformEnum.Land;
                Title = ["常规", "收藏"];
            }
             if (value == 3)
                DownSelectPicture();
             if (value == 4)
                RemoveSelectPicture();
             if (value == 5)
                BuilderVideoPicture();
            if (value == 6)
                BuilderVideoAudioPicture();
        }
        #endregion

        #region 方法
        private void ErrorNotify(string input = "") =>
                  Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(input.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : input).Show());

        public void ChangeActive(int ActiveAnime)
        {
            ChangeType = ActiveAnime;
            if (ChangeType == 1 && CosResult == null)
                OnCosInit();
            else if (ChangeType == 2 && RealResult == null)
                OnRealyInit();
            else
                CollectResult = new(Service.QueryAll());
        }

        private void OnCosInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await CosplayFactory.Cosplay(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CosplayType = CosplayEnum.Init,
                            PlatformType = PlatformType,
                            Init = new CosplayInit
                            {
                                Page = 1
                            }
                        };
                    }).RunsAsync()).InitResult;
                    GeneralTotal = result.Total;
                    CosResult = new ObservableCollection<CosplayInitElementResult>(result.ElementResults);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnLoadMoreCosInit()
        {
            if (CosResult == null) return;
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await CosplayFactory.Cosplay(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CosplayType = CosplayEnum.Init,
                            PlatformType = PlatformType,
                            Init = new CosplayInit
                            {
                                Page = GeneralPage
                            }
                        };
                    }).RunsAsync()).InitResult;
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(CosResult.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnRealyInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await CosplayFactory.Cosplay(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CosplayType = CosplayEnum.Catagory,
                            PlatformType = PlatformType,
                            Category = new CosplayCategory
                            {
                                Page = 1
                            }
                        };
                    }).RunsAsync()).InitResult;
                    RealyTotal = result.Total;
                    RealResult = new ObservableCollection<CosplayInitElementResult>(result.ElementResults);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnLoadMoreRealyInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await CosplayFactory.Cosplay(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CosplayType = CosplayEnum.Catagory,
                            PlatformType = PlatformType,
                            Category = new CosplayCategory
                            {
                                Page = RealyPage
                            }
                        };
                    }).RunsAsync()).InitResult;
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(RealResult.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnCosDetail(CosplayInitElementResult input)
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await CosplayFactory.Cosplay(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            CosplayType = CosplayEnum.Detail,
                            PlatformType = PlatformEnum.Land,
                            Detail = new CosplayDetail
                            {
                                Route = input.Route
                            }
                        };
                    }).RunsAsync()).DetailResult;
                    CosResult.First(t => t.Route.ToMd5() == result.Request.ToMd5()).Images = result.Image;
                    Service.Insert(input.ToMapest<CosplayModel>());
                    CollectResult = new(Service.QueryAll());
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        #endregion

        #region 函数
        private void DownSelectPicture()
        {
            if (Builder.Count > 0)
            {
                Task.Run(() =>
                {
                    Builder.ForEach(async item =>
                    {
                        for (int index = 0; index < item.Images.Count; index++)
                        {
                            var fileBytes = await new HttpClient().GetByteArrayAsync(item.Images[index]);
                            fileBytes.FileCreate(item.Images[index].ToMd5(), FileTypes.Jpg, Path.Combine("Cosplay", item.Platform.ToString(), item.Title.ToMd5()), (catalog, fileName) =>
                            {
                                if (index == item.Images.Count - 1)
                                    new CandyNotifyControl(CommonHelper.DownloadFinishInformation, true, catalog).Show();
                            });
                        }
                    });
                });
            }
        }
        private void RemoveSelectPicture()
        {
            if (Builder.Count > 0)
            {
                Builder.ForEach(item =>
                {
                    item.Images.ForEach(node => SyncStatic.DeleteFile(DownUtil.FilePath(node.ToMd5(), FileTypes.Jpg, Path.Combine("Cosplay", PlatformType.AsString(), item.Title.ToMd5()))));
                    Service.Remove(item.PId);
                });
                CollectResult = new(Service.QueryAll());
                Builder.Clear();
            }
        }
        private void BuilderVideoPicture()
        {
            if (Builder.Count > 0)
            {
                var RealLocal = new List<string>();
                //判断本地文件是否存在
                Builder.ForEach(item =>
                {
                    item.Images.ForEach(node =>
                    {
                        var fileName = DownUtil.FilePath(node.ToMd5(), FileTypes.Jpg, Path.Combine("Cosplay", PlatformType.AsString(), item.Title.ToMd5()));
                        if (File.Exists(fileName)) RealLocal.Add(fileName);
                    });
                });
                //没有被删除真实存在的文件
                if (RealLocal.Count > 0)
                {
                    //异步制作MP4
                    Task.Run(async () =>
                    {
                        var catalog = Path.GetDirectoryName(RealLocal.First());
                        var res = await RealLocal.ImageToVideo(catalog);
                        if (res) Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(CommonHelper.ConvertFinishInformation, true, catalog).Show());
                    });
                }
            }
        }
        private void BuilderVideoAudioPicture() 
        {
            string AudioName = string.Empty;
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "音频|*.mp3"
            };
            var res = dialog.ShowDialog();
            if (res == true)
                AudioName = dialog.FileName;
            if (AudioName.IsNullOrEmpty()) return;
            var Time = AudioFactory.Instance.InitAudio(AudioName).AudioReader.TotalTime.TotalSeconds.ToString("F0");
            AudioFactory.Instance.Dispose();
            if (Builder != null)
            {
              var  RealLocal = new List<string>();
                //判断本地文件是否存在
                Builder.ForEach(item => {
                    item.Images.ForEach(node =>
                    {
                        var fileName = DownUtil.FilePath(node.ToMd5(), FileTypes.Jpg, Path.Combine("Cosplay", PlatformType.AsString(), item.Title.ToMd5()));
                        if (File.Exists(fileName)) RealLocal.Add(fileName);
                    });
                });
                //没有被删除真实存在的文件
                if (RealLocal.Count > 0)
                {
                    //异步制作MP4
                    Task.Run(async () =>
                    {
                        var catalog = Path.GetDirectoryName(RealLocal.First());
                        var res = await RealLocal.ImageToVideo(AudioName, Time, catalog);
                        if (res) Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(CommonHelper.ConvertFinishInformation, true, catalog).Show());
                    });
                }
            }
        }
        #endregion
    }
}
