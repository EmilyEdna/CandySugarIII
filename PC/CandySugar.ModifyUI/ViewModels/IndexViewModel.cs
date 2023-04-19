using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Threading.Tasks;
using System.Windows;

namespace CandySugar.ModifyUI.ViewModels
{
    public class IndexViewModel : ObservableObject
    {
        private string Proxy = "https://hub.gitmirror.com/";
        private string RealRoute = "https://github.com/EmilyEdna/KuRuMi/releases/download/1.0/CandySugar.zip";
        private string TempFileZip = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CandySugar.Zip");
        public IndexViewModel()
        {
            Tip = "软件升级中，请稍后. . .";
            UpgradeFile();
        }

        #region Property
        private string _Result;
        public string Result
        {
            get => _Result;
            set => SetProperty(ref _Result, value);
        }
        private string _Tip;
        public string Tip
        {
            get => _Tip;
            set => SetProperty(ref _Tip, value);
        }
        #endregion

        #region Method
        private void UpgradeFile()
        {
            if (File.Exists(TempFileZip)) File.Delete(TempFileZip);
            var progressMessageHandler = new ProgressMessageHandler(new HttpClientHandler());
            progressMessageHandler.HttpReceiveProgress += (obj, args) =>
            {
                Result = args.ProgressPercentage + "%";
                if (args.ProgressPercentage == 100)
                {
                    ZipFile.ExtractToDirectory(TempFileZip, AppDomain.CurrentDomain.BaseDirectory, true);
                    Process.Start(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CandySugar.exe"));
                    Environment.Exit(0);
                }
            };
            Task.Run(async () =>
            {
                try
                {
                    using var client = new HttpClient(progressMessageHandler);
                    using var filestream = new FileStream(TempFileZip, FileMode.Create);
                    var netstream = await client.GetStreamAsync(Proxy + RealRoute);
                    await netstream.CopyToAsync(filestream);//写入文件
                }
                catch (Exception)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        if (MessageBox.Show($" 升级异常！手动前往下载：\n {RealRoute}") == MessageBoxResult.OK)
                            Environment.Exit(0);
                    });
                }
             
            });
        }
        #endregion
    }
}
