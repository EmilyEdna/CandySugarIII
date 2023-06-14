using CandySugar.Com.Pages.ViewModels.RifanViewModels;
using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;

namespace CandySugar.Com.Pages.Views.RifanViews;

public partial class RifanPlay : ContentPage
{
    public RifanPlay()
    {
        InitializeComponent();
        Media.StateChanged += MediaStateChanged;

    }

    private void MediaStateChanged(object sender, MediaStateChangedEventArgs e)
    {
#if ANDROID
        var me = (sender as MediaElement);
        if (me.CurrentState == MediaElementState.Playing)
        {
            me.Aspect = Aspect.Fill;
            XExten.Advance.Maui.Direction.IDirection.Instance.LockOrientation(XExten.Advance.Maui.Direction.Platforms.Android.OrientationEnum.Landscape);
        }
        else
        {
            me.Aspect = Aspect.AspectFit;
            XExten.Advance.Maui.Direction.IDirection.Instance.LockOrientation(XExten.Advance.Maui.Direction.Platforms.Android.OrientationEnum.Portrait);
        }
#endif
    }

    private void PageUnloadEvent(object sender, EventArgs e)
    {
        (this.BindingContext as RifanPlayViewModel).GoBack();
#if ANDROID
        XExten.Advance.Maui.Direction.IDirection.Instance.LockOrientation(XExten.Advance.Maui.Direction.Platforms.Android.OrientationEnum.Portrait);
#endif
        Media.Handler?.DisconnectHandler();
    }


}