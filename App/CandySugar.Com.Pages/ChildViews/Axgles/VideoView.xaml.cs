using CandySugar.Com.Pages.ChildViewModels.Axgles;
using System.Threading.Tasks;
using XExten.Advance.LinqFramework;
#if ANDROID
using XExten.Advance.Maui.Bar;
using XExten.Advance.Maui.Direction;
using XExten.Advance.Maui.Direction.Platforms.Android;
#endif
namespace CandySugar.Com.Pages.ChildViews.Axgles;

public partial class VideoView : ContentPage
{
    VideoViewModel _VM;
    public VideoView()
    {
        InitializeComponent();
        _VM = new VideoViewModel();
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

    private async void LoadEvent(object sender, EventArgs e)
    {
        if (!_VM.Content.IsNullOrEmpty())
        {
            Player.Source = new HtmlWebViewSource
            {
                Html = _VM.Content
            };
            while (!this.Player.IsLoaded)
            {
                await Task.Delay(500);
            }
            if(this.Player.IsLoaded)
                await Player.EvaluateJavaScriptAsync($"Play('{_VM.Route}')");
        }
    }
}