using CandySugar.Com.Controls.ExtenControls;
using CandySugar.Com.Library.BitConvert;
using SkiaImageView;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CandySugar.Com.Controls.AttachControls
{
    internal class ImageAttach
    {
        internal static string GetSourceAsync(DependencyObject obj)
        {
            return (string)obj.GetValue(SoucreAysncProperty);
        }
        internal static void SetSourceAsync(DependencyObject obj, string value)
        {
            obj.SetValue(SoucreAysncProperty, value);
        }
        internal static readonly DependencyProperty SoucreAysncProperty =
            DependencyProperty.RegisterAttached("SourceAsync", typeof(string), typeof(ImageAttach), new PropertyMetadata(OnComplete));


        internal static object GetBase64Soucre(DependencyObject obj)
        {
            return obj.GetValue(Base64SoucreProperty);
        }

        internal static void SetBase64Soucre(DependencyObject obj, string value) 
        {
            obj.SetValue(Base64SoucreProperty, value);
        }

        internal static readonly DependencyProperty Base64SoucreProperty =
            DependencyProperty.RegisterAttached("Base64Soucre", typeof(object), typeof(ImageAttach), new PropertyMetadata(OnStreamComplete));

        private static void OnStreamComplete(DependencyObject sender, DependencyPropertyChangedEventArgs @event)
        {
            if (@event.NewValue != null)
            {
                var base64 = Convert.FromBase64String(@event.NewValue.ToString());
                SKImageView image = (SKImageView)sender;
                CandyImage candy = (CandyImage)((SKImageView)sender).TemplatedParent;
                image.Source= SkiaBitmapHelper.Bytes2Image(base64, candy.ImageThickness.Width, candy.ImageThickness.Height);
            }
        }

        private static void OnComplete(DependencyObject sender, DependencyPropertyChangedEventArgs @event)
        {
            if (!string.IsNullOrEmpty(@event.NewValue.ToString()))
            {
                CandyImage candy = (CandyImage)((SKImageView)sender).TemplatedParent;
                SKImageView image = (SKImageView)sender;
                DownloadQueue.Init(@event.NewValue.ToString(), image, candy);
            }
        }
    }
}
