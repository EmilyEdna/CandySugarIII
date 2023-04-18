using CandySugar.Com.Library.BitConvert;
using CandySugar.Com.Library.DownQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CandySugar.Com.Controls.AttachControls
{
    public class ImageAttach
    {

        static ImageAttach()
        {
            DownloadQueue.DownEventAction = new(OnDownload);
        }

        public static double GetRenderWidth(DependencyObject obj)
        {
            return (double)obj.GetValue(RenderWidthProperty);
        }
        public static void SetRenderWidth(DependencyObject obj, double value)
        {
            obj.SetValue(RenderWidthProperty, value);
        }
        public static readonly DependencyProperty RenderWidthProperty =
           DependencyProperty.RegisterAttached("RenderWidth", typeof(double), typeof(ImageAttach), new PropertyMetadata(160d));


        public static double GetRenderHeight(DependencyObject obj)
        {
            return (double)obj.GetValue(RenderHeightProperty);
        }
        public static void SetRenderHeight(DependencyObject obj, double value)
        {
            obj.SetValue(RenderHeightProperty, value);
        }
        public static readonly DependencyProperty RenderHeightProperty =
           DependencyProperty.RegisterAttached("RenderHeight", typeof(double), typeof(ImageAttach), new PropertyMetadata(240d));


        public static string GetSourceAsync(DependencyObject obj)
        {
            return (string)obj.GetValue(SoucreAysncProperty);
        }
        public static void SetSourceAsync(DependencyObject obj, string value)
        {
            obj.SetValue(SoucreAysncProperty, value);
        }
        public static readonly DependencyProperty SoucreAysncProperty =
            DependencyProperty.RegisterAttached("SourceAsync", typeof(string), typeof(ImageAttach), new PropertyMetadata(OnComplate));


        private static void OnComplate(DependencyObject sender, DependencyPropertyChangedEventArgs @event)
        {
            DownloadQueue.Init(@event.NewValue.ToString(),  (Enum)((Image)sender).Tag, (Image)sender);
        }
        private static void OnDownload(FrameworkElement element, byte[] data)
        {
            if (data == null) return;
            var images = BitmapHelper.Bytes2Image(data, (int)GetRenderWidth(element), (int)GetRenderHeight(element));
            element.Dispatcher.Invoke(() =>
            {
                ((Image)element).Source = images;
            });
        }

    }
}
