using System.Text;
using BlazorComponent;
using Masa.Blazor;
using Masa.Blazor.Presets;
using Microsoft.AspNetCore.Components;
using Sdk.Component.Vip.Comic.sdk.ViewModel;
using Sdk.Component.Vip.Comic.sdk;
using XP = XExten.Advance.NetFramework.Enums.Platform;
using Sdk.Component.Vip.Comic.sdk.ViewModel.Enums;
using Sdk.Component.Vip.Comic.sdk.ViewModel.Request;
using Sdk.Component.Vip.Comic.sdk.ViewModel.Response;
using XExten.Advance.LinqFramework;

namespace CandySugar.MainUI.Views.ViewMdeols
{
    public class ComicVM : ComponentBase
    {
        [Inject]
        public IPopupService PopupService { get; set; }
        [Inject]
        public NavigationManager Navigation { get; set; }
        protected MContainer Row { get; set; }
        protected string MCVisible;
        protected string PCVisible;
        [Parameter]
        public string Loading { get; set; }
        public string SearchText { get; set; }
        public ComicVM()
        {
            if (Module.Platforms == XP.Windows)
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

        protected override void OnInitialized()
        {
            Loading = "display:none;visibility:hidden;";
        }

        #region Field
        private int Total;
        private int PageIndex = 1;
        private string Route;
        #endregion

        #region Property
        public List<SearchElementResult> InitResult { get; set; } = new List<SearchElementResult>();
        public List<List<SearchElementResult>> InitResults { get; set; } = new List<List<SearchElementResult>>();
        public List<string> Views { get; set; } = new List<string>();
        #endregion

        #region Method
        private async void OnInit()
        {
            try
            {
                var result = (await ComicFactory.Comic(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        CacheSpan = 5,
                        ComicType = ComicEnum.Search,
                        Search = new ComicSearch
                        {
                            Keyword = SearchText,
                            Page = 1
                        }
                    };
                }).RunsAsync()).SearchResult;
                Total = result.Total;
                InitResult = result.Results;
                for (int index = 0; index < InitResult.Count; index++)
                {
                    if (InitResults.ElementAtOrDefault(index / 7) == null)
                    {
                        InitResults.Add(new List<SearchElementResult>());
                    }
                    InitResults[index / 7].Add(InitResult[index]);
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
                Views = (await ComicFactory.Comic(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        CacheSpan = 5,
                        ComicType = ComicEnum.View,
                        Preview = new ComicPreview
                        {
                            Route = Route
                        }
                    };
                }).RunsAsync()).ViewResult.Views;
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
                var result = (await ComicFactory.Comic(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        CacheSpan = 5,
                        ComicType = ComicEnum.Search,
                        Search = new ComicSearch
                        {
                            Keyword = SearchText,
                            Page = PageIndex
                        }
                    };
                }).RunsAsync()).SearchResult;

                result.Results.ForEach(InitResult.Add);

                for (int index = 35 * (PageIndex - 1); index < InitResult.Count; index++)
                {
                    if (InitResults.ElementAtOrDefault(index / 7) == null)
                    {
                        InitResults.Add(new List<SearchElementResult>());
                    }
                    InitResults[index / 7].Add(InitResult[index]);
                }
            }
            catch (Exception Ex)
            {
                await PopupService.EnqueueSnackbarAsync(new SnackbarOptions(Ex.Message, AlertTypes.Error));
            }
        }
        #endregion

        #region Event
        public void PlayEvent(string route)
        {
            var b64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(route));
            Module.HistoryUri = Navigation.Uri;
            Navigation.NavigateTo($"/Watch/{b64}");
        }
        public void DetailEvent(SearchElementResult cart)
        {
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

        public void SearchEvent()
        {
            OnInit();
        }
        #endregion
    }
}
