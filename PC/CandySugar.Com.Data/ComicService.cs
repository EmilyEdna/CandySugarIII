using CandySugar.Com.Data.Entity.AxgleEntity;
using CandySugar.Com.Data.Entity.ComicEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace CandySugar.Com.Data
{
    public class ComicService
    {
        public void Insert(ComicModel input)
        {
            input.Id = Guid.NewGuid();
            DataContext.Sqlite.Insert(input).ExecuteAffrows();
        }
        public void Remove(Guid Id)
        {
            DataContext.Sqlite.Delete<ComicModel>().Where(t => t.Id == Id).ExecuteAffrows();
        }
        public List<ComicModel> QueryAll()
        {
            return DataContext.Sqlite.Queryable<ComicModel>().ToList();
        }
    }
}
