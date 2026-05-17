using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace CandySugar.Com.Library.Extends
{
    public static class AppExtend
    {
        public const string NoSelectCategory = "未选择分类";
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
    }
}
