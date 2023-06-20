using CandySugar.Com.Service.IServiceImpl;
using CandySugar.Com.Service.ServiceImpl;

namespace CandySugar.Com.Service
{
    public class ServiceModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            DbContext.InitTabel();
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ICandyService, CandyService>();
        }
    }
}
