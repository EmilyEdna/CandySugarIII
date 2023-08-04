using CandySugar.MainUI.Views;
using Masa.Blazor;
using Microsoft.Extensions.Logging;

namespace CandySugar.MainUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.UseMauiApp<App>().Services
                .AddMauiBlazorWebView().Services
                .AddMasaBlazor().Services
                .AddControlHandler(GetPlatform());
            return builder.Build();
        }

        private static XExten.Advance.NetFramework.Enums.Platform GetPlatform() =>
            DeviceInfo.Platform.Equals(DevicePlatform.WinUI) ? XExten.Advance.NetFramework.Enums.Platform.Windows : XExten.Advance.NetFramework.Enums.Platform.Android;
    }
}