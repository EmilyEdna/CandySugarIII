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
}