using CandySugar.Com.Data.Entity.HistoryEntity;
using System;
using System.Collections.Generic;

namespace CandySugar.Com.Data.ServiceChannel
{
    public class HistoryService : IService<HistoryModel>
    {
        public Guid Insert(HistoryModel input)
        {
            input.PId = Guid.NewGuid();
            DataContext.Sqlite.Insert(input).ExecuteAffrows();
            return input.PId;
        }

        public List<HistoryModel> QueryAll()
        {
            return DataContext.Sqlite.Queryable<HistoryModel>().ToList();
        }

        public void Remove(Guid Id)
        {
            DataContext.Sqlite.Delete<HistoryModel>().Where(t => t.PId == Id).ExecuteAffrows();
        }
    }
}
