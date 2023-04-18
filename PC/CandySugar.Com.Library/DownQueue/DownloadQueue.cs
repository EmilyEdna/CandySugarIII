using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CandySugar.Com.Library.DownQueue
{
    public static class DownloadQueue
    {
        private static Queue<Tuple<string, Enum, FrameworkElement>> Datas;
        private static AutoResetEvent AutoEvent;
        public static Func<string, Enum, Task<byte[]>> ResultFunc { get; set; }
        public static Action<FrameworkElement, byte[]> DownEventAction { get; set; }
        static DownloadQueue()
        {
            AutoEvent = new AutoResetEvent(true);
            Datas = new Queue<Tuple<string, Enum, FrameworkElement>>();
            (new Thread(new ThreadStart(DownMethod))
            {
                IsBackground = true
            }).Start();
        }

        private static async void DownMethod()
        {
            while (true)
            {
                Tuple<string, Enum, FrameworkElement> Data = null;
                lock (Datas)
                {
                    if (Datas.Count > 0)
                    {
                        Data = Datas.Dequeue();
                    }
                }
                if (Data != null)
                {
                    try
                    {
                        var Bytes = await ResultFunc?.Invoke(Data.Item1, Data.Item2);
                        Data.Item3.Dispatcher
                            .BeginInvoke(new Action<FrameworkElement, byte[]>((Framework, bytes) => DownEventAction?.Invoke(Framework, bytes)), new object[] { Data.Item3, Bytes });
                    }
                    catch (Exception ex)
                    {
                        Log.Logger.Error(ex, "");
                    }
                }
                if (Datas.Count > 0) continue;
                //阻塞线程
                AutoEvent.WaitOne();
            }
        }

        public static void Init(string route, Enum enums, FrameworkElement element)
        {
            lock (Datas)
            {
                Datas.Enqueue(Tuple.Create(route, enums, element));
                AutoEvent.Set();
            }
        }
    }
}
