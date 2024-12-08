using FreeSql.DataAnnotations;
using System.Collections.Generic;

namespace CandySugar.Com.Data.Entity.MusicEntity
{
    public class MusicModel : BasicEntity
    {
        public int MusicPlatformType { get; set; }
        public string SongId { get; set; }
        public string SongName { get; set; }
        public string SongAlbumId { get; set; }
        public string SongAlbumName { get; set; }
        [Column(IsIgnore = true)]
        public List<string> SongArtistId { get; set; }
        public string SongArtistStr { get; set; }
        [Column(IsIgnore = true)]
        public List<string> SongArtistName { get; set; }
        public string SongArtistNameStr { get; set; }
    }
}
