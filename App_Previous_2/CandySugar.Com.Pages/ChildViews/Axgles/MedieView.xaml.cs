using CandySugar.Com.Pages.ChildViewModels.Axgles;
using CommunityToolkit.Maui.Core;

#if ANDROID
using XExten.Advance.Maui.Bar;
using XExten.Advance.Maui.Direction;
using XExten.Advance.Maui.Direction.Platforms.Android;
#endif
namespace CandySugar.Com.Pages.ChildViews.Axgles;

public partial class MedieView : ContentPage
{
    MedieViewModel _VM;
    public MedieView()
    {
        InitializeComponent();
        _VM = new MedieViewModel();
        this.BindingContext = _VM;
        this.Disappearing += delegate
        {
#if ANDROID
            IBarStatus.Instance.ShowStatusBar();
            IDirection.Instance.LockOrientation(OrientationEnum.Portrait);
#endif
            DeviceDisplay.Current.KeepScreenOn = false;
        };
    }
}