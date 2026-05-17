using FreeSql.DataAnnotations;
using System.Collections.Generic;

namespace CandySugar.Com.Data.Entity.CosplayEntity
{
    public class CosplayModel : BasicEntity
    {
        public int Platform { get; set; }
        public string Title { get; set; }
        public string Route { get; set; }
        public string Cover { get; set; }
        [Column(IsIgnore = true)]
        public List<string> Images { get; set; }
        public string Picture { get; set; }
    }
}
