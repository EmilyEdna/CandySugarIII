using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Com.Options.ComponentGeneric
{
    /// <summary>
    /// 组件泛用委托
    /// </summary>
    public class GenericDelegate
    {
        /// <summary>
        /// 长宽通知委托
        /// </summary>
        public static Action<double, double> InformationAction { get; set; }
        /// <summary>
        /// 搜索通知委托
        /// </summary>
        public static Action<string> SearchAction { get; set; }
        /// <summary>
        /// 操作委托
        /// </summary>
        public static Action<object> HandleAction { get; set; }
        /// <summary>
        /// 剪切板内容通知
        /// </summary>
        public static Action<object> ClipboardAction { get; set; }
    }
}
