using CandySugar.Com.Library.BaseViewModel;
using CandySugar.Com.Library.OptionModel;
using Sdk.Component.Vip.Anime.sdk;
using CandySugar.Com.Library.Extends;
using Sdk.Component.Vip.Anime.sdk.ViewModel;
using Sdk.Component.Vip.Anime.sdk.ViewModel.Enums;
using Sdk.Component.Vip.Anime.sdk.ViewModel.Request;
using Sdk.Component.Vip.Anime.sdk.ViewModel.Response;
using System.Collections.ObjectModel;
using XExten.Advance.LinqFramework;
using CommunityToolkit.Mvvm.Input;
using CandySugar.Com.Pages.Views.RifanViews;

namespace CandySugar.Com.Pages.ViewModels.RifanViewModels
{
    public class RifanInfoViewModel : BaseVMModule
    {
        public RifanInfoViewModel(BaseVMService baseServices) : base(baseServices)
        {
        }

        public override void Initialize(INavigationParameters parameters)
        {
            OnWatchInit(parameters.GetValue<SearchElementResult>("Param"));
        }

        #region Property
        private ObservableCollection<PlayInfo> _Info;
        public ObservableCollection<PlayInfo> Info
        {
            get => _Info;
            set => SetProperty(ref _Info, value);
        }

        private ObservableCollection<WatchElementResult> _Link;
        public ObservableCollection<WatchElementResult> Link
        {
            get => _Link;
            set => SetProperty(ref this._Link, value);
        }
        #endregion

        #region Command
        public RelayCommand<string> PlayCommand => new(element => Nav.NavigateAsync(new Uri(nameof(RifanPlay), UriKind.Relative), new NavigationParameters() { { "Param", element } }));

        public RelayCommand<WatchElementResult> WatchCommand => new(element => OnWatchInit(element.ToMapest<SearchElementResult>()));

        public RelayCommand BackCommand => new(() => Nav.GoBackAsync());
        #endregion

        #region ExternalMethod
        private async void OnWatchInit(SearchElementResult element)
        {
            try
            {
                var result = (await AnimeFactory.Anime(opt =>
                {
                    opt.RequestParam = new Input
                    {

                        AnimeType = AnimeEnum.Watch,
                        Watch = new AnimeWatch
                        {
                            Route = element.Route
                        }
                    };
                }).RunsAsync()).WatchResult;

                Info = new ObservableCollection<PlayInfo>(result.Current.Select(t => new PlayInfo
                {
                    Clarity = $"{t.Key}P",
                    Route = t.Value,
                    Name = element.Name
                }));

                Link = new ObservableCollection<WatchElementResult>(result.Results);
            }
            catch (Exception ex)
            {
                ex.Message.Info();
            }
        }
        #endregion

    }
}
