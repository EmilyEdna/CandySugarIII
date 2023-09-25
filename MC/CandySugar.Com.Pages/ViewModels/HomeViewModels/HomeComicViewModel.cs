using CandySugar.Com.Library.BaseViewModel;
using CandySugar.Com.Pages.Views.ComicViews;
using CandySugar.Com.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages.ViewModels.HomeViewModels
{
    public class HomeComicViewModel : BaseVMModule
    {
        public HomeComicViewModel(BaseVMService baseServices) : base(baseServices)
        {
        }

        public override void OnLoad()
        {
            InitComic();
        }

        #region Flied
        private int ComicIndex;

        private int ComicTotal;
        #endregion

        #region Property
        private ObservableCollection<CollectModel> _ComicCollect;
        public ObservableCollection<CollectModel> ComicCollect
        {
            get => _ComicCollect;
            set => SetProperty(ref _ComicCollect, value);
        }
        private ObservableCollection<CollectModel> _RifanCollect;
        public ObservableCollection<CollectModel> RifanCollect
        {
            get => _RifanCollect;
            set => SetProperty(ref _RifanCollect, value);
        }
        private ObservableCollection<CollectModel> _AxgleCollect;
        public ObservableCollection<CollectModel> AxgleCollect
        {
            get => _AxgleCollect;
            set => SetProperty(ref _AxgleCollect, value);
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
            InitComic();
        });
        #endregion

        #region Method
        private async void InitComic()
        {
            var result = await Container.Resolve<ICandyService>().Get(1, 1);
            ComicTotal = result.Item1;
            ComicCollect = new ObservableCollection<CollectModel>(result.Item2);
        }

        private async void LoadMoreMethod()
        {
            ComicIndex += 1;
            if (ComicIndex > ComicTotal) return;
            var result = await Container.Resolve<ICandyService>().Get(1, ComicIndex);
            result.Item2.ForEach(item =>
            {
                if (!ComicCollect.Any(t => t.Hash == item.Hash))
                    ComicCollect.Add(item);
            });
        }


        private void WatchMethod(CollectModel input)
        {
            Nav.NavigateAsync(new Uri(nameof(ComicInfo), UriKind.Relative), new NavigationParameters { { "Param", input.ToMapest<Sdk.Component.Vip.Comic.sdk.ViewModel.Response.SearchElementResult>() } });
        }
        private async void ClearMethod()
        {
            await Container.Resolve<ICandyService>().Remove(1);
        }
        #endregion
    }
}
