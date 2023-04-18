using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.LightNovel
{
    public class MessageNotify
    {
        /// <summary>
        /// 侧边栏开关状态 1 开 2关
        /// </summary>
        public int SliderStatus { get; set; }
        /// <summary>
        /// 控件类型 1首页控件 2阅读控件
        /// </summary>
        public int ControlType { get; set; }
        /// <summary>
        /// 控件参数
        /// </summary>
        public object ControlParam { get; set; }
        /// <summary>
        /// 通知类型
        /// </summary>
        public NotifyType NotifyType { get; set; } = NotifyType.Notify;
    }
    public enum NotifyType
    {
        ChangeControl,
        Notify
    }
}
