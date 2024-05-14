namespace CandySugar.Comic.ViewModels
{
    public class IndexViewModel : PropertyChangedBase
    {
        private object LockObject = new object();
        private IService<ComicModel> Service;
        public IndexViewModel()
        {
            Title = ["全部", "喜爱"];
            GenericDelegate.SearchAction = new(SearchHandler);
            Service = IocDependency.Resolve<IService<ComicModel>>();
            var LocalDATA = Service.QueryAll();
            CollectResult = [];
            LocalDATA?.ForEach(CollectResult.Add);
        }

        #region Field
        private int Total;
        private int PageIndex;
        private string Keyword;
        private string Route;
        #endregion

        #region Property
        private ObservableCollection<string> _Title;
        public ObservableCollection<string> Title
        {
            get => _Title;
            set => SetAndNotify(ref _Title, value);
        }

        private ObservableCollection<ComicModel> _CollectResult;
        public ObservableCollection<ComicModel> CollectResult
        {
            get => _CollectResult;
            set => SetAndNotify(ref _CollectResult, value);
        }

        private ObservableCollection<SearchElementResult> _SearchResult;
        public ObservableCollection<SearchElementResult> SearchResult
        {
            get => _SearchResult;
            set => SetAndNotify(ref _SearchResult, value);
        }

        private ObservableCollection<string> _Preview;
        public ObservableCollection<string> Preview
        {
            get => _Preview;
            set => SetAndNotify(ref _Preview, value);
        }

        private ObservableCollection<string> _View;
        public ObservableCollection<string> View
        {
            get => _View;
            set => SetAndNotify(ref _View, value);
        }

        private ObservableCollection<ViewTagRootResult> _TagResult;
        public ObservableCollection<ViewTagRootResult> TagResult 
        {
            get => _TagResult;
            set => SetAndNotify(ref _TagResult, value);
        }
        #endregion

        #region Method
        private void OnComicInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await ComicFactory.Comic(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            ComicType = ComicEnum.Search,
                            Search = new ComicSearch
                            {
                                Keyword = Keyword,
                                Page = 1
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    Total = result.Total;
                    SearchResult = new ObservableCollection<SearchElementResult>(result.Results);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnViewInit() 
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await ComicFactory.Comic(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            ComicType = ComicEnum.View,
                            Preview = new ComicPreview 
                            {
                                Route = Route
                            }
                        };
                    }).RunsAsync()).ViewResult;
                    TagResult = new ObservableCollection<ViewTagRootResult>(result.TagResult);
                    View = new ObservableCollection<string>(result.Views);
                    Preview= new ObservableCollection<string>(result.Previews);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }

        private void OnLoadMoreComicInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await ComicFactory.Comic(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            ComicType = ComicEnum.Search,
                            Search = new ComicSearch
                            {
                                Keyword = Keyword,
                                Page = PageIndex
                            }
                        };
                    }).RunsAsync()).SearchResult;
                    BindingOperations.EnableCollectionSynchronization(SearchResult, LockObject);
                    Application.Current.Dispatcher.Invoke(() => result.Results.ForEach(SearchResult.Add));
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

        public void ChangeCommand(int ActiveAnime)
        {
            if (SearchResult == null)
                OnComicInit();
        }
        /// <summary>
        /// 加载更多
        /// </summary>
        public RelayCommand<ScrollChangedEventArgs> ScrollCommand => new((obj) =>
        {
            if (PageIndex <= Total && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
            {
                PageIndex += 1;
                OnLoadMoreComicInit();
            }
        });

        public void CollectCommand(SearchElementResult element)
        {
            var Model = element.ToMapest<ComicModel>();
            Model.Id = Service.Insert(Model);
            CollectResult.Add(Model);
        }

        public void RemoveCommand(Guid id)
        {
            CollectResult.Remove(CollectResult.First(t=>t.Id==id));
            Service.Remove(id);
        }

        public void WatchCommand(string route)
        {
            this.Route = route;
            OnViewInit();
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
                ControlParam = View
            });
        }

        public void LinkCommand(ViewTagElementResult input)
        {
            if (!input.Valid) return;
            SearchHandler(input.Tag);
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
            OnComicInit();
        }
        #endregion

    }
}
