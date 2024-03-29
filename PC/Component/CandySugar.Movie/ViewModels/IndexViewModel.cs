﻿using CandySugar.Com.Controls.ExtenControls;
using CandySugar.Com.Library.VisualTree;
using HandyControl.Controls;
using Org.BouncyCastle.Asn1.X509;
using System.DirectoryServices;
using System.Windows.Input;

namespace CandySugar.Movie.ViewModels
{
    public class IndexViewModel : PropertyChangedBase
    {
        private object LockObject = new object();
        public IndexViewModel()
        {
            Title = ["电影", "剧集"];
            OnInit();
        }

        #region Field
        public PlatformEnum Platform = PlatformEnum.Film;
        private string Route;
        private int Total;
        private int PageIndex = 1;
        private Dictionary<string, string> InitKey = new Dictionary<string, string>
        {
            {"T","" },{"Y",""},{"P",""},{ "C",""}
        };
        #endregion

        #region Property
        private ObservableCollection<string> _Title;
        public ObservableCollection<string> Title
        {
            get => _Title;
            set => SetAndNotify(ref _Title, value);
        }
        private ObservableCollection<string> _Year;
        public ObservableCollection<string> Year
        {
            get => _Year;
            set => SetAndNotify(ref _Year, value);
        }
        private ObservableCollection<string> _Plot;
        public ObservableCollection<string> Plot
        {
            get => _Plot;
            set => SetAndNotify(ref _Plot, value);
        }
        private ObservableCollection<string> _Country;
        public ObservableCollection<string> Country
        {
            get => _Country;
            set => SetAndNotify(ref _Country, value);
        }
        private ObservableCollection<MovieInitElementResult> _InitResult;
        public ObservableCollection<MovieInitElementResult> InitResult
        {
            get => _InitResult;
            set => SetAndNotify(ref _InitResult, value);
        }

        private MovieDetailRootResult _DetailResult;
        public MovieDetailRootResult DetailResult
        {
            get => _DetailResult;
            set => SetAndNotify(ref _DetailResult, value);
        }
        #endregion

        #region Command

        public void ChangeCommand(int ActiveAnime)
        {
            if (ActiveAnime == 1) Platform = PlatformEnum.Film;
            else Platform = PlatformEnum.Video;
            OnInit();
        }

        public RelayCommand<object> ChangedCommand => new((item) =>
        {
            PageIndex = 1;
            InitKey["P"] = InitKey["T"] = InitKey["Y"] = InitKey["C"] = string.Empty;
            var Target = ((CandyToggleItem)item);
            if (Target.FindParent<UserControl>() is IndexView View)
            {
                if (Target.Tag.ToString().AsInt() == 0)
                {
                    View.ActiveAnime=1;
                    View.AnimeX1.Begin();
                }
                else
                {
                    View.ActiveAnime = 2;
                    View.AnimeX2.Begin();
                }
            }
        });

        public RelayCommand<object> PlotChangedCommand => new((item) =>
        {
            var Target = ((CandyToggleItem)item);
            InitKey["T"] = Target.Content.ToString();
            InitKey["P"] = string.Empty;
            OnInit();
        });
        public RelayCommand<object> YearChangedCommand => new((item) =>
        {
            var Target = ((CandyToggleItem)item);
            InitKey["Y"] = Target.Content.ToString();
            InitKey["P"] = string.Empty;
            OnInit();
        });
        public RelayCommand<object> CountryChangedCommand => new((item) =>
        {
            var Target = ((CandyToggleItem)item);
            InitKey["C"] = Target.Content.ToString();
            InitKey["P"] = string.Empty;
            OnInit();
        });
        public RelayCommand<ScrollChangedEventArgs> ScrollCommand => new(obj =>
        {
            if (PageIndex <= Total && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
            {
                PageIndex += 1;
                InitKey["P"] = PageIndex.AsString();
                OnInit();
            }
        });

        public void DetailCommand(MovieInitElementResult input)
        {
            this.Route = input.Route;
            OnDetail();
            WeakReferenceMessenger.Default.Send(new MessageNotify());
        }

        #endregion

        #region Method
        private void OnInit()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    var result = (await MovieFactory.Movie(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MovieType = MovieEnum.Init,
                            Init = new MovieInit
                            {
                                PlatformType = Platform,
                                InitKey = InitKey
                            }
                        };
                    }).RunsAsync()).InitResult;
                    if (InitKey["P"].IsNullOrEmpty())
                    {
                        Year = new ObservableCollection<string>(result.Year);
                        Plot = new ObservableCollection<string>(result.Plot);
                        Country = new ObservableCollection<string>(result.Country);
                        InitResult = new ObservableCollection<MovieInitElementResult>(result.ElementResults);
                        Total = result.Total;
                    }
                    else
                    {
                        BindingOperations.EnableCollectionSynchronization(InitResult, LockObject);
                        Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(InitResult.Add));
                    }
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void OnDetail()
        {
            Task.Run(async () =>
            {
                try
                {
                    var Proxy = Module.IocModule.Proxy;
                    DetailResult = (await MovieFactory.Movie(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            ProxyIP = Proxy.IP,
                            ProxyPort = Proxy.Port,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MovieType = MovieEnum.Detail,
                            Detail = new MovieDetail
                            {
                                Route = Route
                            }
                        };
                    }).RunsAsync()).DetailResult;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void ErrorNotify()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new ScreenNotifyView(CommonHelper.ComponentErrorInformation).Show();
            });
        }
        #endregion
    }
}
