using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using XExten.Advance.Maui.MainActivity;

namespace CandySugar.MainUI
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CrossCurrentActivity.Current.Init(this, savedInstanceState);

            AndroidEnvironment.UnhandledExceptionRaiser += AndroidException;
        }
        protected override async void OnResume()
        {
            base.OnResume();
            await Task.Delay(500);
        }

        /// <summary>
        /// 全局异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AndroidException(object sender, RaiseThrowableEventArgs e)
        {
            Thread.Sleep(2000);
            e.Handled = true;
        }
    }
}