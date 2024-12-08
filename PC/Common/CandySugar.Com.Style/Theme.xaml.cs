using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace CandySugar.Com.Style
{
    /// <summary>
    /// Theme.xaml 的交互逻辑
    /// </summary>
    public partial class Theme : ResourceDictionary
    {
        private Stopwatch Watch;
        /// <summary>
        /// 背景轮询队列
        /// </summary>
        private ConcurrentQueue<string> BackQueue;
        public Theme()
        {
            Watch = new();
            BackQueue = new();
            CompositionTarget.Rendering += AnimetionEvent;
            Watch.Start();
        }

        /// <summary>
        /// 利用关键帧动态切换背景图(呼吸效果动效)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AnimetionEvent(object sender, EventArgs args)
        {

        }
    }
}
