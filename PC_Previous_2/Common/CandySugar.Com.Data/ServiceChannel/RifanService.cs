using CandySugar.Com.Data.Entity.RifanEntity;
using System;
using System.Collections.Generic;

namespace CandySugar.Com.Data.ServiceChannel
{
    public class RifanService : IService<RifanModel>
    {
        public Guid Insert(RifanModel input)
        {
            input.PId = Guid.NewGuid();
            DataContext.Sqlite.Insert(input).ExecuteAffrows();
            return input.PId;
        }

        public void Remove(Guid Id)
        {
            DataContext.Sqlite.Delete<RifanModel>().Where(t => t.PId == Id).ExecuteAffrows();
        }

        public List<RifanModel> QueryAll()
        {
            return DataContext.Sqlite.Queryable<RifanModel>().ToList();
        }
    }
}
