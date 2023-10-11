using System.Collections.ObjectModel;
using System.Web;
using CandySugar.Com.Library;
using CandySugar.Com.Pages.ChildViews.Novels;
using CandySugar.Com.Service;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sdk.Component.Novel.sdk;
using Sdk.Component.Novel.sdk.ViewModel;
using Sdk.Component.Novel.sdk.ViewModel.Enums;
using Sdk.Component.Novel.sdk.ViewModel.Request;
using Sdk.Component.Novel.sdk.ViewModel.Response;
using XExten.Advance.IocFramework;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages.ChildViewModels.Novels
{
    public partial class ChapterViewModel : ObservableObject, IQueryAttributable
    {
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Page = 1;
            Platform = (PlatformEnum)query["Type"].ToString().AsInt();
            BookName = HttpUtility.UrlDecode(query["Name"].ToString());
            Route = query["Route"].ToString();
            Cover = query["Cover"].ToString();
            Application.Current.Dispatcher.DispatchAsync(ChapterAsync);
        }
        #region Field
        private string Cover;
        private PlatformEnum Platform;
        private string Code = string.Empty;
        private string Route;
        private int Page;
        private int Total;
        #endregion

        #region Property
        [ObservableProperty]
        private string _BookName;
        [ObservableProperty]
        private ObservableCollection<NovelDetailElementResult> _ElementResult;
        #endregion

        #region Method
        private async void Insert(string Name, string Route, string Cover, string Common)
        {
            await IocDependency.Resolve<ICandyService>().Add(new CollectModel
            {
                Category = 3,
                Cover = Cover,
                Name = Name,
                Route = Route,
                Commom = Common
            });
        }

        private async void ChapterAsync()
        {
            try
            {
                var Result = (await NovelFactory.Novel(opt =>
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
                ElementResult = new ObservableCollection<NovelDetailElementResult>(Result.ElementResults);
                Code = Result.BookCode;
                Total = Result.Total;
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }
        private async void MoreChapterAsync()
        {
            try
            {
                var Result = (await NovelFactory.Novel(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        PlatformType = Platform,
                        CacheSpan = 5,
                        NovelType = NovelEnum.Chapter,
                        Chapter = new NovelChapter
                        {
                            BookCode = Code,
                            Page = Page
                        }
                    };
                }).RunsAsync()).DetailResult;
                Result.ElementResults.ForEach(ElementResult.Add);
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

        public RelayCommand MoreCommand => new(() =>
        {
            if (!Code.IsNullOrEmpty() && Platform == PlatformEnum.Pendown)
            {
                Page += 1;
                if (Page <= Total)
                    Application.Current.Dispatcher.DispatchAsync(MoreChapterAsync);
            }
        });

        public RelayCommand LoveCommand => new(()=>Insert(BookName, Route, Cover, ((int)Platform).ToString()));
        #endregion
    }
}
