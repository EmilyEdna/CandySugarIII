using System.Collections.ObjectModel;
using CandySugar.Com.Library;
using CandySugar.Com.Pages.ChildViews.Comics;
using CandySugar.Com.Pages.Views;
using CandySugar.Com.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sdk.Component.Vip.Comic.sdk;
using Sdk.Component.Vip.Comic.sdk.ViewModel;
using Sdk.Component.Vip.Comic.sdk.ViewModel.Enums;
using Sdk.Component.Vip.Comic.sdk.ViewModel.Request;
using Sdk.Component.Vip.Comic.sdk.ViewModel.Response;
using XExten.Advance.IocFramework;

namespace CandySugar.Com.Pages.ChildViewModels.Comics
{
    public partial class CatalogViewModel : ObservableObject, IQueryAttributable
    {

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Result = (SearchElementResult)query["Param"];
            Application.Current.Dispatcher.DispatchAsync(PreviewAsync);
        }

        #region Property
        [ObservableProperty]
        private SearchElementResult _Result;
        [ObservableProperty]
        private ObservableCollection<string> _Preview;
        [ObservableProperty]
        private ObservableCollection<string> _View;
        [ObservableProperty]
        private ObservableCollection<ViewTagRootResult> _Tags;
        #endregion

        #region Method
        private async void PreviewAsync()
        {
            try
            {
                var result = (await ComicFactory.Comic(opt =>
                {
                    opt.RequestParam = new Input
                    {

                        ComicType = ComicEnum.View,
                        Preview = new ComicPreview
                        {
                            Route = Result.Route
                        }
                    };
                }).RunsAsync()).ViewResult;
                Tags = new ObservableCollection<ViewTagRootResult>(result.TagResult);
                View = new ObservableCollection<string>(result.Views);
                Preview = new ObservableCollection<string>(result.Previews);
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }
        private async void Next(string input)
        {
            var Index = Preview.ToList().FindIndex(t => t.Equals(input));
            await Shell.Current.GoToAsync(Extend.RouteMap[nameof(VisitView)], new Dictionary<string, object> {
                { "Index",Index},{"Param" ,View.ToList() }
            });
        }
        private async void Insert(SearchElementResult result)
        {
            await IocDependency.Resolve<ICandyService>().Add(new CollectModel
            {
                Category = 1,
                Cover = result.Cover,
                Name = result.Name,
                Route = result.Route,
            });
        }
        private async void Query(ViewTagElementResult input)
        {
            if (!input.Valid) return;
            await Shell.Current.GoToAsync(Extend.RouteMap[nameof(ComicView)], new Dictionary<string, object> { {"Tag" ,input.Tag } });
        }
        #endregion

        #region Command
        public RelayCommand<string> WatchCommand => new(Next);
        public RelayCommand<ViewTagElementResult> LinkCommand => new(Query);
        public RelayCommand LoveCommand => new(() => Insert(Result));
        #endregion
    }
}
