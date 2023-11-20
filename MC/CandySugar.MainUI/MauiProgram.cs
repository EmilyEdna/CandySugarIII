using CandySugar.Com.Library;
using CandySugar.Com.Service;
using CommunityToolkit.Maui;
using Sdk.Plugins;

namespace CandySugar.MainUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            SdkOption.EnableEmail = true;
            SdkOption.AcceptEmail = "1575890051@qq.com";
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement()
                .AddServiceHandler()
                .AddControlHandler()
                .AddEmailHandler()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("FontAwesome6Thin.otf", "Thin");
                });

            return builder.Build();
        }
    }
}