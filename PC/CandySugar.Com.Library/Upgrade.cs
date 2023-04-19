using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using XExten.Advance.LinqFramework;
using XExten.Advance.RestHttpFramework;
using XExten.Advance.RestHttpFramework.Options;
using XExten.Advance.StaticFramework;

namespace CandySugar.Com.Library
{
    public class Upgrade
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
                    Nofity(CommonHelper.ProgramErronInformation);
                });
                Task.Run(async () =>
                {
                    await Task.Delay(3000);
                    Environment.Exit(0);
                });
            }

        }

        private static async void CheckModifyVersion()
        {
            try
            {
                var ver = await IRestHttpClient.Rest.UseNode(opt =>
                {
                    opt.Provider = RestProviderMethod.GET;
                    opt.Route = $"{ComponentBinding.OptionObjectModels.Raw}/EmilyEdna/CandySugar/master/vers.txt";
                }).RunStringFirstAsync();
                if (!ver.IsNullOrEmpty())
                {
                    var newVer = ver.Replace("\n", "");
                    if (!newVer.Equals(CommonHelper.Version))
                    {
                        var exe = Path.Combine(CommonHelper.AppPath, "CandySugarModify.exe");
                        Process.Start(exe);
                        Environment.Exit(0);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "");
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Nofity(CommonHelper.VersionErronInformation);
                });
                Task.Run(async () =>
                {
                    await Task.Delay(3000);
                    Environment.Exit(0);
                });
            }
        }

        private static void Nofity(string param)
        {
            var NofityView = SyncStatic.Assembly("CandySugar.Com.Controls").SelectMany(t => t.ExportedTypes.Where(x => x.BaseType == typeof(Window)))
                   .Where(t => t.Name == "ScreenNotifyView").FirstOrDefault();
            ((dynamic)Activator.CreateInstance(NofityView, new object[] { param })).Show();
        }
    }
}
