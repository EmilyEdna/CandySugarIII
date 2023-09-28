using CandySugar.Com.Library;
using CandySugar.Com.Service;
using CommunityToolkit.Maui;

namespace CandySugar.MainUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement()
                .AddServiceHandler()
                .AddControlHandler()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("FontAwesome6Thin.otf", "Thin");
                });

            return builder.Build();
        }
    }
}