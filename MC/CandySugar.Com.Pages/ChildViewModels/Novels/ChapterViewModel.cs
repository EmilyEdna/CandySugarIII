using CommunityToolkit.Mvvm.ComponentModel;
using Sdk.Component.Novel.sdk;
using Sdk.Component.Novel.sdk.ViewModel;
using CandySugar.Com.Library;
using Sdk.Component.Novel.sdk.ViewModel.Enums;
using Sdk.Component.Novel.sdk.ViewModel.Request;
using Sdk.Component.Novel.sdk.ViewModel.Response;
using XExten.Advance.LinqFramework;
using System.Web;
using CommunityToolkit.Mvvm.Input;
using CandySugar.Com.Pages.ChildViews.Novels;
using XExten.Advance.IocFramework;
using CandySugar.Com.Service;

namespace CandySugar.Com.Pages.ChildViewModels.Novels
{
    public partial class ChapterViewModel : ObservableObject, IQueryAttributable
    {
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Platform = (PlatformEnum)query["Type"].ToString().AsInt();
            BookName = HttpUtility.UrlDecode(query["Name"].ToString());
            Route = query["Route"].ToString();
            Insert(BookName, Route, query["Cover"].ToString(), ((int)Platform).ToString());
            Application.Current.Dispatcher.DispatchAsync(ChapterAsync);
        }
        #region Field
        private PlatformEnum Platform;
        private string Route;
        #endregion

        #region Property
        [ObservableProperty]
        private string _BookName;
        [ObservableProperty]
        private NovelDetailRootResult _DetailResult;
        #endregion

        #region Method
        private async void Insert(string Name,string Route,string Cover,string Common)
        {
            await IocDependency.Resolve<ICandyService>().Add(new CollectModel
            {
                Category = 3,
                Cover = Cover,
                Name = Name,
                Route = Route,
                Commom= Common
            });
        }

        private async void ChapterAsync()
        {
            try
            {
                DetailResult = (await NovelFactory.Novel(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        PlatformType = Platform,
                        CacheSpan = 5,
                        NovelType = NovelEnum.Detail,
                        Detail = new NovelDetail
                        {
                            BookName = BookName,
                            Route = Route
                        }
                    };
                }).RunsAsync()).DetailResult;
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }
        #endregion

        #region Command
        public RelayCommand<string> ReaderCommand => new(async input =>
        {
            await Shell.Current.GoToAsync($"{Extend.RouteMap[nameof(ReaderView)]}?Type={(int)Platform}&Route={input}");
        });
        #endregion
    }
}
