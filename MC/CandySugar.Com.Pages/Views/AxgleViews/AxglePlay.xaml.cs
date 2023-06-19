using CandySugar.Com.Pages.ViewModels.AxgleViewModels;
using System.Numerics;

namespace CandySugar.Com.Pages.Views.AxgleViews;

public partial class AxglePlay : ContentPage
{
    public AxglePlay()
    {
        LoadAsset();
        InitializeComponent();
    }


    private async void LoadAsset()
    {
        using var stream = await FileSystem.OpenAppPackageFileAsync("Inner.html");
        using var reader = new StreamReader(stream);
        var html = await reader.ReadToEndAsync();
        HtmlWebViewSource Source = new()
        {
            Html = html
        };
        Web.Source = Source;
    }

    private async void Test(object sender, EventArgs e)
    {
        var Height = (DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density);
        var Width = (DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density);
        var ViewModel = (AxglePlayViewModel)this.BindingContext;
        await Web.EvaluateJavaScriptAsync($"Play('{ViewModel.Route}','{Width}','{Height}')");
    }
}