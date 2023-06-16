using CommunityToolkit.Maui.Core.Primitives;
using CommunityToolkit.Maui.Views;
#if ANDROID
using CommunityToolkit.Maui.Views;
using XExten.Advance.Maui.Direction;
using XExten.Advance.Maui.Direction.Platforms.Android;
#endif

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
        var me = (sender as MediaElement);
        if (me.CurrentState == MediaElementState.Playing)
            me.Aspect = Aspect.Fill;
        else
            me.Aspect = Aspect.AspectFit;
    }

    private void PageUnloadEvent(object sender, EventArgs e)
    {
        Media.Handler?.DisconnectHandler();
    }


}