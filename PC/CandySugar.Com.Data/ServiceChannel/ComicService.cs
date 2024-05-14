using CandySugar.Com.Data.Entity.AxgleEntity;
using CandySugar.Com.Data.Entity.ComicEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CandySugar.Com.Data.ServiceChannel
{
    public class ComicService: IService<ComicModel>
    {
        public Guid Insert(ComicModel input)
        {
            input.PId = Guid.NewGuid();
            DataContext.Sqlite.Insert(input).ExecuteAffrows();
            return input.PId;
        }
        public void Remove(Guid Id)
        {
            DataContext.Sqlite.Delete<ComicModel>().Where(t => t.PId == Id).ExecuteAffrows();
        }
        public List<ComicModel> QueryAll()
        {
            return DataContext.Sqlite.Queryable<ComicModel>().ToList();
        }
    }
}
