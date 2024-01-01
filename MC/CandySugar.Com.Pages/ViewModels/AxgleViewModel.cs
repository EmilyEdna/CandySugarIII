using CandySugar.Com.Library;
using CandySugar.Com.Library.Model;
using CandySugar.Com.Pages.ChildViews.Axgles;
using CandySugar.Com.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sdk.Component.Vip.Axgle.sdk;
using Sdk.Component.Vip.Axgle.sdk.ViewModel;
using Sdk.Component.Vip.Axgle.sdk.ViewModel.Enums;
using Sdk.Component.Vip.Axgle.sdk.ViewModel.Request;
using Sdk.Component.Vip.Axgle.sdk.ViewModel.Response;
using System.Collections.ObjectModel;
using XExten.Advance.IocFramework;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages.ViewModels
{
    public partial class AxgleViewModel : ObservableObject
    {
        public AxgleViewModel()
        {
            Application.Current.Dispatcher.DispatchAsync(InitAsync);
        }

        #region Field
        private int CId;
        private int Total;
        private int Page=0;
        #endregion

        #region Property
        [ObservableProperty]
        private ObservableCollection<BarModel> _Bar;
        [ObservableProperty]
        private ObservableCollection<AxgleCategoryElementResult> _CateResult;
        [ObservableProperty]
        private string _QueryKey;
        #endregion

        #region Method
        private async void InitAsync()
        {
            try
            {
                var result = (await AxgleFactory.Axgle(opt =>
                {
                    opt.RequestParam = new Input
                    {

                        AxgleType = AxgleEnum.Init,
                        CacheSpan = 5,
                    };
                }).RunsAsync()).InitResults;
                var data = result.Select(t => new BarModel
                {
                    Name = t.ShortName,
                    Route = t.AId
                }).ToList();
                Bar = new(data);

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
                var result = (await AxgleFactory.Axgle(opt =>
                {
                    opt.RequestParam = new Input
                    {

                        AxgleType = AxgleEnum.Search,
                        CacheSpan = 5,
                        Search = new AxgleSearch { 
                         Page=Page,
                         KeyWord=QueryKey
                        }
                    };
                }).RunsAsync()).SearchResult;

                if (Page == 0)
                {
                    Total = result.Total;
                    CateResult = new(result.ElementResult.ToMapest<List<AxgleCategoryElementResult>>());
                }
                else
                    result.ElementResult.ToMapest<List<AxgleCategoryElementResult>>().ForEach(CateResult.Add);

            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }
        private async void CateAsync()
        {
            try
            {
                var result = (await AxgleFactory.Axgle(opt =>
                {
                    opt.RequestParam = new Input
                    {

                        AxgleType = AxgleEnum.Category,
                        CacheSpan = 5,
                        Category= new AxgleCategory { 
                         Desc= AxgleDescEnum.MostViewed,
                         CId= CId,
                         Page=Page
                        }
                    };
                }).RunsAsync()).CategoryResult;
                if (Page == 0)
                {
                    Total = result.Total;
                    CateResult = new(result.ElementResult);
                }else
                    result.ElementResult.ForEach(CateResult.Add);
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }

        private async void PlayAsync(string route) 
        {

            try
            {
                var result = (await AxgleFactory.Axgle(opt =>
                {
                    opt.RequestParam = new Input
                    {

                        AxgleType = AxgleEnum.Detail,
                        CacheSpan = 5,
                        Detail = new AxgleDetail
                        {
                            FrameURL = route
                        }
                    };
                }).RunsAsync()).DetailResult.Route;

                await Shell.Current.GoToAsync(Extend.RouteMap[nameof(VideoView)], new Dictionary<string, object> { { "Param", result } });
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }

        private async void Insert(AxgleCategoryElementResult result)
        {
            await IocDependency.Resolve<ICandyService>().Add(new CollectModel
            {
                Category = 6,
                Cover = result.Preview,
                Name = result.Title,
                Route = result.Play,
            });
        }

        #endregion

        #region Command
        public RelayCommand MoreCommand => new(() =>
        {
            if (QueryKey.IsNullOrEmpty())
            {
                Page += 1;
                if (Page <= Total)
                    Application.Current.Dispatcher.DispatchAsync(CateAsync);
            }
            else
            {
                Page += 1;
                if (Page <= Total)
                    Application.Current.Dispatcher.DispatchAsync(SearchAsync);
            }
        });

        public RelayCommand QueryCommand => new(() =>
        {
            if (QueryKey.IsNullOrEmpty()) return;
            Page = 0;
            Application.Current.Dispatcher.DispatchAsync(SearchAsync);
        });

        public RelayCommand<string> CatalogCommand => new(obj => {
            CId = obj.AsInt();
            Page = 0;
            Application.Current.Dispatcher.DispatchAsync(CateAsync);
        });

        public RelayCommand<AxgleCategoryElementResult> CollectCommand => new(Insert);

        public RelayCommand<string> PlayCommand => new(PlayAsync);
        #endregion
    }
}
