using CandySugar.Com.Controls.UIExtenControls;
using CandySugar.Com.Library;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using XExten.Advance.LinqFramework;

namespace CandySugar.MainUI
{
    public class Modify
    {
        public static void CandySugarModify()
        {
            var Files = Directory.GetFiles(CommonHelper.AppPath).Select(Path.GetFileName)
                .Where(t => t.Contains("CandySugar"))
                .Where(t => Path.GetExtension(t).Equals(".dll") || Path.GetExtension(t).Equals(".exe")).ToList();

            var Target = Files.Where(t => t.Contains("CandySugar.ModifyUI.dll") || t.Contains("CandySugarModify.exe")).Count();
            if (Target == 2)
                Task.Run(CheckModifyVersion);
            else
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    new ScreenNotifyView(CommonHelper.ProgramErronInformation).Show();
                    Task.Run(async () =>
                    {
                        await Task.Delay(3000);
                        Environment.Exit(0);
                    });
                });
            }

        }

        private static async void CheckModifyVersion()
        {
            try
            {
                var ver = await new HttpClient().GetStringAsync($"{ComponentBinding.OptionObjectModels.Raw}/EmilyEdna/CandySugar/master/vers.txt");
                if (!ver.IsNullOrEmpty())
                {
                    if (ver.Contains("\n"))
                        ver = ver.Replace("\n", "");
                    if (!ver.Equals(CommonHelper.Version))
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            new ScreenNotifyView(CommonHelper.UpgradeInformation).Show();
                            Task.Run(async () =>
                            {
                                await Task.Delay(3000);
                                var exe = Path.Combine(CommonHelper.AppPath, "CandySugarModify.exe");
                                Process.Start(exe, "CandySugar");
                                Environment.Exit(0);
                            });
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    new ScreenNotifyView(CommonHelper.VersionErronInformation).Show();
                    Task.Run(async () =>
                    {
                        await Task.Delay(3000);
                        Environment.Exit(0);
                    });
                });
            }
        }
    }
}
