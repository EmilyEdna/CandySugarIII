using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XExten.Advance.IocFramework;

namespace CandySugar.Com.Service
{
    public static class ServiceModule
    {
        public static MauiAppBuilder AddServiceHandler(this MauiAppBuilder builder)
        {
            DbContext.InitTabel();
            IocDependency.Register<ICandyService, CandyService>(1);
            return builder;
        }
    }
}
