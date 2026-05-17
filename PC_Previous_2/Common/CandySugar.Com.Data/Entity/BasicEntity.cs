using FreeSql.DataAnnotations;
using System;

namespace CandySugar.Com.Data.Entity
{
    public class BasicEntity
    {
        [Column(IsPrimary = true)]
        public Guid PId { get; set; }
    }
}
