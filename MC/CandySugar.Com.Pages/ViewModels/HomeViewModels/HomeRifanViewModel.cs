using CandySugar.Com.Library.BaseViewModel;
using CandySugar.Com.Pages.Views.RifanViews;
using CandySugar.Com.Service;
using System.Collections.ObjectModel;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages.ViewModels.HomeViewModels
{
    public class HomeRifanViewModel : BaseVMModule
    {
        public HomeRifanViewModel(BaseVMService baseServices) : base(baseServices)
        {
        }

        public override void OnLoad()
        {
            InitRifan();
        }

        #region Flied
        private int RifanIndex;

        private int RifanTotal;
        #endregion

        #region Property
        
        private ObservableCollection<CollectModel> _RifanCollect;
        public ObservableCollection<CollectModel> RifanCollect
        {
            get => _RifanCollect;
            set => SetProperty(ref _RifanCollect, value);
        }
        #endregion

        #region Command
        public DelegateCommand LoadCommand => new(LoadMoreMethod);
        public DelegateCommand<CollectModel> WatchCommand => new(WatchMethod);
        public DelegateCommand ClearCommand => new(ClearMethod);
        public DelegateCommand BackCommand => new(() => Nav.GoBackAsync());
        public DelegateCommand<CollectModel> DeleteCommand => new(async element =>
        {
            await Container.Resolve<ICandyService>().Delete(element.Id);
            InitRifan();
        });
        #endregion

        #region Method

        private async void InitRifan()
        {
            var result = await Container.Resolve<ICandyService>().Get(2, 1);
            RifanTotal = result.Item1;
            RifanCollect = new ObservableCollection<CollectModel>(result.Item2);
        }

        private async void LoadMoreMethod()
        {
            RifanIndex += 1;
            if (RifanIndex > RifanTotal) return;
            var result = await Container.Resolve<ICandyService>().Get(2, RifanIndex);
            result.Item2.ForEach(RifanCollect.Add);

        }

     
        private void WatchMethod(CollectModel input)
        {
            Nav.NavigateAsync(new Uri(nameof(RifanInfo), UriKind.Relative), new NavigationParameters { { "Param", input.ToMapest<Sdk.Component.Vip.Anime.sdk.ViewModel.Response.SearchElementResult>() } });
        }
        private async void ClearMethod()
        {
            await Container.Resolve<ICandyService>().Remove(2);
        }
        #endregion
    }
}
