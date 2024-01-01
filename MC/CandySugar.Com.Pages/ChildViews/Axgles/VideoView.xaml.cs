using CandySugar.Com.Pages.ChildViewModels.Axgles;
#if ANDROID
using XExten.Advance.Maui.Bar;
using XExten.Advance.Maui.Direction;
using XExten.Advance.Maui.Direction.Platforms.Android;
#endif
namespace CandySugar.Com.Pages.ChildViews.Axgles;

public partial class VideoView : ContentPage
{
    public VideoView()
    {
        InitializeComponent();
        this.BindingContext = new VideoViewModel();
        this.Disappearing += delegate
        {
#if ANDROID
            IBarStatus.Instance.ShowStatusBar();
            IDirection.Instance.LockOrientation(OrientationEnum.Portrait);
#endif
            DeviceDisplay.Current.KeepScreenOn = false;
            Player.Dispose();
        };

    }
}