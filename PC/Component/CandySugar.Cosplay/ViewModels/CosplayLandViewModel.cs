using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.JsonDbFramework;

namespace CandySugar.Cosplay.ViewModels
{
    public class CosplayLandViewModel: PropertyChangedBase
    {
        private object LockObject = new object();
        public List<CosplayInitElementResult> Builder;
        private JsonDbHandle<CosplayInitElementResult> JsonHandler;
        private string DbPath = Path.Combine(CommonHelper.DownloadPath, "Cosplay", $"CosplayLand.{FileTypes.Dat}");
        public CosplayLandViewModel()
        {
            Title = ["常规", "收藏"];
            JsonHandler = new JsonDbContext(DbPath).LoadInMemory<CosplayInitElementResult>();
            var LocalDATA = JsonHandler.GetAll();
            CollectResult = new ObservableCollection<CosplayInitElementResult>();
            if (LocalDATA != null)
            {
                LocalDATA.ForEach(CollectResult.Add);
            }
        }

        #region Field
        private string Keyword;
        private int GeneralTotal;
        private int GeneralPageIndex = 1;
        /// <summary>
        /// 1：常规  2：收藏
        /// </summary>
        private int ChangeType = 1;
        #endregion

        #region Property
        private ObservableCollection<string> _Title;
        public ObservableCollection<string> Title
        {
            get => _Title;
            set => SetAndNotify(ref _Title, value);
        }

        private ObservableCollection<CosplayInitElementResult> _CosResult;
        public ObservableCollection<CosplayInitElementResult> CosResult
        {
            get => _CosResult;
            set => SetAndNotify(ref _CosResult, value);
        }

        private ObservableCollection<CosplayInitElementResult> _CollectResult;
        public ObservableCollection<CosplayInitElementResult> CollectResult
        {
            get => _CollectResult;
            set => SetAndNotify(ref _CollectResult, value);
        }
        #endregion

        #region Command
        public RelayCommand<object> ChangedCommand => new((item) =>
        {
            var Target = ((CandyToggleItem)item);
            if (Target.FindParent<UserControl>() is CosplayLandView View)
            {
                var Index = Target.Tag.ToString().AsInt();

                if (Index == 0)
                {
                    View.ActiveAnime = 1;
                    View.AnimeY1.Begin();
                }
                if (Index == 1)
                {
                    View.ActiveAnime = 2;
                    View.AnimeY2.Begin();
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
            if (ChangeType == 1 && CosResult == null)
                OnCosInit();
        }
        /// <summary>
        /// 加载更多
        /// </summary>
        public RelayCommand<ScrollChangedEventArgs> ScrollCommand => new((obj) =>
        {
            if (ChangeType == 1)
            {
                if (GeneralPageIndex <= GeneralTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    GeneralPageIndex += 1;
                    OnLoadMoreCosInit();
                }
            }
        });
        /// <summary>
        /// 收藏
        /// </summary>
        /// <param name="element"></param>
        public void CollectCommand(CosplayInitElementResult element)
        {
            CollectResult.Add(element);
            JsonHandler.Insert(element).ExuteInsert().SaveChange();
        }
        public void CheckCommand(CosplayInitElementResult input)
        {
            Builder.Add(input);
            GenericDelegate.HandleAction?.Invoke(Builder);
        }
        public void UnCheckCommand(CosplayInitElementResult input)
        {
            Builder.Remove(input);
            GenericDelegate.HandleAction?.Invoke(Builder);
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
                            PlatformType = PlatformEnum.Land,
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
                            PlatformType = PlatformEnum.Land,
                            Init = new CosplayInit
                            {
                                Page = GeneralPageIndex
                            }
                        };
                    }).RunsAsync()).InitResult;
                    BindingOperations.EnableCollectionSynchronization(CosResult, LockObject);
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(CosResult.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        #endregion
    }
}
