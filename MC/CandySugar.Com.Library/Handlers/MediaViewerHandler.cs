using CandySugar.Com.Library.Controls;
using Microsoft.Maui.Handlers;
using System;

namespace CandySugar.Com.Library.Handlers
{
#if ANDROID
    public partial class MediaViewerHandler
	{
        public static IPropertyMapper<MediaViewer, MediaViewerHandler> PropertyMapper = 
                                new PropertyMapper<MediaViewer, MediaViewerHandler>()
        {
        };

        public static CommandMapper<MediaViewer, MediaViewerHandler> CommandMapper = new(ViewCommandMapper)
        {
        };

        public MediaViewerHandler() : base(PropertyMapper, CommandMapper)
        {
		}
	}
#endif
}

