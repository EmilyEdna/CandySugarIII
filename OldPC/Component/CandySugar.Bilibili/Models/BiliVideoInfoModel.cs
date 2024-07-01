namespace CandySugar.Bilibili.Models
{
    public class BiliVideoInfoModel: PropertyChangedBase
    {
        public string Cover { get; set; }

        public string BVID { get; set; }

        public string CID { get; set; }

        public string Title { get; set; }
        private double _Width;
        public double Width
        {
            get => _Width;
            set => SetAndNotify(ref _Width, value);
        }
    }
}
