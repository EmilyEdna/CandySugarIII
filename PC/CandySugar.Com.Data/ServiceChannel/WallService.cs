using CandySugar.Com.Data.Entity.WallEntity;
using System;
using System.Collections.Generic;

namespace CandySugar.Com.Data.ServiceChannel
{
    public class WallService : IService<WallModel>
    {
        public Guid Insert(WallModel input)
        {
           input.PId= Guid.NewGuid();
            DataContext.Sqlite.Insert(input).ExecuteAffrows();
            return input.PId;
        }

        public void Remove(Guid Id)
        {
            DataContext.Sqlite.Delete<WallModel>().Where(t => t.PId == Id).ExecuteAffrows();
        }

        public List<WallModel> QueryAll()
        {
            return DataContext.Sqlite.Queryable<WallModel>().ToList();
        }
    }
}
