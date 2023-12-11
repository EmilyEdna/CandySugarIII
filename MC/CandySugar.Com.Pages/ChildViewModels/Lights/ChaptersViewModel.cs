using CandySugar.Com.Library;
using CandySugar.Com.Pages.ChildViews.Lights;
using CandySugar.Com.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sdk.Component.Lovel.sdk;
using Sdk.Component.Lovel.sdk.ViewModel;
using Sdk.Component.Lovel.sdk.ViewModel.Enums;
using Sdk.Component.Lovel.sdk.ViewModel.Request;
using Sdk.Component.Lovel.sdk.ViewModel.Response;
using System.Collections.ObjectModel;
using System.Web;
using XExten.Advance.IocFramework;

namespace CandySugar.Com.Pages.ChildViewModels.Lights
{
    public partial class ChaptersViewModel : ObservableObject, IQueryAttributable
    {
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            BookName = HttpUtility.UrlDecode(query["Name"].ToString());
            Route = query["Route"].ToString();
            Cover = query["Cover"].ToString();
            Application.Current.Dispatcher.DispatchAsync(ChapterAsync);
        }

        #region Field
        private string Cover;
        private string Route;
        #endregion

        #region Property
        [ObservableProperty]
        private string _BookName;
        [ObservableProperty]
        private ObservableCollection<LovelViewResult> _ViewResult;
        #endregion

        #region Method
        /// <summary>
        /// 初始化章节
        /// </summary>
        /// <param name="ChapterRoute"></param>
        private async void ChapterAsync()
        {
            try
            {
                var ChapterResult = (await LovelFactory.Lovel(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        CacheSpan = 5,
                        LovelType = LovelEnum.Detail,
                        Detail = new LovelDetail
                        {
                            Route = Route
                        }
                    };
                }).RunsAsync()).DetailResult;
                await Task.Delay(1000);
                var result = (await LovelFactory.Lovel(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        CacheSpan = 5,
                        LovelType = LovelEnum.View,
                        View = new LovelView
                        {
                            Route = ChapterResult.Route
                        }
                    };
                }).RunsAsync()).ViewResult;
                ViewResult = new ObservableCollection<LovelViewResult>(result);
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }

        private async void Insert(string Name, string Route, string Cover)
        {
            await IocDependency.Resolve<ICandyService>().Add(new CollectModel
            {
                Category = 5,
                Cover = Cover,
                Name = Name,
                Route = Route
            });
        }
        #endregion

        #region Command
        public RelayCommand<string> ReaderCommand => new(async input =>
        {
            await Shell.Current.GoToAsync($"{Extend.RouteMap[nameof(ReadersView)]}?Route={input}");
        });
        public RelayCommand LoveCommand => new(() => Insert(BookName, Route, Cover));
        #endregion
    }
}
