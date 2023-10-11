using System.Collections.ObjectModel;
using CandySugar.Com.Library;
using CandySugar.Com.Pages.ChildViews.Animes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sdk.Component.Cart.sdk;
using Sdk.Component.Cart.sdk.ViewModel;
using Sdk.Component.Cart.sdk.ViewModel.Enums;
using Sdk.Component.Cart.sdk.ViewModel.Request;
using Sdk.Component.Cart.sdk.ViewModel.Response;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages.ViewModels
{
    public partial class AnimeViewModel : ObservableObject
    {
        public AnimeViewModel()
        {
            Application.Current.Dispatcher.DispatchAsync(InitAsync);
        }
        #region Field
        private int Page = 1;
        private int Total;
        private int QPage = 1;
        private int QTotal;
        #endregion

        #region Property
        [ObservableProperty]
        private string _QueryKey;
        [ObservableProperty]
        private ObservableCollection<CartInitElementResult> _InitResult;
        #endregion

        #region Method
        private async void InitAsync()
        {
            try
            {
                var result = (await CartFactory.Car(opt =>
                {
                    opt.RequestParam = new Input
                    {

                        CartType = CartEnum.Init,
                        CacheSpan = 5,
                        Init = new CartInit
                        {
                            Page = Page,
                        }
                    };
                }).RunsAsync()).InitResult;
                if (Page == 1)
                {
                    Total = result.Total;
                    InitResult = new ObservableCollection<CartInitElementResult>(result.ElementResults);
                }
                else result.ElementResults.ForEach(InitResult.Add);
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
                var result = (await CartFactory.Car(opt =>
                {
                    opt.RequestParam = new Input
                    {

                        CartType = CartEnum.Search,
                        CacheSpan = 5,
                        Search = new CartSearch
                        {
                            Page = QPage,
                            Keyword = QueryKey
                        }
                    };
                }).RunsAsync()).SearchResult;
                var TargetModel = result.ElementResults.ToMapest<List<CartInitElementResult>>();
                if (QPage == 1)
                {
                    QTotal = result.Total;
                    InitResult = new ObservableCollection<CartInitElementResult>(TargetModel);
                }
                else TargetModel.ForEach(InitResult.Add);
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }

        private async void Next(CartInitElementResult Model)
        {
            await Shell.Current.GoToAsync(Extend.RouteMap[nameof(CollectView)], new Dictionary<string, object> { { "Param", Model } });
        }
        #endregion

        #region  Command
        public RelayCommand<CartInitElementResult> CollectCommand => new(Next);
        public RelayCommand QueryCommand => new(() =>
        {
            if (QueryKey.IsNullOrEmpty()) return;
            QPage = 1;
            Application.Current.Dispatcher.DispatchAsync(SearchAsync);
        });
        public RelayCommand MoreCommand => new(() =>
        {
            if (QueryKey.IsNullOrEmpty())
            {
                Page += 1;
                if (Page <= Total)
                    Application.Current.Dispatcher.DispatchAsync(InitAsync);
            }
            else
            {
                QPage += 1;
                if (QPage <= QTotal)
                    Application.Current.Dispatcher.DispatchAsync(SearchAsync);
            }
        });
        #endregion
    }
}
