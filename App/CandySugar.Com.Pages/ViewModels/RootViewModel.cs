using System.Collections.ObjectModel;
using CandySugar.Com.Library;
using CandySugar.Com.Library.Model;
using CandySugar.Com.Pages.ChildViews.Animes;
using CandySugar.Com.Pages.ChildViews.Rifans;
using CandySugar.Com.Pages.Views;
using CandySugar.Com.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mopups.Events;
using Mopups.Interfaces;
using Mopups.Services;
using Sdk.Component.Cart.sdk.ViewModel.Response;
using Sdk.Component.Vip.Jron.sdk;
using Sdk.Component.Vip.Jron.sdk.ViewModel;
using Sdk.Component.Vip.Jron.sdk.ViewModel.Enums;
using Sdk.Component.Vip.Jron.sdk.ViewModel.Request;
using XExten.Advance.IocFramework;
using XExten.Advance.LinqFramework;
using RifanSearch = Sdk.Component.Vip.Anime.sdk.ViewModel.Response.SearchElementResult;
using VideoView = CandySugar.Com.Pages.ChildViews.Axgles.VideoView;

namespace CandySugar.Com.Pages.ViewModels
{
    public partial class RootViewModel : ObservableObject
    {

        public RootViewModel()
        {
            Bar = new ObservableCollection<BarModel>
            {
               new BarModel{ Name="里番", Route="1" },
               new BarModel{ Name="动漫", Route="2" },
               new BarModel{ Name="车牌", Route="3" },
            };
            GetLocalData();
            Popup = MopupService.Instance;
            Popup.Popped -= PopEvent;
            Popup.Popped += PopEvent;
        }

        #region Field
        private int Category = 4;
        private int Page = 1;
        private int Total;
        private IPopupNavigation Popup;
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
            GetLocalData();
            if (Model.Category == 1)
                await Shell.Current.GoToAsync(Extend.RouteMap[nameof(DetailView)], new Dictionary<string, object> { { "Param", new RifanSearch { Cover = Model.Cover, Name = Model.Name, Route = Model.Route } } });
            if (Model.Category == 2)
                await Shell.Current.GoToAsync(Extend.RouteMap[nameof(CollectView)], new Dictionary<string, object> { { "Param", new CartInitElementResult { Title = Model.Name, Route = Model.Route, Cover = Model.Cover } } });
            if (Model.Category == 3)
            {
                if (Model.Route.Contains("surrit"))
                {
                    await Shell.Current.GoToAsync(Extend.RouteMap[nameof(VideoView)], new Dictionary<string, object> { { "Param", Model.Route } });
                    return;
                }
                var Type = Model.Route.Contains("javbangers") ? PlatformEnum.Jav : PlatformEnum.Skb;
                var result = (await JronFactory.Jron(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        JronType = JronEnum.Detail,
                        PlatformType = Type,
                        CacheSpan = 5,
                        Play = new JronPlay
                        {
                            Route = Model.Route
                        }
                    };
                }).RunsAsync()).PlayResult;
                await Shell.Current.GoToAsync(Extend.RouteMap[nameof(VideoView)], new Dictionary<string, object> { { "Param", result.Play } });
            }
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
        public RelayCommand<CollectModel> NextCommand => new RelayCommand<CollectModel>(Next);
        public RelayCommand<dynamic> RemoveCommand => new(input => DeleteLocalData(input));
        public RelayCommand<CollectModel> AlterCommand => new(async input =>
        {
            await Popup.PushAsync(new AlterView
            {
                BindingContext = new AlterViewModel
                {
                    CollectModel = input
                }
            });
        });
        #endregion

        private void PopEvent(object sender, PopupNavigationEventArgs e)
        {
            Page = 1;
            GetLocalData();
        }
    }
}
