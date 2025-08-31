using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using System.Text.RegularExpressions;
using XExten.Advance.LinqFramework;
#if ANDROID
using XExten.Advance.Maui.Bar;
using XExten.Advance.Maui.Direction;
using XExten.Advance.Maui.Direction.Platforms.Android;
#endif

namespace CandySugar.Com.Pages.ChildViewModels.Axgles
{
    public partial class VideoViewModel : ObservableObject, IQueryAttributable
    {
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Route = (string)query["Param"];
            if ((bool)query["Is24Net"])
            {
                using var stream =  FileSystem.OpenAppPackageFileAsync("Dplayer.html");
                stream.Wait();
                using var reader = new StreamReader(stream.Result);
                Content = reader.ReadToEnd();
            }
#if ANDROID
            IBarStatus.Instance.HiddenStatusBar();
            IDirection.Instance.LockOrientation(OrientationEnum.Landscape);
#endif
            DeviceDisplay.Current.KeepScreenOn = true;
        }

        #region Property
        [ObservableProperty]
        private string _Route;
        [ObservableProperty]
        private string _Content;
        #endregion
    }
}
