using System;
using System.Collections.Generic;
using System.Text;

namespace CandySugar.Com.Data.Entity.WallEntity
{
    public class WallModel : BasicEntity
    {
        public string Preview {  get; set; }
        public string Original { get; set; }
        public string OriginalPng {  get; set; }
        public string OriginalJepg {  get; set; }
        public string Pixel { get; set; }
        public int Platform {  get; set; }
    }
}
