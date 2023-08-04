using BlazorComponent;
using Masa.Blazor;
using Masa.Blazor.Presets;
using Microsoft.AspNetCore.Components;
using Sdk.Component.Cart.sdk;
using Sdk.Component.Cart.sdk.ViewModel;
using Sdk.Component.Cart.sdk.ViewModel.Enums;
using Sdk.Component.Cart.sdk.ViewModel.Request;
using Sdk.Component.Cart.sdk.ViewModel.Response;
using XExten.Advance.LinqFramework;
using XExten.Advance.NetFramework.Enums;

namespace CandySugar.MainUI.Views.ViewMdeols
{
    public class AnimeVM : ComponentBase
    {
        [Inject]
        public IPopupService PopupService { get; set; }
        protected MContainer Row { get; set; }
        protected string MCVisible;
        protected string PCVisible;
        [Parameter]
        public string Loading { get; set; }
        public AnimeVM()
        {
            if (Module.Platforms == Platform.Windows)
            {
                MCVisible = "visibility:hidden;overflow:hidden;display:none;";
                PCVisible = "visibility:visible;overflow:hidden";
            }
            else
            {
                PCVisible = "visibility:hidden;overflow:hidden;display:none;";
                MCVisible = "visibility:visible;overflow:hidden";
            }
        }

        protected override async Task OnInitializedAsync()
        {
            OnInit();
            await Task.Delay(3000);
            Loading = "display:none;visibility:hidden;";
        }

        #region Field
        private int Total;
        private int PageIndex = 1;
        private string Route;
        #endregion

        #region Property
        public List<CartResult> InitResult { get; set; } = new List<CartResult>();
        public List<List<CartResult>> InitResults { get; set; } = new List<List<CartResult>>();
        public List<CartDetailElementResult> DetailResult { get; set; } = new List<CartDetailElementResult>();
        #endregion

        #region Method
        private async void OnInit()
        {
            try
            {
              
                var result = (await CartFactory.Car(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        CacheSpan = 5,
                        CartType = CartEnum.Init,
                        Init = new CartInit()
                    };
                }).RunsAsync()).InitResult;
                Total = result.Total;
                InitResult = result.ElementResults.ToMapest<List<CartResult>>();
                for (int index = 0; index < InitResult.Count; index++)
                {
                    if (InitResults.ElementAtOrDefault(index / 8) == null)
                    {
                        InitResults.Add(new List<CartResult>());
                    }
                    InitResults[index / 8].Add(InitResult[index]);
                } 
            }
            catch (Exception Ex)
            {
                await PopupService.EnqueueSnackbarAsync(new SnackbarOptions(Ex.Message, AlertTypes.Error));
            }
        }
        private async void OnDetail()
        {
            try
            {
                DetailResult = (await CartFactory.Car(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        CacheSpan = 5,
                        CartType = CartEnum.Detail,
                        Detail = new CartDetail
                        {
                            Route = Route
                        }
                    };
                }).RunsAsync()).DetailResult.ElementResults;
            }
            catch (Exception Ex)
            {
                await PopupService.EnqueueSnackbarAsync(new SnackbarOptions(Ex.Message, AlertTypes.Error));
            }
        }
        private async void OnMoreInit()
        {
            try
            {
                var result = (await CartFactory.Car(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        CacheSpan = 5,
                        CartType = CartEnum.Init,
                        Init = new CartInit
                        {
                            Page = PageIndex
                        }
                    };
                }).RunsAsync()).InitResult;
                result.ElementResults.ToMapest<List<CartResult>>().ForEach(InitResult.Add);
                for (int index = 72 * (PageIndex-1); index < InitResult.Count; index++)
                {
                    if (InitResults.ElementAtOrDefault(index / 8) == null)
                    {
                        InitResults.Add(new List<CartResult>());
                    }
                    InitResults[index / 8].Add(InitResult[index]);
                }
            }
            catch (Exception Ex)
            {
                await PopupService.EnqueueSnackbarAsync(new SnackbarOptions(Ex.Message, AlertTypes.Error));
            }
        }
        #endregion

        #region Event
        public void DetailEvent(CartResult cart)
        {
            cart.IsExtend = !cart.IsExtend;
            Route = cart.Route;
            Task.Run(OnDetail);
        }
        public async void OnLoad(InfiniteScrollLoadEventArgs args)
        {
            if (PageIndex <= Total)
            {
                PageIndex += 1;
                OnMoreInit();
                await Task.Delay(3000);
                args.Status = InfiniteScrollLoadStatus.Ok;
            }
            else
                PageIndex = 1;
        }
        #endregion
    }

    public class CartResult : CartInitElementResult
    {
        public bool IsExtend { get; set; } = false;
    }
}
