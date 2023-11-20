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
            HttpEvent.RestActionEvent = new((Client, Ex) => Ex.Message.Info());
            HttpEvent.HttpActionEvent = new((Client, Ex) => Ex.Message.Info());
            return builder;
        }

        public static MauiAppBuilder AddEmailHandler(this MauiAppBuilder builder)
        {
            EmailSetting.SetOption("smtp.qq.com", "847432003@qq.com", "odvbqicdusiobfed");
            return builder;
        }

        public static Dictionary<string, string> RouteMap = new()
        {
             {"RootView","//RootView" },
             {"RifanView","//RifanView" },
             {"NovelView","//NovelView" },
             {"ComicView","//ComicView" },
             {"AnimeView","//AnimeView" },

             {"ChapterView","//NovelView/ChapterView" },
             {"ReaderView","//NovelView/ChapterView/ReaderView" },

             {"CatalogView","//ComicView/CatalogView" },
             {"VisitView","//ComicView/CatalogView/VisitView" },

             {"CollectView","//AnimeView/CollectView" },
             {"PlayView","//AnimeView/CollectView/PlayView" },

            { "DetailView","//RifanView/DetailView"},
            { "WatchView","//RifanView/DetailView/WatchView"},
         };
    }
}
