﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CandySugar.Com.Options.ComponentGeneric
{
    /// <summary>
    /// 组件泛用委托
    /// </summary>
    public class GenericDelegate
    {
        /// <summary>
        /// 搜索通知委托
        /// </summary>
        public static Action<string> SearchAction { get; set; }
        /// <summary>
        /// 剪切板内容通知
        /// </summary>
        public static Action<object> ClipboardAction { get; set; }
        /// <summary>
        /// 窗体变化
        /// </summary>
        public static event Action<WindowState> WindowStateEvent;
        public static void WindowStateAction(WindowState state)=> WindowStateEvent?.Invoke(state);
    }
}
