using CommunityToolkit.Mvvm.ComponentModel;
#if ANDROID
using XExten.Advance.Maui.Bar;
using XExten.Advance.Maui.Direction;
using XExten.Advance.Maui.Direction.Platforms.Android;
#endif

namespace CandySugar.Com.Pages.ChildViewModels.Rifans
{
    public partial class WatchViewModel : ObservableObject, IQueryAttributable
    {
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            Route = query["Route"].ToString();
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
