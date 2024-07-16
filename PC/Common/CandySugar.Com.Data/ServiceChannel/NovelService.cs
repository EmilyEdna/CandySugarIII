using CandySugar.Com.Data.Entity.NovelEntity;
using System;
using System.Collections.Generic;

namespace CandySugar.Com.Data.ServiceChannel
{
    public class NovelService : IService<NovelModel>
    {
        public Guid Insert(NovelModel input)
        {
            input.PId = Guid.NewGuid();
            if (!DataContext.Sqlite.Queryable<NovelModel>().Any(t => t.BookName == input.BookName)) 
            {
                DataContext.Sqlite.Insert(input).ExecuteAffrows();
                return input.PId;
            }
            return input.PId;

        }

        public List<NovelModel> QueryAll()
        {
           return DataContext.Sqlite.Queryable<NovelModel>().ToList();
        }

        public void Remove(Guid Id)
        {
            DataContext.Sqlite.Delete<NovelModel>().Where(t => t.PId == Id).ExecuteAffrows();
        }
    }
}
