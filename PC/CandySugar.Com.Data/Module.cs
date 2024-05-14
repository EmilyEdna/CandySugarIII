using System;
using System.Collections.Generic;
using System.Text;
using CandySugar.Com.Data.Entity.AxgleEntity;
using CandySugar.Com.Data.ServiceChannel;

namespace CandySugar.Com.Data
{
    public class Module
    {
        public static Dictionary<Type,Type> Services => new Dictionary<Type, Type>
        {
            { typeof(AxgleService),typeof(IService<AxgleModel>) },
            { typeof(ComicService),typeof(IService<AxgleModel>) },
            { typeof(AxgleService),typeof(IService<AxgleModel>) },
        };
    }
}
