using CandySugar.Com.Data.Entity.MusicEntity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CandySugar.Com.Data.ServiceChannel
{
    public class MusicService : IService<MusicModel>
    {
        public Guid Insert(MusicModel input)
        {
            input.PId = Guid.NewGuid();
            input.SongArtistStr = string.Join("|", input.SongArtistId);
            input.SongArtistNameStr = string.Join("|", input.SongArtistName);
            DataContext.Sqlite.Insert(input).ExecuteAffrows();
            return input.PId;
        }
        public void Remove(Guid Id)
        {
            DataContext.Sqlite.Delete<MusicModel>().Where(t => t.PId == Id).ExecuteAffrows();
        }
        public List<MusicModel> QueryAll()
        {
            var data = DataContext.Sqlite.Queryable<MusicModel>().ToList();
            data.ForEach(item =>
            {
                item.SongArtistId = item.SongArtistStr.Split("|").ToList();
                item.SongArtistName = item.SongArtistNameStr.Split("|").ToList();
            });
            return data;
        }
    }
}
