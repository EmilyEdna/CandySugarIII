using System;
using System.Timers;
using System.Windows;

namespace CandySugar.Com.Library.Timers
{
    public class TimerHelper
    {
        private static Timer _Timer;
        public static void InitTimer(int span, Action<Timer> action)
        {
            _Timer = new Timer(span);
            _Timer.Elapsed += delegate { Application.Current.Dispatcher.Invoke(() => action.Invoke(_Timer)); };
        }
        public static void Stop() => _Timer?.Stop();
        public static void Start() => _Timer?.Start();
    }
}
