using CandySugar.Com.Pages.ViewModels.RifanViewModels;
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
        }
    }
}
