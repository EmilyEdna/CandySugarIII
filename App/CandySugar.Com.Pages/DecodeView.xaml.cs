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
                    this.Name.Text = t.Result;
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
        await CandyService.AddOption(this.Name.Text);
        await MopupService.Instance.PopAllAsync();
    }
}