namespace CandySugar.Bilibili.Models
{
    public partial class BiliVideoInfoModel: ObservableObject
    {
        public string Cover { get; set; }

        public string BVID { get; set; }

        public string CID { get; set; }

        public string Title { get; set; }

        [ObservableProperty]
        private double _Width;
    }
}
