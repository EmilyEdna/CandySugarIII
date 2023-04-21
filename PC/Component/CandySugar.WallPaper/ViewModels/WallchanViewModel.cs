using Sdk.Component.Image.sdk.ViewModel;

namespace CandySugar.WallPaper.ViewModels
{
    public class WallchanViewModel : PropertyChangedBase
    {
        private object LockObject = new object();
        private List<ImageElementResult> Builder;
        public WallchanViewModel()
        {
            GenericDelegate.SearchAction = new(SearchHandler);
            var LocalDATA = DownUtil.ReadFile<List<ImageElementResult>>("Konachan", FileTypes.Dat, "WallPaper");
            CollectResult = new ObservableCollection<ImageElementResult>();
            if (LocalDATA != null)
            {
                LocalDATA.ForEach(CollectResult.Add);
            }
            Builder = new List<ImageElementResult>();
        }

        #region Field
        private int Limit = 12;
        private string Keyword;
        private int GeneralTotal;
        private int GeneralPageIndex = 1;
        private int OrdinaryTotal;
        private int OrdinaryPageIndex = 1;
        private int FishyTotal;
        private int FishyPageIndex = 1;
        /// <summary>
        /// 1：常规 2：一般 3：可疑 4：收藏
        /// </summary>
        private int ChangeType = 1;
        #endregion

        #region Property
        private ObservableCollection<ImageElementResult> _GeneralResult;
        public ObservableCollection<ImageElementResult> GeneralResult
        {
            get => _GeneralResult;
            set => SetAndNotify(ref _GeneralResult, value);
        }

        private ObservableCollection<ImageElementResult> _OrdinaryResult;
        public ObservableCollection<ImageElementResult> OrdinaryResult
        {
            get => _OrdinaryResult;
            set => SetAndNotify(ref _OrdinaryResult, value);
        }

        private ObservableCollection<ImageElementResult> _FishyResult;
        public ObservableCollection<ImageElementResult> FishyResult
        {
            get => _FishyResult;
            set => SetAndNotify(ref _FishyResult, value);
        }
        private ObservableCollection<ImageElementResult> _CollectResult;
        public ObservableCollection<ImageElementResult> CollectResult
        {
            get => _CollectResult;
            set => SetAndNotify(ref _CollectResult, value);
        }
        #endregion

        #region Method
        private void OnGeneralInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await ImageFactory.Image(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            ImageType = ImageEnum.Search,
                            Search = new ImageSearch
                            {
                                Limit = this.Limit,
                                Page = 1,
                                KeyWord = Expert(1)
                            }
                        };
                    }).RunsAsync()).GlobalResult;
                    GeneralTotal = result.Total;
                    GeneralResult = new ObservableCollection<ImageElementResult>(result.Result);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void OnOrdinaryInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await ImageFactory.Image(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            ImageType = ImageEnum.Search,
                            Search = new ImageSearch
                            {
                                Limit = this.Limit,
                                Page = 1,
                                KeyWord = Expert(2)
                            }

                        };
                    }).RunsAsync()).GlobalResult;
                    OrdinaryTotal = result.Total;
                    OrdinaryResult = new ObservableCollection<ImageElementResult>(result.Result);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void OnFishyInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await ImageFactory.Image(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            ImageType = ImageEnum.Search,
                            Search = new ImageSearch
                            {
                                Limit = this.Limit,
                                Page = 1,
                                KeyWord = Expert(3)
                            }

                        };
                    }).RunsAsync()).GlobalResult;
                    FishyTotal = result.Total;
                    FishyResult = new ObservableCollection<ImageElementResult>(result.Result);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void OnLoadMoreGeneralInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await ImageFactory.Image(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            ImageType = ImageEnum.Search,
                            Search = new ImageSearch
                            {
                                Limit = this.Limit,
                                Page = GeneralPageIndex,
                                KeyWord = Expert(1)
                            }

                        };
                    }).RunsAsync()).GlobalResult;
                    BindingOperations.EnableCollectionSynchronization(GeneralResult, LockObject);
                    Application.Current.Dispatcher.Invoke(() => result.Result.ForEach(GeneralResult.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void OnLoadMoreOrdinaryInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await ImageFactory.Image(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            ImageType = ImageEnum.Search,
                            Search = new ImageSearch
                            {
                                Limit = this.Limit,
                                Page = OrdinaryPageIndex,
                                KeyWord = Expert(2)
                            }

                        };
                    }).RunsAsync()).GlobalResult;
                    BindingOperations.EnableCollectionSynchronization(OrdinaryResult, LockObject);
                    Application.Current.Dispatcher.Invoke(() => result.Result.ForEach(OrdinaryResult.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void OnLoadMoreFishyInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await ImageFactory.Image(opt =>
                    {
                        opt.RequestParam = new Input
                        {

                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            ImageType = ImageEnum.Search,
                            Search = new ImageSearch
                            {
                                Limit = this.Limit,
                                Page = FishyPageIndex,
                                KeyWord = Expert(3)
                            }

                        };
                    }).RunsAsync()).GlobalResult;
                    BindingOperations.EnableCollectionSynchronization(FishyResult, LockObject);
                    Application.Current.Dispatcher.Invoke(() => result.Result.ForEach(FishyResult.Add));
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

        private string Expert(int type)
        {
            if (type == 1) return !this.Keyword.IsNullOrEmpty()? $"{this.Keyword} rating:safe": "rating:safe";
            else if (type == 2) return  !this.Keyword.IsNullOrEmpty() ? $"{this.Keyword} rating:questionable": "rating:questionable";
            else return  !this.Keyword.IsNullOrEmpty() ? $"{this.Keyword} rating:explicit": "rating:explicit";
        }
        #endregion

        #region Command

        /// <summary>
        /// 加载更多
        /// </summary>
        public RelayCommand<ScrollChangedEventArgs> ScrollCommand => new((obj) =>
        {
            if (ChangeType == 1)
            {
                if (GeneralPageIndex <= FishyTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    GeneralPageIndex += 1;
                    OnLoadMoreGeneralInit();
                }
            }
            if (ChangeType == 2)
            {
                if (OrdinaryPageIndex <= OrdinaryTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    OrdinaryPageIndex += 1;
                    OnLoadMoreOrdinaryInit();
                }
            }
            if (ChangeType == 3)
            {
                if (FishyPageIndex <= GeneralTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    FishyPageIndex += 1;
                    OnLoadMoreFishyInit();
                }
            }
        });
        /// <summary>
        /// 切换
        /// </summary>
        /// <param name="type"></param>
        public void ChangeCommand(int type)
        {
            ChangeType = type;
            if (ChangeType == 1 && GeneralResult == null)
                OnGeneralInit();
            if (ChangeType == 2 && OrdinaryResult == null)
                OnOrdinaryInit();
            if (ChangeType == 3 && FishyResult == null)
                OnFishyInit();
        }
        /// <summary>
        /// 收藏
        /// </summary>
        /// <param name="element"></param>
        public void CollectCommand(ImageElementResult element)
        {
            CollectResult.Add(element);
            CollectResult.ToList().DeleteAndCreate("Konachan", FileTypes.Dat, "WallPaper");
        }

        public void CheckCommand(ImageElementResult element)
        {
            Builder.Add(element);
            GenericDelegate.HandleAction?.Invoke(Builder);
        }
        public void UnCheckCommand(ImageElementResult element)
        {
            Builder.Remove(element);
            GenericDelegate.HandleAction?.Invoke(Builder);
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
            GeneralPageIndex = OrdinaryPageIndex = FishyPageIndex = 1;
            if (ChangeType == 1)
                OnGeneralInit();
            if (ChangeType == 2)
                OnOrdinaryInit();
            if (ChangeType == 3)
                OnFishyInit();
        }
        #endregion
    }
}
