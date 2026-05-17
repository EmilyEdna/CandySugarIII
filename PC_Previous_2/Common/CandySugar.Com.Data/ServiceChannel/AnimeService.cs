using CandySugar.Com.Data.Entity.AnimeEntity;
using System;
using System.Collections.Generic;

namespace CandySugar.Com.Data.ServiceChannel
{
    public class AnimeService : IService<AnimeModel>
    {
        public Guid Insert(AnimeModel input)
        {
            input.PId = Guid.NewGuid();
            DataContext.Sqlite.Insert(input).ExecuteAffrows();
            return input.PId;
        }

        public List<AnimeModel> QueryAll()
        {
            return DataContext.Sqlite.Queryable<AnimeModel>().ToList();
        }

        public void Remove(Guid Id)
        {
            DataContext.Sqlite.Delete<AnimeModel>().Where(t => t.PId == Id).ExecuteAffrows();
        }
    }
}
