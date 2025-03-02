using System.Collections.ObjectModel;
using CandySugar.Com.Library;
using CandySugar.Com.Library.Model;
using CandySugar.Com.Pages.ChildViews.Axgles;
using CandySugar.Com.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sdk.Component.Vip.Jron.sdk;
using Sdk.Component.Vip.Jron.sdk.ViewModel;
using Sdk.Component.Vip.Jron.sdk.ViewModel.Enums;
using Sdk.Component.Vip.Jron.sdk.ViewModel.Request;
using Sdk.Component.Vip.Jron.sdk.ViewModel.Response;
using XExten.Advance.IocFramework;
using XExten.Advance.LinqFramework;
using Application = Microsoft.Maui.Controls.Application;

namespace CandySugar.Com.Pages.ViewModels
{
    public partial class AvgleViewModel : ObservableObject
    {
        public AvgleViewModel()
        {
            Results = [];
            Mode = ModeEnum.ReleaseDate;
            Platform = PlatformEnum.A24;
            InitDict();
            InitAsync();
        }

        #region Field
        private PlatformEnum Platform;
        private ModeEnum Mode;
        private int InitTotal;
        private int InitPage;
        private int SearchPage;
        private int SearchTotal;
        private Dictionary<string, List<string>> TagDict;
        #endregion

        #region Property
        [ObservableProperty]
        private ObservableCollection<BarModel> _Bar;
        [ObservableProperty]
        private ObservableCollection<JronElemetInitResult> _Results;
        [ObservableProperty]
        private string _QueryKey;
        [ObservableProperty]
        private string _Tag;
        [ObservableProperty]
        private ObservableCollection<string> _Tags;
        #endregion

        #region Event
        private void InitDict()
        {
            Bar = new ObservableCollection<BarModel>();
            typeof(ModeEnum).GetEnumNames()
                .ForArrayEach<string>(item =>
                {
                    var Mode = Enum.Parse<ModeEnum>(item);
                    Bar.Add(new BarModel { Name = Mode.ToDes(), Route = item });
                });
        }
        public void Changed(bool input)
        {
            if (TagDict==null) return;
            if (input)
                Tags = [.. TagDict.FirstOrDefault().Value];
            else
                Tags = [.. TagDict.LastOrDefault().Value];
        }
        #endregion

        #region Method


        private async void InitAsync()
        {
            try
            {
                var result = (await JronFactory.Jron(opt =>
                {
                    opt.RequestParam = new Input
                    {

                        JronType = JronEnum.Init,
                        PlatformType = Platform,
                        CacheSpan = 5,
                        Init = new JronInit
                        {
                            ModeType = Mode,
                            Page = 1,
                            Tag = Tag
                        }
                    };
                }).RunsAsync()).InitResult;
                TagDict ??= result.Tags;
                InitTotal = result.Total;
                Results = new(result.ElementResults);
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }

        private async void InitMoreAsync()
        {
            try
            {
                var result = (await JronFactory.Jron(opt =>
                {
                    opt.RequestParam = new Input
                    {

                        JronType = JronEnum.Init,
                        PlatformType = Platform,
                        CacheSpan = 5,
                        Init = new JronInit
                        {
                            ModeType = Mode,
                            Page = InitPage,
                            Tag = Tag
                        }
                    };
                }).RunsAsync()).InitResult;
                result.ElementResults.ForEach(Results.Add);
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }

        private async void SearchAsync()
        {
            try
            {
                var result = (await JronFactory.Jron(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        JronType = JronEnum.Search,
                        PlatformType = Platform,
                        CacheSpan = 5,
                        Search = new JronSearch
                        {
                            Page = SearchPage,
                            Keyword = QueryKey,
                            ModeType = Mode,
                        }
                    };
                }).RunsAsync()).SearchResult;

                if (SearchPage == 1)
                {
                    SearchTotal = result.Total;
                    Results = new(result.ElementResults.ToMapest<List<JronElemetInitResult>>());
                }
                else
                    result.ElementResults.ToMapest<List<JronElemetInitResult>>().ForEach(Results.Add);

            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }

        private async void PlayAsync(JronElemetInitResult input)
        {
            try
            {
                var result = (await JronFactory.Jron(opt =>
                {
                    opt.RequestParam = new Input
                    {

                        JronType = JronEnum.Detail,
                        PlatformType = Platform,
                        CacheSpan = 5,
                        Play = new JronPlay
                        {
                            Route = input.Route
                        }
                    };
                }).RunsAsync()).PlayResult;
                var Params = new Dictionary<string, object>();
                Params.Add("Title", input.Title);
                Params.Add("Result", result.Plays);
                Params.Add("Links" ,result.ElementResults);
                await Shell.Current.GoToAsync(Extend.RouteMap[nameof(LinkView)], new Dictionary<string, object> { { "Param", Params } });
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }

        private async void Insert(JronElemetInitResult result)
        {
            var res = (await JronFactory.Jron(opt =>
            {
                opt.RequestParam = new Input
                {

                    JronType = JronEnum.Detail,
                    PlatformType = Platform,
                    CacheSpan = 5,
                    Play = new JronPlay
                    {
                        Route = result.Route
                    }
                };
            }).RunsAsync()).PlayResult;

            res.Plays.ForDicEach((k,v,i) =>
            {
                IocDependency.Resolve<ICandyService>().Add(new CollectModel
                {
                    Category = 3,
                    Cover = result.Cover,
                    Name = result.Title+$"-{i+1}",
                    Route = v,
                }).Wait();
            });
        }

        #endregion

        #region Command
        [RelayCommand]
        public void More()
        {
            if (QueryKey.IsNullOrEmpty())
            {
                InitPage += 1;
                if (InitPage <= InitTotal)
                    Application.Current.Dispatcher.DispatchAsync(InitMoreAsync);
            }
            else
            {
                SearchPage += 1;
                if (SearchPage <= SearchTotal)
                    Application.Current.Dispatcher.DispatchAsync(SearchAsync);
            }
        }
        [RelayCommand]
        public void Reset() 
        {
            Results = [];
            InitPage = 1;
            QueryKey = string.Empty;
            InitAsync();
        }

        [RelayCommand]
        public void Query()
        {
            if (QueryKey.IsNullOrEmpty()) return;
            SearchPage = 1;
            SearchTotal = 0;
            Results = [];
            Application.Current.Dispatcher.DispatchAsync(SearchAsync);
        }

        public RelayCommand<JronElemetInitResult> PlayCommand => new(PlayAsync);

        public RelayCommand<JronElemetInitResult> CollectCommand => new(Insert);

        public RelayCommand<string> CatalogCommand => new(obj =>
        {
            Mode = Enum.Parse<ModeEnum>(obj);
            InitAsync();
        });
        #endregion
    }
}
