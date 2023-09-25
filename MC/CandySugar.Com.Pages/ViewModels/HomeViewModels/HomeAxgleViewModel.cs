using CandySugar.Com.Library.BaseViewModel;
using CandySugar.Com.Pages.Views.AxgleViews;
using CandySugar.Com.Service;
using System.Collections.ObjectModel;

namespace CandySugar.Com.Pages.ViewModels.HomeViewModels
{
    public class HomeAxgleViewModel : BaseVMModule
    {
        public HomeAxgleViewModel(BaseVMService baseServices) : base(baseServices)
        {
        }

        public override void OnLoad()
        {
            InitAxgle();
        }

        #region Property

        private ObservableCollection<CollectModel> _AxgleCollect;
        public ObservableCollection<CollectModel> AxgleCollect
        {
            get => _AxgleCollect;
            set => SetProperty(ref _AxgleCollect, value);
        }
        #endregion

        #region Flied
        private int AxgleIndex;

        private int AxgleTotal;
        #endregion

        #region Command
        public DelegateCommand LoadCommand => new(LoadMoreMethod);
        public DelegateCommand<CollectModel> WatchCommand => new(WatchMethod);
        public DelegateCommand ClearCommand => new(ClearMethod);
        public DelegateCommand BackCommand => new(()=>Nav.GoBackAsync());
        public DelegateCommand<CollectModel> DeleteCommand => new(async element =>
        {
            await Container.Resolve<ICandyService>().Delete(element.Id);
            InitAxgle();
        });
        #endregion

        #region Method
        private async void InitAxgle()
        {
            var result = await Container.Resolve<ICandyService>().Get(3, 1);
            AxgleTotal = result.Item1;
            AxgleCollect = new ObservableCollection<CollectModel>(result.Item2);
        }

        private async void LoadMoreMethod()
        {
            AxgleIndex += 1;
            if (AxgleIndex > AxgleTotal) return;
            var result = await Container.Resolve<ICandyService>().Get(3, AxgleIndex);
            result.Item2.ForEach(item =>
            {
                if (!AxgleCollect.Any(t => t.Hash == item.Hash))
                    AxgleCollect.Add(item);
            });

        }

        private void WatchMethod(CollectModel input)
        {
            Nav.NavigateAsync(new Uri(nameof(AxglePlay), UriKind.Relative), new NavigationParameters { { "Param", input.Route } });
        }
        private async void ClearMethod()
        {
            await Container.Resolve<ICandyService>().Remove(3);
        }
        #endregion
    }
}
