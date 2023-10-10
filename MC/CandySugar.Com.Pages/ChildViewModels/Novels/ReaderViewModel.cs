using CandySugar.Com.Library;
using CommunityToolkit.Mvvm.ComponentModel;
using Sdk.Component.Novel.sdk.ViewModel.Enums;
using Sdk.Component.Novel.sdk.ViewModel.Request;
using Sdk.Component.Novel.sdk;
using XExten.Advance.LinqFramework;
using Sdk.Component.Novel.sdk.ViewModel;
using Sdk.Component.Novel.sdk.ViewModel.Response;
using System.Collections.ObjectModel;

namespace CandySugar.Com.Pages.ChildViewModels.Novels
{
    public partial class ReaderViewModel : ObservableObject, IQueryAttributable
    {
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Platform = (PlatformEnum)query["Type"].ToString().AsInt();
            Route = query["Route"].ToString();
            Application.Current.Dispatcher.DispatchAsync(ReaderAsync);
        }

        #region Field
        private PlatformEnum Platform;
        private string Route;
        #endregion

        #region Property
        [ObservableProperty]
        private ObservableCollection<string> _Element;
        [ObservableProperty]
        private string _ChapterTitle;
        #endregion

        #region Method
        private async void ReaderAsync()
        {
            try
            {
                var result = (await NovelFactory.Novel(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        PlatformType = Platform,
                        CacheSpan = 5,
                        NovelType = NovelEnum.Content,
                        Content = new NovelContent
                        {
                            Route = Route
                        }
                    };
                }).RunsAsync()).ContentResult.ElementResult;
                ChapterTitle = result.ChapterName;
                Element = new ObservableCollection<string>(result.Content);
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }
        #endregion
    }
}
