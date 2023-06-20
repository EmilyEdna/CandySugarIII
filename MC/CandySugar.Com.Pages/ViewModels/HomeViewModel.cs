using CandySugar.Com.Library.BaseViewModel;
using CandySugar.Com.Pages.Views.AxgleViews;
using CandySugar.Com.Pages.Views.ComicViews;
using CandySugar.Com.Pages.Views.RifanViews;
using CandySugar.Com.Service.IServiceImpl;
using CandySugar.Com.Service.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.LinqFramework;
using static SQLite.SQLite3;

namespace CandySugar.Com.Pages.ViewModels
{
    public class HomeViewModel : BaseVMModule
    {
        public HomeViewModel(BaseVMService baseServices) : base(baseServices)
        {
            ComicIndex = RifanIndex = AxgleIndex;
        }

        public override void OnLoad()
        {
            InitComic();
        }

        #region Flied
        private int ComicIndex;
        private int RifanIndex;
        private int AxgleIndex;

        private int ComicTotal;
        private int RifanTotal;
        private int AxgleTotal;
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
        public DelegateCommand<string> LoadCommand => new(LoadMoreMethod);
        public DelegateCommand<dynamic> ChangeCommand => new(ChangeMethod);
        public DelegateCommand<CollectModel> WatchCommand => new(WatchMethod);
        public DelegateCommand<string> ClearCommand => new(ClearMethod);
        #endregion

        #region Method
        private async void InitComic()
        {
            var result = await Container.Resolve<ICandyService>().Get(1, 1);
            ComicTotal = result.Item1;
            ComicCollect = new ObservableCollection<CollectModel>(result.Item2);
        }
        private async void InitRifan()
        {
            var result = await Container.Resolve<ICandyService>().Get(2, 1);
            AxgleTotal = result.Item1;
            RifanCollect = new ObservableCollection<CollectModel>(result.Item2);
        }
        private async void InitAxgle()
        {
            var result = await Container.Resolve<ICandyService>().Get(3, 1);
            RifanTotal = result.Item1;
            AxgleCollect = new ObservableCollection<CollectModel>(result.Item2);
        }

        private async void LoadMoreMethod(string param)
        {
            if (param.Equals("1"))
            {
                ComicIndex += 1;
                if (ComicIndex > ComicTotal) return;
                var result = await Container.Resolve<ICandyService>().Get(1, ComicIndex);
                result.Item2.ForEach(ComicCollect.Add);
            }
            if (param.Equals("2"))
            {
                RifanIndex += 1;
                if (RifanIndex > RifanTotal) return;
                var result = await Container.Resolve<ICandyService>().Get(2, RifanIndex);
                result.Item2.ForEach(RifanCollect.Add);
            }
            if (param.Equals("3"))
            {
                AxgleIndex += 1;
                if (AxgleIndex > AxgleTotal) return;
                var result = await Container.Resolve<ICandyService>().Get(3, AxgleIndex);
                result.Item2.ForEach(AxgleCollect.Add);
            }

        }

        private void ChangeMethod(dynamic input)
        {
            var param = (int)input;
            if (param == 1) InitComic();
            if (param == 2) InitRifan();
            if (param == 3) InitAxgle();
        }

        private void WatchMethod(CollectModel input)
        {
            //Comic
            if (input.Category == 1)
                Nav.NavigateAsync(new Uri(nameof(ComicInfo), UriKind.Relative), new NavigationParameters { { "Param", input.ToMapest<Sdk.Component.Vip.Comic.sdk.ViewModel.Response.SearchElementResult>() } });
            //Rifan
            if (input.Category == 2)
                Nav.NavigateAsync(new Uri(nameof(RifanInfo), UriKind.Relative), new NavigationParameters { { "Param", input.ToMapest<Sdk.Component.Vip.Anime.sdk.ViewModel.Response.SearchElementResult>() } });
            //Axgle
            if (input.Category == 3)
                Nav.NavigateAsync(new Uri(nameof(AxglePlay), UriKind.Relative), new NavigationParameters { { "Param", input.Route } });
        }
        private async void ClearMethod(string input)
        {
            await Container.Resolve<ICandyService>().Remove(input.AsInt());
        }
        #endregion
    }
}
