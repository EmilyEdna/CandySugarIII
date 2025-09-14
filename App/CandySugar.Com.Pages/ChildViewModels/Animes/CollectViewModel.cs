using CandySugar.Com.Library;
using CandySugar.Com.Pages.ChildViews.Animes;
using CandySugar.Com.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using Sdk.Component.Cart.sdk;
using Sdk.Component.Cart.sdk.ViewModel;
using Sdk.Component.Cart.sdk.ViewModel.Enums;
using Sdk.Component.Cart.sdk.ViewModel.Request;
using Sdk.Component.Cart.sdk.ViewModel.Response;
using XExten.Advance.IocFramework;
using Application = Microsoft.Maui.Controls.Application;

namespace CandySugar.Com.Pages.ChildViewModels.Animes
{
    public partial class CollectViewModel : ObservableObject, IQueryAttributable
    {
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Result = (CartInitElementResult)query["Param"];
            Application.Current.Dispatcher.DispatchAsync(PreviewAsync);
        }

        #region Property
        [ObservableProperty]
        private CartInitElementResult _Result;
        [ObservableProperty]
        private CartDetailRootResult _DetailResult;
        #endregion

        #region Method
        private async void Insert(CartInitElementResult result)
        {
            await IocDependency.Resolve<ICandyService>().Add(new CollectModel
            {
                Category = 1,
                Cover = result.Cover,
                Name = result.Title,
                Route = result.Route,
            });
        }

        private async void PreviewAsync()
        {
            try
            {
                DetailResult = (await CartFactory.Car(opt =>
                {
                    opt.RequestParam = new Input
                    {

                        CartType = CartEnum.Detail,
                        CacheSpan = 5,
                        Detail = new CartDetail
                        {
                            Route = Result.Route,
                        }
                    };
                }).RunsAsync()).DetailResult;
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }

        public async  void PlayAsync(string input)
        {
            try
            {
                var Route = (await CartFactory.Car(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        CartType = CartEnum.Play,
                        CacheSpan = 5,
                        Play = new CartPlay
                        {
                            Route = input,
                        }
                    };
                }).RunsAsync()).PlayResult.PlayRoute;
                Next(Route.Trim().Replace("https","http"));
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }

        private async void Next(string input)
        {
            await Shell.Current.GoToAsync(Extend.RouteMap[nameof(PlayView)], new Dictionary<string, object> { { "Route", input } });
        }
        #endregion

        #region Command
        public RelayCommand<string> PlayCommand => new(PlayAsync);
        public RelayCommand LoveCommand => new(() => Insert(Result));
        #endregion
    }
}
