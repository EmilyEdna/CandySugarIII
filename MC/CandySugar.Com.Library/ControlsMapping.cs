using Microsoft.Maui.Handlers;

namespace CandySugar.Com.Library
{
    public static class ControlsMapping
    {
        public static MauiAppBuilder AddControlMapping(this MauiAppBuilder builder)
        {
            EntryHandler.Mapper.AppendToMapping("Entry", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Background = null;
#endif
            });
            PickerHandler.Mapper.AppendToMapping("Pick", (handler, view) => {
#if ANDROID
                handler.PlatformView.Background = null;
#endif
            });
            return builder;
        }
    }
}
