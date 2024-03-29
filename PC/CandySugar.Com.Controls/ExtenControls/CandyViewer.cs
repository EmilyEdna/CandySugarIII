using CandySugar.Com.Library.BitConvert;
using CandySugar.Com.Library.DownPace;
using System;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using XExten.Advance.CacheFramework;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Controls.ExtenControls
{
    public class CandyViewer : UserControl
    {
        static CandyViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CandyViewer), new FrameworkPropertyMetadata(typeof(CandyViewer)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            HttpSchedule.ProcessAction = (process) =>
            {
                CalcProgress(process, 50, 12);
                if (process >= 100)
                    ImageLoader = Visibility.Visible;
            };
        }

        public string ViewSoucre
        {
            get { return (string)GetValue(ViewSoucreProperty); }
            set { SetValue(ViewSoucreProperty, value); }
        }

        public static readonly DependencyProperty ViewSoucreProperty =
            DependencyProperty.Register("ViewSoucre", typeof(string), typeof(CandyViewer), new FrameworkPropertyMetadata(OnChaged));

        public DoubleCollection ProcessValue
        {
            get { return (DoubleCollection)GetValue(ProcessValueProperty); }
            set { SetValue(ProcessValueProperty, value); }
        }

        public static readonly DependencyProperty ProcessValueProperty =
            DependencyProperty.Register("ProcessValue", typeof(DoubleCollection), typeof(CandyViewer), new PropertyMetadata(default));

        public Visibility ImageLoader
        {
            get { return (Visibility)GetValue(ImageLoaderProperty); }
            set { SetValue(ImageLoaderProperty, value); }
        }

        public static readonly DependencyProperty ImageLoaderProperty =
            DependencyProperty.Register("ImageLoader", typeof(Visibility), typeof(CandyViewer), new PropertyMetadata(Visibility.Collapsed));

        private static async void OnChaged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = (CandyViewer)d;
            var PART_IMG = (Image)uc.Template.FindName("PART_IMG", uc);
            var key = e.NewValue.ToString().ToMd5();
            var result = Caches.RunTimeCacheGet<byte[]>(key);
            if (result != null) PART_IMG.Source = BitmapHelper.Bytes2Image(result, (int)uc.Width, (int)uc.Height);
            else
            {
                var bytes = await HttpSchedule.HttpDownload(e.NewValue.ToString());
                PART_IMG.Source = BitmapHelper.Bytes2Image(result, (int)uc.Width, (int)uc.Height);
                Caches.RunTimeCacheSet(key,bytes,5);
            }
        }

        private static DoubleCollection CalcProgress(double progress, double radius, double thickness)
        {
            var r = radius - thickness / 2;
            var perimeter = 2 * Math.PI * r / thickness;
            var step = progress / 100 * perimeter;
            var result = new DoubleCollection() { step, 1000 };
            return result;
        }
    }
}
