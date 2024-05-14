using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace CandySugar.Com.Data.Entity
{
    public class BasicEntity
    {
        [Column(IsPrimary = true)]
        public Guid Id { get; set; }
    }
}
