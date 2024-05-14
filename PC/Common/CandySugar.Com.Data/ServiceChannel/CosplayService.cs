using CandySugar.Com.Data.Entity.ComicEntity;
using CandySugar.Com.Data.Entity.CosplayEntity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CandySugar.Com.Data.ServiceChannel
{
    public class CosplayService : IService<CosplayModel>
    {
        public Guid Insert(CosplayModel input)
        {
            input.PId = Guid.NewGuid();
            input.Picture = string.Join("|", input.Images);
            DataContext.Sqlite.Insert(input).ExecuteAffrows();
            return input.PId;
        }
        public void Remove(Guid Id)
        {
            DataContext.Sqlite.Delete<CosplayModel>().Where(t => t.PId == Id).ExecuteAffrows();
        }
        public List<CosplayModel> QueryAll()
        {
            var data = DataContext.Sqlite.Queryable<CosplayModel>().ToList();
            data.ForEach(item =>
            {
                item.Images = item.Picture.Split("|").ToList();
            });
            return data;
        }
    }
}
