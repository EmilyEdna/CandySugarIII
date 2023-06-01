using CandySugar.Com.Library.Extends;
using XExten.Advance;
using XExten.Advance.NetFramework;
using Enums = XExten.Advance.NetFramework.Enums;

namespace CandySugar.Com.Library
{
    public static class ControlsHandler
    {
        public static MauiAppBuilder AddControlHandler(this MauiAppBuilder builder)
        {
            NetFactoryExtension.RegisterNetFramework(1, Enums.Platform.Android);
            HttpEvent.RestActionEvent = new((Client, Ex) => Ex.Message.Info());
            HttpEvent.HttpActionEvent = new((Client, Ex) => Ex.Message.Info());
            return builder;
        }
    }
}
