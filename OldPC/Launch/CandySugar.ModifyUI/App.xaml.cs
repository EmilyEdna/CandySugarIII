using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CandySugar.ModifyUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //日志
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs", "Modify.log"), rollingInterval: RollingInterval.Day)
                .CreateLogger();
            if (string.IsNullOrEmpty(e.Args.FirstOrDefault()))
            {
                this.Shutdown(0);
                return;
            }
            if (e.Args.FirstOrDefault() != "CandySugar")
            {
                this.Shutdown(0);
                return;
            }
            base.OnStartup(e);

        }
    }
}
