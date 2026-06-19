using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using XExten.Advance.NetFramework;
using XExten.Advance;
using Platform = XExten.Advance.NetFramework.Enums.Platform;
using XExten.Advance.InternalFramework.Email;

namespace CandySugar.Com.Library
{
    public static class Extend
    {
        public async static void Info(this string input, bool IsLong = false)
        {
            try
            {
                await Toast.Make(input, IsLong ? ToastDuration.Long : ToastDuration.Short).Show();
            }
            catch (Exception)
            {
                //解决Toast在子线程问题
#if ANDROID
                Android.OS.Looper.Prepare();
                await Toast.Make(input, IsLong ? ToastDuration.Long : ToastDuration.Short).Show();
                Android.OS.Looper.Loop();
#endif
            }
        }

        public static MauiAppBuilder AddControlHandler(this MauiAppBuilder builder)
        {
            NetFactoryExtension.RegisterNetFramework(1, Platform.Android);
            HttpEvent.HttpActionEvent = new((Client, Ex) => Ex.Message.Info());
            return builder;
        }

        public static Dictionary<string, string> RouteMap = new()
        {
             {"RootView","//RootView" },
             {"AxgleView","//AxgleView" },
             {"AvgleView","//AvgleView" },
             
            { "LinkView","//AxgleView/LinkView"},
            { "VideoView","//AxgleView/VideoView"},
         };
    }
}
