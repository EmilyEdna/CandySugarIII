using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Com.Service
{
    public class BasicEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }
        public DateTime Span { get; set; }
        public void InitProperty()
        {
            this.Id = Guid.NewGuid();
            this.Span = DateTime.Now;
        }
    }
}
