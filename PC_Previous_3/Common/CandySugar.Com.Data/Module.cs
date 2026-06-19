using System;
using System.Collections.Generic;
using CandySugar.Com.Data.Entity.AxgleEntity;
using CandySugar.Com.Data.Entity.CosplayEntity;
using CandySugar.Com.Data.Entity.HistoryEntity;
using CandySugar.Com.Data.Entity.MusicEntity;
using CandySugar.Com.Data.Entity.WallEntity;
using CandySugar.Com.Data.ServiceChannel;

namespace CandySugar.Com.Data
{
    public class Module
    {
        public static Dictionary<Type, Type> Services => new Dictionary<Type, Type>
        {
            { typeof(AxgleService),typeof(IService<AxgleModel>) },
            { typeof(CosplayService),typeof(IService<CosplayModel>) },
            { typeof(WallService),typeof(IService<WallModel>) },
            { typeof(MusicService),typeof(IService<MusicModel>) },
            { typeof(HistoryService),typeof(IService<HistoryModel>) }
        };
    }
}
