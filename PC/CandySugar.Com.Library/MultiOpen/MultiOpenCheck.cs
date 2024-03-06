using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.IO;

namespace CandySugar.Com.Library.MultiOpen
{
    public static class MultiOpenCheck
    {
        public static void AllowCheck(Action<string> action)
        {
            string MName = Process.GetCurrentProcess().MainModule.ModuleName;
            string PName = Path.GetFileNameWithoutExtension(MName);
            Process[] myProcess = Process.GetProcessesByName(PName);
            if (myProcess.Length > 1)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    action.Invoke("已经有一个实例在运行中");
                    Application.Current.Shutdown();
                    return;
                });
            }
        }
    }
}
