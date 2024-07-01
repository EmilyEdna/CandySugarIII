using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Comic
{
    public class MessageNotify
    {
        /// <summary>
        /// 通知类型
        /// </summary>
        public NotifyType NotifyType { get; set; }
        /// <summary>
        /// 控件参数
        /// </summary>
        public object ControlParam { get; set; }
    }
    public enum NotifyType
    {
        ChangeControl,
        Notify
    }
}
