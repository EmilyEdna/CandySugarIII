using System;

namespace CandySugar.Com.Data.Entity.WallEntity
{
    public class WallModel : BasicEntity
    {
        public string Id { get; set; }

        public string Author { get; set; }

        public string Cover { get; set; }

        public string Original { get; set; }

        public DateTime CreateAt { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }
    }
}
