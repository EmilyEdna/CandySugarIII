using CandySugar.Com.Data.Entity.ComicEntity;
using CandySugar.Com.Data.Entity.RifanEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CandySugar.Com.Data.ServiceChannel
{
    public class RifanService : IService<RifanModel>
    {
        public Guid Insert(RifanModel input)
        {
            input.Id = Guid.NewGuid();
            DataContext.Sqlite.Insert(input).ExecuteAffrows();
            return input.Id;
        }

        public void Remove(Guid Id)
        {
            DataContext.Sqlite.Delete<RifanModel>().Where(t => t.Id == Id).ExecuteAffrows();
        }

        public List<RifanModel> QueryAll()
        {
            return DataContext.Sqlite.Queryable<RifanModel>().ToList();
        }
    }
}
