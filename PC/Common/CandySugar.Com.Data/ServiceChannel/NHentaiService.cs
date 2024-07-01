using CandySugar.Com.Data.Entity.NHentaiEntity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CandySugar.Com.Data.ServiceChannel
{
    public class NHentaiService : IService<NHentaiModel>
    {
        public Guid Insert(NHentaiModel input)
        {
            input.PId = Guid.NewGuid();
            input.ThumbImage = string.Join("|", input.ThumbImages);
            input.OriginImage = string.Join("|", input.OriginImages);
            DataContext.Sqlite.Insert(input).ExecuteAffrows();
            return input.PId;
        }

        public List<NHentaiModel> QueryAll()
        {
            var data = DataContext.Sqlite.Queryable<NHentaiModel>().ToList();
            data.ForEach(item =>
            {
                item.OriginImages = item.OriginImage.Split("|").ToList();
                item.ThumbImages = item.ThumbImage.Split("|").ToList();
            });
            return data;
        }

        public void Remove(Guid Id)
        {
            DataContext.Sqlite.Delete<NHentaiModel>().Where(t => t.PId == Id).ExecuteAffrows();
        }
    }
}
