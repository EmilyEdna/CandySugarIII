using CandySugar.Com.Library.BaseViewModel;
using CandySugar.Com.Library.Extends;
using CandySugar.Com.Pages.Views.ComicViews;
using Sdk.Component.Vip.Comic.sdk;
using Sdk.Component.Vip.Comic.sdk.ViewModel;
using Sdk.Component.Vip.Comic.sdk.ViewModel.Enums;
using Sdk.Component.Vip.Comic.sdk.ViewModel.Request;
using Sdk.Component.Vip.Comic.sdk.ViewModel.Response;
using System.Collections.ObjectModel;

namespace CandySugar.Com.Pages.ViewModels.ComicViewModels
{
    public class ComicInfoViewModel : BaseVMModule
    {
        public ComicInfoViewModel(BaseVMService baseServices) : base(baseServices)
        {
        }

        public override void Initialize(INavigationParameters parameters)
        {
            Result = parameters.GetValue<SearchElementResult>("Param");
            OnViewInit();
        }

        #region Property
        private SearchElementResult _Result;
        public SearchElementResult Result
        {
            get => _Result;
            set => SetProperty(ref _Result, value);
        }
        private ObservableCollection<string> _Preview;
        public ObservableCollection<string> Preview
        {
            get => _Preview;
            set => SetProperty(ref _Preview, value);
        }

        private ObservableCollection<string> _View;
        public ObservableCollection<string> View
        {
            get => _View;
            set => SetProperty(ref _View, value);
        }
        #endregion

        #region Command
        public DelegateCommand BackCommand => new(() => Nav.GoBackAsync());
        public DelegateCommand<string> WatchCommand => new(element =>
        {
            var Index = Preview.ToList().FindIndex(t => t.Equals(element));
            Nav.NavigateAsync(new Uri(nameof(ComicWatch), UriKind.Relative), new NavigationParameters { { "Param", View }, { "Index", Index } });
        });
        #endregion

        #region ExternalMethod
        private async void OnViewInit()
        {
            try
            {
                var result = (await ComicFactory.Comic(opt =>
                {
                    opt.RequestParam = new Input
                    {

                        ComicType = ComicEnum.View,
                        Preview = new ComicPreview
                        {
                            Route = Result.Route
                        }
                    };
                }).RunsAsync()).ViewResult;
                View = new ObservableCollection<string>(result.Views);
                Preview = new ObservableCollection<string>(result.Previews);
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }
        #endregion
    }
}
