using CandySugar.Com.Library.OptionModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandySugar.Com.Library.GenericAction
{
    public class GenericDelegate
    {
        /// <summary>
        /// 搜索通知委托
        /// </summary>
        public static Action<SearchOptionModel> SearchAction { get; set; }
    }
}
