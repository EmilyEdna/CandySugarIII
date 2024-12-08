namespace CandySugar.Com.Options.ComponentObject
{
    public class OptionObjectModel
    {
        /// <summary>
        /// 缓存时常
        /// </summary>
        public int Cache { get; set; }
        /// <summary>
        /// 背景图片地址
        /// </summary>
        public string BackgroudLocation { get; set; }
        /// <summary>
        /// 变换间隔时常
        /// </summary>
        public double Interval { get; set; }
        /// <summary>
        /// github访问加速
        /// </summary>
        public string Raw { get; set; }
        /// <summary>
        /// 启用代理地址
        /// </summary>
        public bool UseProxy { get; set; }
    }
}
