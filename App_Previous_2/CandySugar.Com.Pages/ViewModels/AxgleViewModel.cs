using CandySugar.Com.Library;
using CandySugar.Com.Library.Model;
using CandySugar.Com.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sdk.Component.Vip.Miss.Sdk;
using Sdk.Component.Vip.Miss.Sdk.ViewModel;
using Sdk.Component.Vip.Miss.Sdk.ViewModel.Enums;
using Sdk.Component.Vip.Miss.Sdk.ViewModel.Request;
using Sdk.Component.Vip.Miss.Sdk.ViewModel.Response;
using System.Collections.ObjectModel;
using XExten.Advance.IocFramework;
using XExten.Advance.LinqFramework;
using Application = Microsoft.Maui.Controls.Application;
using VideoView = CandySugar.Com.Pages.ChildViews.Axgles.VideoView;

namespace CandySugar.Com.Pages.ViewModels
{
    public partial class AxgleViewModel : ObservableObject
    {
        public AxgleViewModel()
        {
            Results = [];
            Platform = PlatformEnum.Jav;
            Bar = [
                new BarModel { Name = "Jav", Route = "Jav" },
                new BarModel { Name = "Skb", Route = "Skb" },
                new BarModel { Name = "最新", Route = "1" },
                new BarModel { Name = "热门", Route = "2" },
                new BarModel { Name = "好评", Route = "3" },
                ];
        }

        #region Field
        private PlatformEnum Platform;
        private ModeEnum Mode;
        private int InitTotal;
        private int InitPage;
        private int SearchPage;
        private int SearchTotal;
        #endregion

        #region Property
        [ObservableProperty]
        private ObservableCollection<BarModel> _Bar;
        [ObservableProperty]
        private ObservableCollection<MissElemetInitResult> _Results;
        [ObservableProperty]
        private string _QueryKey;
        #endregion

        #region Method
        private async void InitAsync()
        {
            try
            {
                var result = (await MissFactory.Miss(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        FuncType = FuncEnum.Init,
                        PlatformType = Platform,
                        CacheSpan = 5,
                        Init = new MissInit
                        {
                            ModeType = Mode,
                            Page = 1,
                        }
                    };
                }).RunsAsync()).InitResult;
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
                var result = (await MissFactory.Miss(opt =>
                {
                    opt.RequestParam = new Input
                    {

                        FuncType = FuncEnum.Init,
                        PlatformType = Platform,
                        CacheSpan = 5,
                        Init = new MissInit
                        {
                            ModeType = Mode,
                            Page = InitPage,
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
                var result = (await MissFactory.Miss(opt =>
                {
                    opt.RequestParam = new Input
                    {

                        FuncType = FuncEnum.Search,
                        PlatformType = Platform,
                        CacheSpan = 5,
                        Search = new MissSearch
                        {
                            Page = SearchPage,
                            Keyword = QueryKey
                        }
                    };
                }).RunsAsync()).SearchResult;

                if (SearchPage == 1)
                {
                    SearchTotal = result.Total;
                    Results = new(result.ElementResults.ToMapest<List<MissElemetInitResult>>());
                }
                else
                    result.ElementResults.ToMapest<List<MissElemetInitResult>>().ForEach(Results.Add);

            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }

        private async void PlayAsync(MissElemetInitResult input)
        {
            try
            {
                var result = (await MissFactory.Miss(opt =>
                {
                    opt.RequestParam = new Input
                    {

                        FuncType = FuncEnum.Detail,
                        PlatformType = Platform,
                        CacheSpan = 5,
                        Play = new  MissPlay
                        {
                            Route = input.Route
                        }
                    };
                }).RunsAsync()).PlayResult;
                await Shell.Current.GoToAsync(Extend.RouteMap[nameof(VideoView)], new Dictionary<string, object> { { "Param", result.Play }, { "Is24Net", false } });
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }

        private async void Insert(MissElemetInitResult result)
        {
            await IocDependency.Resolve<ICandyService>().Add(new CollectModel
            {
                Category = 2,
                Cover = result.Cover,
                Name = result.Title,
                Route = result.Route,
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
        public void Query() 
        {
            if (QueryKey.IsNullOrEmpty()) return;
            SearchPage = 1;
            SearchTotal = 0;
            Results = [];
            Application.Current.Dispatcher.DispatchAsync(SearchAsync);
        }

        public RelayCommand<string> CatalogCommand => new(obj =>
        {
            Results = [];
            if (obj == "Jav")
            {
                Platform = PlatformEnum.Jav;
            }
            else if (obj == "Skb")
            {
                Platform = PlatformEnum.Skb;
            }
            else
            {
                var type = obj.AsInt();
                if (type == 1)
                    Mode = ModeEnum.Latest;
                else if (type == 2)
                    Mode = ModeEnum.Hot;
                else
                    Mode = ModeEnum.Praised;
                InitPage = 1;
                QueryKey = string.Empty;
                InitAsync();
            }
        });

        public RelayCommand<MissElemetInitResult> CollectCommand => new(Insert);

        public RelayCommand<MissElemetInitResult> PlayCommand => new(PlayAsync);
        #endregion
    }
}
