using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CandySugar.Com.Data.Entity.NovelEntity
{
    public class NovelModel : BasicEntity
    {
        public string BookName { get; set; }
        public string Author { get; set; }
        public string Detail {  get; set; }
        public string Chapter { get; set; }   
        public string Route {  get; set; }
        public int Platform { get; set; }
        public int Current { get; set; }
    }
}
