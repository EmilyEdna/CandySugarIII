using System.Collections.ObjectModel;
using CandySugar.Com.Library;
using CandySugar.Com.Library.Model;
using CandySugar.Com.Pages.ChildViews.Comics;
using CandySugar.Com.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ComicSearch = Sdk.Component.Vip.Comic.sdk.ViewModel.Response.SearchElementResult;
using RifanSearch = Sdk.Component.Vip.Anime.sdk.ViewModel.Response.SearchElementResult;
using XExten.Advance.IocFramework;
using XExten.Advance.LinqFramework;
using CandySugar.Com.Pages.ChildViews.Rifans;
using CandySugar.Com.Pages.ChildViews.Novels;
using CandySugar.Com.Pages.ChildViews.Animes;
using Sdk.Component.Cart.sdk.ViewModel.Response;
using CandySugar.Com.Pages.ChildViews.Lights;
using CandySugar.Com.Pages.ChildViews.Axgles;

namespace CandySugar.Com.Pages.ViewModels
{
    public partial class RootViewModel : ObservableObject
    {

        public RootViewModel()
        {
            Bar = new ObservableCollection<BarModel>
            {
               new BarModel{ Name="漫画", Route="1" },
               new BarModel{ Name="里番", Route="2" },
               new BarModel{ Name="小说", Route="3" },
               new BarModel{ Name="动漫", Route="4" },
               new BarModel{ Name="文库", Route="5" },
               new BarModel{ Name="车牌", Route="6" },
            };
            GetLocalData();
        }

        #region Field
        private int Category = 4;
        private int Page = 1;
        private int Total;
        #endregion

        #region Property
        [ObservableProperty]
        private ObservableCollection<BarModel> _Bar;
        [ObservableProperty]
        private ObservableCollection<CollectModel> _Data;
        #endregion

        #region Method
        private async void GetLocalData()
        {
            var data = await IocDependency.Resolve<ICandyService>().Get(Category, Page);
            if (Page <= 1)
            {
                Total = data.Item1;
                Data = new ObservableCollection<CollectModel>(data.Item2);
            }
            else data.Item2.ForEach(Data.Add);
        }
        private async void DeleteLocalData(Guid Id)
        {
            await IocDependency.Resolve<ICandyService>().Delete(Id);
            Page = 1;
            GetLocalData();
        }
        private async void Next(CollectModel Model)
        {
            if (Model.Category == 1)
                await Shell.Current.GoToAsync(Extend.RouteMap[nameof(CatalogView)], new Dictionary<string, object> { { "Param", new ComicSearch { Cover = Model.Cover, Name = Model.Name, Route = Model.Route } } });
            if (Model.Category == 2)
                await Shell.Current.GoToAsync(Extend.RouteMap[nameof(DetailView)], new Dictionary<string, object> { { "Param", new RifanSearch { Cover = Model.Cover, Name = Model.Name, Route = Model.Route } } });
            if (Model.Category == 3)
                await Shell.Current.GoToAsync($"{Extend.RouteMap[nameof(ChapterView)]}?Type={Model.Commom}&Name={Model.Name}&Route={Model.Route}&Cover={Model.Cover}");
            if (Model.Category == 4)
                await Shell.Current.GoToAsync(Extend.RouteMap[nameof(CollectView)], new Dictionary<string, object> { { "Param", new CartInitElementResult { Title = Model.Name, Route = Model.Route, Cover = Model.Cover } } });
            if (Model.Category == 5)
                await Shell.Current.GoToAsync($"{Extend.RouteMap[nameof(ChaptersView)]}?Name={Model.Name}&Route={Model.Route}&Cover={Model.Cover}");
            if (Model.Category == 5)
                await Shell.Current.GoToAsync(Extend.RouteMap[nameof(AjaxView)], new Dictionary<string, object> { { "Param", Model.Route },{ "Title",Model.Name} });
        }
        #endregion

        #region Command
        public RelayCommand<string> CatalogCommand => new(input =>
        {
            Category = input.AsInt();
            Page = 1;
            GetLocalData();
        });
        public RelayCommand MoreCommand => new(() =>
        {
            Page += 1;
            if (Page <= Total)
                GetLocalData();
        });
        public RelayCommand<CollectModel> NextCommand => new(Next);
        public RelayCommand<dynamic> RemoveCommand => new(input => DeleteLocalData(input));
        #endregion
    }
}
