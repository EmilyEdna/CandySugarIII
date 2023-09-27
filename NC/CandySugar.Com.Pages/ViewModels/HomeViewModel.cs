using CandySugar.Com.Library.BaseViewModel;
using CandySugar.Com.Pages.ViewModels.HomeViewModels;
using CandySugar.Com.Pages.Views.AxgleViews;
using CandySugar.Com.Pages.Views.ComicViews;
using CandySugar.Com.Pages.Views.HomeViews;
using CandySugar.Com.Pages.Views.RifanViews;
using CandySugar.Com.Service;
using System.Collections.ObjectModel;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Pages.ViewModels
{
    public class HomeViewModel : BaseVMModule
    {
        private BaseVMService BaseServices;
        public HomeViewModel(BaseVMService baseServices) : base(baseServices)
        {
            BaseServices = baseServices;
        }

        #region Property
        private View _Views;
        public View Views { get => _Views; set => SetProperty(ref _Views, value); }
        #endregion

        #region Command

        public void SetContent(int Parameter)
        {
            Application.Current.Dispatcher.DispatchAsync(() =>
            {
                if (Parameter == 1)
                    Views = new HomeComic
                    {
                        BindingContext = new HomeComicViewModel(BaseServices)
                    };
                if (Parameter == 2)
                    Views = new HomeRifan
                    {
                        BindingContext = new HomeRifanViewModel(BaseServices)
                    };
                if (Parameter == 3)
                    Views = new HomeAxgle
                    {
                        BindingContext = new HomeAxgleViewModel(BaseServices)
                    };
            });
        }
        #endregion
    }
}
