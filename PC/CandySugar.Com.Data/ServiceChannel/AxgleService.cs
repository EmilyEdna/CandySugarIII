using CandySugar.Com.Data.Entity.AxgleEntity;
using System;
using System.Collections.Generic;

namespace CandySugar.Com.Data.ServiceChannel
{
    public class AxgleService: IService<AxgleModel>
    {
        public void Insert(AxgleModel input)
        {
            input.Id = Guid.NewGuid();
            DataContext.Sqlite.Insert(input).ExecuteAffrows();
        }
        public void Remove(Guid Id)
        {
            DataContext.Sqlite.Delete<AxgleModel>().Where(t => t.Id == Id).ExecuteAffrows();
        }
        public List<AxgleModel> QueryAll()
        {
            return DataContext.Sqlite.Queryable<AxgleModel>().ToList();
        }
    }
}
