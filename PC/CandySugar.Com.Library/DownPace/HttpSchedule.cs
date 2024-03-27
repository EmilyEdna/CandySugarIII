using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Handlers;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using XExten.Advance.NetFramework;
using XExten.Advance.StaticFramework;

namespace CandySugar.Com.Library.DownPace
{
    public class HttpSchedule
    {
        private static double DownCount { get; set; }
        public static Action<double, double> ReceiveAction { get; set; }
        private static ProgressMessageHandler ProgressHandler()
        {
            var ProgressHandler = new ProgressMessageHandler(new HttpClientHandler());
            ProgressHandler.HttpReceiveProgress += HttpReceiveProgress;
            return ProgressHandler;
        }

        private static void HttpReceiveProgress(object sender, HttpProgressEventArgs e)
        {
            ReceiveAction?.Invoke(double.Parse((e.ProgressPercentage / DownCount).ToString("F2")), DownCount);
        }

        public static async Task HttpDownload(string uri, string file, Action<HttpRequestHeaders> action = null)
        {
            HttpClient Client = new HttpClient(ProgressHandler());
            Client.DefaultRequestHeaders.Add(ConstDefault.UserAgent, ConstDefault.UserAgentValue);
            action?.Invoke(Client.DefaultRequestHeaders);
            var stream = await Client.GetStreamAsync(uri);
            SyncStatic.DeleteFile(file);
            using FileStream fs = new FileStream(file, FileMode.CreateNew);
            await stream.CopyToAsync(fs);
        }

        public static async Task HttpDownload(Dictionary<string, string> data, Action<HttpRequestHeaders> action = null)
        {
            DownCount = data.Count;
            HttpClient Client = new HttpClient(ProgressHandler());
            Client.DefaultRequestHeaders.Add(ConstDefault.UserAgent, ConstDefault.UserAgentValue);
            action?.Invoke(Client.DefaultRequestHeaders);
            foreach (var item in data)
            {
                try
                {
                    var stream = await Client.GetStreamAsync(item.Value);
                    SyncStatic.DeleteFile(item.Key);
                    using FileStream fs = new FileStream(item.Key, FileMode.CreateNew);
                    await stream.CopyToAsync(fs);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                }
            }
        }
    }
}
