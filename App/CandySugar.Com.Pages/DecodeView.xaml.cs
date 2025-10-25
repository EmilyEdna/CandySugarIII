using CandySugar.Com.Service;
using Microsoft.Maui.Platform;
using Mopups.Pages;
using Mopups.Services;
using XExten.Advance.IocFramework;

namespace CandySugar.Com.Pages;

public partial class DecodeView : PopupPage
{
    private ICandyService CandyService;
	public DecodeView()
	{
		InitializeComponent();
        CandyService = IocDependency.Resolve<ICandyService>();
        CandyService.GetOption().ContinueWith(t =>
        {
            if (t.IsCompletedSuccessfully)
            {
                this.Dispatcher.Dispatch(() =>
                {
                    var R = t.Result;
                    if (R != null)
                    {
                        this.Name.Text = R.DecodeDataKey;
                        this.Play.Text = R.DecodePlayKey;
                    }
                });
            }
        });

    }

    private async void CancelEvent(object sender, EventArgs e)
    {
        await MopupService.Instance.PopAllAsync();
    }

    private async void OkEvent(object sender, EventArgs e)
    {
        await CandyService.AddOption(this.Name.Text,this.Play.Text);
        await MopupService.Instance.PopAllAsync();
    }
}