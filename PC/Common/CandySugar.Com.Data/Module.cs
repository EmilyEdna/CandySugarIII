using CandySugar.Com.Data.Entity.AnimeEntity;
using CandySugar.Com.Data.Entity.AxgleEntity;
using CandySugar.Com.Data.Entity.ComicEntity;
using CandySugar.Com.Data.Entity.CosplayEntity;
using CandySugar.Com.Data.Entity.HistoryEntity;
using CandySugar.Com.Data.Entity.MusicEntity;
using CandySugar.Com.Data.Entity.NHentaiEntity;
using CandySugar.Com.Data.Entity.NovelEntity;
using CandySugar.Com.Data.Entity.RifanEntity;
using CandySugar.Com.Data.Entity.WallEntity;
using CandySugar.Com.Data.ServiceChannel;
using System;
using System.Collections.Generic;

namespace CandySugar.Com.Data
{
    public class Module
    {
        public static Dictionary<Type, Type> Services => new Dictionary<Type, Type>
        {
            { typeof(AxgleService),typeof(IService<AxgleModel>) },
            { typeof(ComicService),typeof(IService<ComicModel>) },
            { typeof(CosplayService),typeof(IService<CosplayModel>) },
            { typeof(RifanService),typeof(IService<RifanModel>) },
            { typeof(WallService),typeof(IService<WallModel>) },
            { typeof(MusicService),typeof(IService<MusicModel>) },
            { typeof(NHentaiService),typeof(IService<NHentaiModel>) },
            { typeof(AnimeService),typeof(IService<AnimeModel>) },
            { typeof(NovelService),typeof(IService<NovelModel>) },
            { typeof(HistoryService),typeof(IService<HistoryModel>) }
        };
    }
}
