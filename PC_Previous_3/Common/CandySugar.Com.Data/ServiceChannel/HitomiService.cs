using CandySugar.Com.Data.Entity.HitomiEntity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CandySugar.Com.Data.ServiceChannel
{
    public class HitomiService : IService<HitomiModel>
    {
        public Guid Insert(HitomiModel input)
        {
            input.PId = Guid.NewGuid();
            input.OriginImage = string.Join("|", input.OriginImages);
            DataContext.Sqlite.Insert(input).ExecuteAffrows();
            return input.PId;
        }

        public List<HitomiModel> QueryAll()
        {
            var data = DataContext.Sqlite.Queryable<HitomiModel>().ToList();
            data.ForEach(item =>
            {
                item.OriginImages = item.OriginImage.Split("|").ToList();
            });
            return data;
        }

        public void Remove(Guid Id)
        {
            DataContext.Sqlite.Delete<HitomiModel>().Where(t => t.PId == Id).ExecuteAffrows();
        }
    }
}
