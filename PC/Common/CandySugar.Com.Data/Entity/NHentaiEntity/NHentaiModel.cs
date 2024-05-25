using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;

namespace CandySugar.Com.Data.Entity.NHentaiEntity
{
    public class NHentaiModel : BasicEntity
    {
        public long Id { get; set; }
        public long MediaId {  get; set; }
        public string Cover {  get; set; }
        public string Name {  get; set; }
        public DateTime UploadDate {  get; set; }
        [Column(IsIgnore = true)]
        public List<string> ThumbImages {  get; set; }
        public string ThumbImage {  get; set; }
        [Column(IsIgnore = true)]
        public List<string> OriginImages {  get; set; }
        public string OriginImage { get; set; }
        [Column(IsIgnore = true)]
        public List<string> ImageType {  get; set; }
    }
}
