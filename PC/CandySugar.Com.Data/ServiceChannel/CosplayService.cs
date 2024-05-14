using CandySugar.Com.Data.Entity.ComicEntity;
using CandySugar.Com.Data.Entity.CosplayEntity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CandySugar.Com.Data.ServiceChannel
{
    public class CosplayService: IService<CosplayModel>
    {
        public void Insert(CosplayModel input)
        {
            input.Id = Guid.NewGuid();
            input.Picture = string.Join("|", input.Images);
            DataContext.Sqlite.Insert(input).ExecuteAffrows();
        }
        public void Remove(Guid Id)
        {
            DataContext.Sqlite.Delete<CosplayModel>().Where(t => t.Id == Id).ExecuteAffrows();
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
