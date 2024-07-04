using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;

namespace CandySugar.Com.Data.Entity.HistoryEntity
{
    public class HistoryModel: BasicEntity
    {
        [Column(IsNullable=true)]
        public string Name {  get; set; }
        public string Route {  get; set; }
        public DateTime AddDate { get; set; }
    }
}
