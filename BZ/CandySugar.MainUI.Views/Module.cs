using System;
using BlazorComponent;
using Masa.Blazor;
using Masa.Blazor.Presets;
using Microsoft.Extensions.DependencyInjection;
using XExten.Advance;
using XExten.Advance.NetFramework;
using XExten.Advance.NetFramework.Enums;

namespace CandySugar.MainUI.Views
{
    public static class Module
    {
        public static Action<Exception> Handle { get; set; }
        public static Platform Platforms { get; set; }
        public static IServiceCollection AddControlHandler(this IServiceCollection builder, Platform platform)
        {
            Platforms = platform;
            NetFactoryExtension.RegisterNetFramework(1, Platform.Android);
            HttpEvent.RestActionEvent = new((Client, Ex) => Handle?.Invoke(Ex));
            HttpEvent.HttpActionEvent = new((Client, Ex) => Handle?.Invoke(Ex));
            return builder;
        }

    }
}
