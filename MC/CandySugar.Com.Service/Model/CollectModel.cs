using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Com.Service
{
    public class CollectModel : BasicEntity
    {
        /// <summary>
        /// 1漫画2里番3小说4动漫
        /// </summary>
        public int Category { get; set; }
        public string Cover { get; set; }
        public string Name { get; set; }
        public string Route { get; set; }
        public string Hash { get; set; }
    }
}
