namespace CandySugar.WallPaper.ViewModels
{
    public class IndexViewModel : PropertyChangedBase
    {

        public IndexViewModel()
        {
            Service = IocDependency.Resolve<IService<WallModel>>(); 
            GenericDelegate.SearchAction = new(SearchHandler);
            Platform = PlatformEnum.Wallhaven;
            Title = ["常规", "一般", "可疑", "收藏"];
            Mode = ["WallHaven", "Konachan", "下载选中", "删除选中", "无声相册", "音乐相册"];
            WallBuilder = [];
            CollectResult = new(Service.QueryAll());
        }

        #region Field
        private IService<WallModel> Service;
        private object LockObject = new object();
        private List<WallModel> WallBuilder;

        private PlatformEnum Platform;
        private GradEnum Grad;
        private int InitTotal;
        private int InitPage;
        private int SearchPage;
        private int SearchTotal;
        private string Keyword;
        #endregion

        #region Property
        private ObservableCollection<string> _Mode;
        public ObservableCollection<string> Mode
        {
            get => _Mode;
            set => SetAndNotify(ref _Mode, value);
        }
        private ObservableCollection<string> _Title;
        public ObservableCollection<string> Title
        {
            get => _Title;
            set => SetAndNotify(ref _Title, value);
        }
        private ObservableCollection<WallElementResult> _Result;
        public ObservableCollection<WallElementResult> Result
        {
            get => _Result;
            set => SetAndNotify(ref _Result, value);
        }
        private ObservableCollection<WallModel> _CollectResult;
        public ObservableCollection<WallModel> CollectResult
        {
            get => _CollectResult;
            set => SetAndNotify(ref _CollectResult, value);
        }
        #endregion

        #region Method
        private void ErrorNotify(string Info = "")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new ScreenNotifyView(Info.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : Info).Show();
            });
        }

        private void OnInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await WallFactory.Wall(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            WallType = WallEnum.Init,
                            PlatformType = Platform,
                            Init = new WallInit
                            {
                                Grad = Grad,
                                Page = 1,
                            }
                        };
                    }).RunsAsync()).Result;
                    InitTotal = result.Total;
                    Result = new ObservableCollection<WallElementResult>(result.ElementResults);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnLoadMoreInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await WallFactory.Wall(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            WallType = WallEnum.Init,
                            PlatformType = Platform,
                            Init = new WallInit
                            {
                                Grad = Grad,
                                Page = InitPage,
                            }
                        };
                    }).RunsAsync()).Result;
                    BindingOperations.EnableCollectionSynchronization(Result, LockObject);
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(Result.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnSearch()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await WallFactory.Wall(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            WallType = WallEnum.Search,
                            PlatformType = Platform,
                            Search = new WallSearch
                            {
                                Grad = Grad,
                                Page = 1,
                                Keyword = Keyword
                            }
                        };
                    }).RunsAsync()).Result;
                    SearchTotal = result.Total;
                    Result = new ObservableCollection<WallElementResult>(result.ElementResults);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnLoadMoreSearch()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await WallFactory.Wall(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            WallType = WallEnum.Search,
                            PlatformType = Platform,
                            Search = new WallSearch
                            {
                                Grad = Grad,
                                Page = SearchPage,
                                Keyword = Keyword
                            }
                        };
                    }).RunsAsync()).Result;
                    BindingOperations.EnableCollectionSynchronization(Result, LockObject);
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(Result.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void Download()
        {
            Task.Run(() =>
            {
                WallBuilder.ForEach(async item =>
                {
                    var fileBytes = await new HttpClient().GetByteArrayAsync(item.Original);
                    fileBytes.FileCreate(item.PId.ToString(), FileTypes.Jpg, "WallPaper", (catalog, fileName) =>
                    {
                        new ScreenDownNofityView(CommonHelper.DownloadFinishInformation, catalog).Show();
                    });
                });
            });
        }

        private void Remove()
        {
            WallBuilder.ForEach(item =>
            {
                SyncStatic.DeleteFile(DownUtil.FilePath(item.PId.ToString(), FileTypes.Jpg, "WallPaper"));
            });
        }

        private void BuildPicture()
        {
            var RealLocal = new List<string>();
            WallBuilder.ForEach(item =>
            {
                var fileName = DownUtil.FilePath(item.PId.ToString(), FileTypes.Jpg, "WallPaper");
                if (File.Exists(fileName)) RealLocal.Add(fileName);
            });
            //没有被删除真实存在的文件
            if (RealLocal.Count > 0)
            {
                //异步制作MP4
                Task.Run(async () =>
                {
                    var catalog = Path.Combine(CommonHelper.DownloadPath, "WallPaper");
                    var res = await RealLocal.ImageToVideo(catalog);
                    if (res) Application.Current.Dispatcher.Invoke(() =>
                    {
                        new ScreenDownNofityView(CommonHelper.ConvertFinishInformation, catalog).Show();
                    });
                });
            }
        }

        private void BuildAudio()
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

            var RealLocal = new List<string>();
            //判断本地文件是否存在
            WallBuilder.ForEach(item =>
            {
                var fileName = DownUtil.FilePath(item.PId.ToString(), FileTypes.Jpg, "WallPaper");
                if (File.Exists(fileName)) RealLocal.Add(fileName);
            });
            //没有被删除真实存在的文件
            if (RealLocal.Count > 0)
            {
                //异步制作MP4
                Task.Run(async () =>
                {
                    var catalog = Path.Combine(CommonHelper.DownloadPath, "WallPaper");
                    var res = await RealLocal.ImageToVideo(AudioName, Time, catalog);
                    if (res) Application.Current.Dispatcher.Invoke(() =>
                    {
                        new ScreenDownNofityView(CommonHelper.ConvertFinishInformation, catalog).Show();
                    });
                });
            }

        }
        #endregion

        #region Command
        public void ChangeCommand(int ActiveAnime)
        {
            this.InitPage = 1;
            this.Keyword = string.Empty;
            if (ActiveAnime != 4)
                OnInit();
            else
                CollectResult = new(Service.QueryAll());
        }

        public void RemoveCommand(Guid id)
        {
            CollectResult.Remove(CollectResult.First(t => t.PId == id));
            Service.Remove(id);
        }

        /// <summary>
        /// 正选
        /// </summary>
        /// <param name="param"></param>
        public void CheckCommand(WallModel param)
        {
            WallBuilder.Add(param);
        }

        /// <summary>
        /// 反选
        /// </summary>
        /// <param name="param"></param>
        public void UnCheckCommand(WallModel param)
        {
            WallBuilder.Remove(param);
        }

        /// <summary>
        /// 功能点
        /// </summary>
        /// <param name="mode"></param>
        public void ActiveCommand(string mode)
        {
            if (mode.Equals(Mode.ElementAt(0)))
            {
                Platform = PlatformEnum.Wallhaven;
                Result = [];
            }
            if (mode.Equals(Mode.ElementAt(1)))
            {
                Platform = PlatformEnum.Konachan;
                Result = [];
            }
            if (WallBuilder.Count <= 0) return;
            if (mode.Equals(Mode.ElementAt(2)))
                Download();
            if (mode.Equals(Mode.ElementAt(3)))
                Remove();
            if (mode.Equals(Mode.ElementAt(4)))
                BuildPicture();
            if (mode.Equals(Mode.ElementAt(5)))
                BuildAudio();
        }

        /// <summary>
        /// 加载更多
        /// </summary>
        public RelayCommand<ScrollChangedEventArgs> ScrollCommand => new((obj) =>
        {
            if (Keyword.IsNullOrEmpty())
            {
                if (InitPage <= InitTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    InitPage += 1;
                    OnLoadMoreInit();
                }
            }
            else
            {
                if (SearchPage <= SearchTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    SearchPage += 1;
                    OnLoadMoreSearch();
                }
            }
        });

        /// <summary>
        /// 收藏
        /// </summary>
        /// <param name="element"></param>
        public void CollectCommand(WallElementResult element)
        {
            var Model = element.ToMapest<WallModel>();
            Model.PId = Service.Insert(Model);
            CollectResult.Add(Model);
        }

        /// <summary>
        /// 切换模块
        /// </summary>
        public RelayCommand<object> ChangedCommand => new((item) =>
        {
            var Target = ((CandyToggleItem)item);
            if (Target.FindParent<UserControl>() is IndexView View)
            {
                var Index = Target.Tag.ToString().AsInt();

                if (Index == 0)
                {
                    Grad = GradEnum.SFW;
                    View.ActiveAnime = 1;
                    View.AnimeX1.Begin();
                }
                if (Index == 1)
                {
                    Grad = GradEnum.Sketchy;
                    View.ActiveAnime = 2;
                    View.AnimeX2.Begin();
                }
                if (Index == 2)
                {
                    Grad = GradEnum.NSFW;
                    View.ActiveAnime = 3;
                    View.AnimeX3.Begin();
                }
                if (Index == 3)
                {
                    View.ActiveAnime = 4;
                    View.AnimeX4.Begin();
                }
            }
        });
        #endregion

        #region ExternalCalls
        /// <summary>
        /// 检索数据
        /// </summary>
        /// <param name="keyword"></param>
        private void SearchHandler(string keyword)
        {
            this.Keyword = keyword;
            SearchPage = 1;
            OnSearch();
        }
        #endregion
    }
}
