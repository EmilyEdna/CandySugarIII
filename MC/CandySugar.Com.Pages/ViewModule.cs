using CandySugar.Com.Pages.ViewModels.AxgleViewModels;
using CandySugar.Com.Pages.ViewModels.ComicViewModels;
using CandySugar.Com.Pages.ViewModels.RifanViewModels;
using CandySugar.Com.Pages.Views.AxgleViews;
using CandySugar.Com.Pages.Views.ComicViews;
using CandySugar.Com.Pages.Views.RifanViews;

namespace CandySugar.Com.Pages
{
    public class ViewModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<RifanInfo, RifanInfoViewModel>();
            containerRegistry.RegisterForNavigation<RifanPlay, RifanPlayViewModel>();

            containerRegistry.RegisterForNavigation<ComicInfo, ComicInfoViewModel>();
            containerRegistry.RegisterForNavigation<ComicWatch, ComicWatchViewModel>();

            containerRegistry.RegisterForNavigation<AxgleInfo, AxgleInfoViewModel>();
            containerRegistry.RegisterForNavigation<AxglePlay, AxglePlayViewModel>();
        }
    }
}
