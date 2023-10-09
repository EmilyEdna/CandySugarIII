using CandySugar.Com.Pages.ChildViewModels.Rifans;
#if ANDROID
using XExten.Advance.Maui.Bar;
using XExten.Advance.Maui.Direction;
using XExten.Advance.Maui.Direction.Platforms.Android;
#endif
namespace CandySugar.Com.Pages.ChildViews.Rifans;

public partial class WatchView : ContentPage
{
	public WatchView()
	{
		InitializeComponent();
		this.BindingContext = new WatchViewModel();
        this.Disappearing += delegate
        {
            Media.Handler?.DisconnectHandler();
#if ANDROID
            IBarStatus.Instance.ShowStatusBar();
            IDirection.Instance.LockOrientation(OrientationEnum.Portrait);
#endif
        };
    }
}