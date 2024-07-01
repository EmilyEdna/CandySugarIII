using CandySugar.Com.Controls;
using CandySugar.Com.Library;
using CandySugar.Com.Pages;
using CandySugar.Com.Service;
using CandySugar.MainUI.ViewModels;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using SkiaSharp.Views.Maui.Controls.Hosting;
using UraniumUI;
using Index = CandySugar.MainUI.Views.Index;

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
                .UseUraniumUI()
                .UseUraniumUIMaterial()
                .UseUraniumUIBlurs()
                .UseSkiaSharp()
                .UsePrism(ioc => ioc.ConfigureModuleCatalog(catalog =>
                {
                    catalog.AddModule<ViewModule>();
                    catalog.AddModule<ControlModule>();
                    catalog.AddModule<LibraryModule>();
                    catalog.AddModule<ServiceModule>();
                }).RegisterTypes(container =>
                {
                    container.RegisterGlobalNavigationObserver();
                    container.RegisterForNavigation<Index>();
                }).OnAppStart(nav => nav.CreateBuilder().AddSegment<IndexViewModel>().Navigate(HandleNavigationError)))
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("FontAwesome6Thin.otf", "Thin");
                })
                .AddControlMapping()
                .AddControlHandler();

            return builder.Build();
        }

        private static void HandleNavigationError(Exception exception)
        {
        }
    }
}