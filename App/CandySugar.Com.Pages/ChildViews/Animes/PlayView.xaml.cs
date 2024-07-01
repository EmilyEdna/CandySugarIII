using CandySugar.Com.Pages.ChildViewModels.Animes;
#if ANDROID
using XExten.Advance.Maui.Bar;
using XExten.Advance.Maui.Direction;
using XExten.Advance.Maui.Direction.Platforms.Android;
#endif
namespace CandySugar.Com.Pages.ChildViews.Animes;

public partial class PlayView : ContentPage
{
    public PlayView()
    {
        InitializeComponent();
        this.BindingContext = new PlayViewModel();
        this.Disappearing += delegate
        {
#if ANDROID
            IBarStatus.Instance.ShowStatusBar();
            IDirection.Instance.LockOrientation(OrientationEnum.Portrait);
#endif
        };

    }
}