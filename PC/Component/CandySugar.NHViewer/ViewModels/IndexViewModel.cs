namespace CandySugar.NHViewer.ViewModels
{
    public class IndexViewModel : PropertyChangedBase
    {
        private object LockObject = new object();
        private IService<NHentaiModel> Service;
        public IndexViewModel()
        {
            Title = ["全部", "喜爱"];
            GenericDelegate.SearchAction = new(SearchHandler);
            Service = IocDependency.Resolve<IService<NHentaiModel>>();
            var LocalDATA = Service.QueryAll();
            CollectResult = [];
            LocalDATA?.ForEach(CollectResult.Add);
            OnInit();
            HttpSchedule.ReceiveAction += ReceiveProcess;
        }

        #region Field
        private int Total;
        private int PageIndex;
        private string Keyword;
        private double Counts = 0;
        private string Catalog = SyncStatic.CreateDir(Path.Combine(CommonHelper.DownloadPath, "NHentai"));
        private bool IsDown = false;
        #endregion

        #region Property
        private ObservableCollection<string> _Title;
        public ObservableCollection<string> Title
        {
            get => _Title;
            set => SetAndNotify(ref _Title, value);
        }

        private ObservableCollection<NHentaiModel> _CollectResult;
        public ObservableCollection<NHentaiModel> CollectResult
        {
            get => _CollectResult;
            set => SetAndNotify(ref _CollectResult, value);
        }

        private ObservableCollection<NHentaiModel> _Results;
        public ObservableCollection<NHentaiModel> Results
        {
            get => _Results;
            set => SetAndNotify(ref _Results, value);
        }

        private NHentaiModel _Result;
        public NHentaiModel Result
        {
            get => _Result;
            set => SetAndNotify(ref _Result, value);
        }
        #endregion

        #region Command
        public RelayCommand<object> ChangedCommand => new((item) =>
        {
            var Target = ((CandyToggleItem)item);
            if (Target.FindParent<UserControl>() is IndexView View)
            {
                if (Target.Tag.ToString().AsInt() == 0)
                {
                    View.ActiveAnime = 1;
                    View.AnimeX1.Begin();
                }
                else
                {
                    View.ActiveAnime = 2;
                    View.AnimeX2.Begin();
                }
            }
        });

        /// <summary>
        /// 加载更多
        /// </summary>
        public RelayCommand<ScrollChangedEventArgs> ScrollCommand => new((obj) =>
        {
            if (this.Keyword.IsNullOrEmpty())
            {
                if (PageIndex <= Total && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    PageIndex += 1;
                    OnLoadMoreInit();
                }
            }
            else
            {
                if (PageIndex <= Total && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    PageIndex += 1;
                    OnLoadMoreSearch();
                }
            }
        });

        public void CollectCommand(InitElementResult input)
        {
            var Model = input.ToMapest<NHentaiModel>();
            Model.PId = Service.Insert(Model);
            CollectResult.Add(Model);
        }

        public void RemoveCommand(Guid id)
        {
            CollectResult.Remove(CollectResult.First(t => t.PId == id));
            Service.Remove(id);
        }

        public void WatchCommand(NHentaiModel input)
        {
            Result = input;
            WeakReferenceMessenger.Default.Send(new MessageNotify
            {
                NotifyType = NotifyType.Notify
            });
        }

        public void ViewCommand()
        {
            WeakReferenceMessenger.Default.Send(new MessageNotify
            {
                NotifyType = NotifyType.ChangeControl,
                ControlParam = Result.OriginImages
            });
        }

        public void DownCommand() 
        {
            ErrorNotify(CommonHelper.DownloadWait);
            Download();
        }
        #endregion

        #region Method
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
                            Init = new PandaInit()
                        };
                    }).RunsAsync()).InitResult;
                    Total = result.TotalPage;
                    Results = new ObservableCollection<NHentaiModel>(result.Results.ToMapest<List<NHentaiModel>>());
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
                            Init = new PandaInit
                            {
                                Page = PageIndex
                            }
                        };
                    }).RunsAsync()).InitResult;
                    BindingOperations.EnableCollectionSynchronization(Results, LockObject);
                    Application.Current.Dispatcher.Invoke(() => result.Results.ToMapest<List<NHentaiModel>>().ForEach(Results.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        public async void OnitSearch()
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
                            PandaType = PandaEnum.Search,
                            Search = new PandaSearch
                            {
                                Keyword = Keyword
                            }
                        };
                    }).RunsAsync()).InitResult;
                    Total = result.TotalPage;
                    Results = new ObservableCollection<NHentaiModel>(result.Results.ToMapest<List<NHentaiModel>>());
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        public async void OnLoadMoreSearch()
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
                            PandaType = PandaEnum.Search,
                            Search = new PandaSearch
                            {
                                Keyword = Keyword,
                                Page = PageIndex
                            }
                        };
                    }).RunsAsync()).InitResult;
                    BindingOperations.EnableCollectionSynchronization(Results, LockObject);
                    Application.Current.Dispatcher.Invoke(() => result.Results.ToMapest<List<NHentaiModel>>().ForEach(Results.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void ErrorNotify(string Info = "")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new ScreenNotifyView(Info.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : Info).Show();
            });
        }

        private async void Download()
        {
            if (Result != null && IsDown == false)
            {
                Dictionary<string, string> data = new Dictionary<string, string>();
                for (int index = 0; index < Result.ImageType.Count; index++)
                {
                    var fullName = Path.Combine(Catalog, $"{index + 1}.{Result.ImageType[index]}");

                    data.Add(fullName, Result.OriginImages[index]);
                }
                await HttpSchedule.HttpDownload(data);
                IsDown = true;
            }
        }
        private void ReceiveProcess(double item, double num)
        {
            if (item == double.Parse((100 / num).ToString("F2")))
                Counts += item;
            if (Math.Ceiling(Counts) >= 100)
            {
                Application.Current.Dispatcher.Invoke(() => new ScreenDownNofityView(CommonHelper.DownloadFinishInformation, Catalog).Show());
                IsDown = false;
            }
        }
        #endregion

        #region ExternalCalls
        /// <summary>
        /// 检索数据
        /// </summary>
        /// <param name="keyword"></param>
        private void SearchHandler(string keyword)
        {
            this.Keyword = keyword;
            PageIndex = 1;
            if (!this.Keyword.IsNullOrEmpty()) OnitSearch();
            else OnInit();
        }
        #endregion
    }
}
