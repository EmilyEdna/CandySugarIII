using System;
using System.Collections.Generic;
using System.Text;

namespace CandySugar.Com.Data.Entity.RifanEntity
{
    public class RifanModel : BasicEntity
    {
        public string Route { get; set; }
        public string Name { get; set; }
        public string Cover { get; set; }
        public string Duration { get; set; }
        public string Views { get; set; }
    }
}
