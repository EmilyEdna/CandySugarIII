using FreeSql.DataAnnotations;
using System.Collections.Generic;

namespace CandySugar.Com.Data.Entity.HitomiEntity
{
    public class HitomiModel : BasicEntity
    {
        public string Title { get; set; }
        public string Cover { get; set; }
        public string OriginImage { get; set; }
        public string CId { get; set; }
        public string ReaderReferer { get; set; }
        [Column(IsIgnore = true)]
        public List<string> OriginImages { get; set; }
    }
}
