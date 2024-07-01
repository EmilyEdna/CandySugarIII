using System.Collections.Generic;

namespace CandySugar.Novel.Models
{
    internal class ContentDataModel
    {
        public PlatformEnum Platform {  get; set; }
        public int Index {  get; set; }
        public List<NovelDetailElementResult> Chapters { get; set; }
        public string Current {  get; set; }
    }
}
