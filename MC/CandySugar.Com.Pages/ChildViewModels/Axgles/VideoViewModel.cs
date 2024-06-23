using CommunityToolkit.Mvvm.ComponentModel;
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
#if ANDROID
            IBarStatus.Instance.HiddenStatusBar();
            IDirection.Instance.LockOrientation(OrientationEnum.Landscape);
#endif
        }

        #region Property
        [ObservableProperty]
        private string _Route;
        #endregion
    }
}
