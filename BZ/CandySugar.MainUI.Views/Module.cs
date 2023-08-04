using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using XExten.Advance;
using XExten.Advance.LinqFramework;
using XExten.Advance.NetFramework;
using XP = XExten.Advance.NetFramework.Enums.Platform;

namespace CandySugar.MainUI.Views
{
    public static class Module
    {
        public static string HistoryUri { get; set; }
        public static Action<Exception> Handle { get; set; }
        public static XP Platforms { get; set; }
        public static IServiceCollection AddControlHandler(this IServiceCollection builder, XP platform)
        {
            Platforms = platform;
            NetFactoryExtension.RegisterNetFramework(1, XP.Android);
            HttpEvent.RestActionEvent = new((Client, Ex) => Handle?.Invoke(Ex));
            HttpEvent.HttpActionEvent = new((Client, Ex) => Handle?.Invoke(Ex));
            return builder;
        }

        public static void Back(this NavigationManager navigation)
        {
            if (HistoryUri.IsNullOrEmpty()) return;
            navigation.NavigateTo(HistoryUri);
            HistoryUri = string.Empty;
        }
    }
}
