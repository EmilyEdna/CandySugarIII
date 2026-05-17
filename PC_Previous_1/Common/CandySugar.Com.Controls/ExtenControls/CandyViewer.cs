using CandySugar.Com.Library.BitConvert;
using CandySugar.Com.Library.DownPace;
using SkiaImageView;
using System;
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

        private SKImageView PART_IMG;
        public override void OnApplyTemplate()
        {
            PART_IMG = (SKImageView)this.Template.FindName("PART_IMG", this);
            HttpSchedule.ProcessAction = (process) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    this.ProcessValue = CalcProgress(process, 50, 12);
                    if (process >= 100)
                        Show = true;
                });
            };
        }

        public string ViewSoucre
        {
            get { return (string)GetValue(ViewSoucreProperty); }
            set { SetValue(ViewSoucreProperty, value); }
        }

        public static readonly DependencyProperty ViewSoucreProperty =
            DependencyProperty.Register("ViewSoucre", typeof(string), typeof(CandyViewer), new FrameworkPropertyMetadata(Onchanged));

        public DoubleCollection ProcessValue
        {
            get { return (DoubleCollection)GetValue(ProcessValueProperty); }
            set { SetValue(ProcessValueProperty, value); }
        }

        public static readonly DependencyProperty ProcessValueProperty =
            DependencyProperty.Register("ProcessValue", typeof(DoubleCollection), typeof(CandyViewer), new FrameworkPropertyMetadata(new DoubleCollection { 0, 0 }));

        public bool Show
        {
            get { return (bool)GetValue(ShowProperty); }
            set { SetValue(ShowProperty, value); }
        }

        public static readonly DependencyProperty ShowProperty =
            DependencyProperty.Register("Show", typeof(bool), typeof(CandyViewer), new FrameworkPropertyMetadata(false));

        private static async void Onchanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var uc = (CandyViewer)d;
            uc.Show = false;
            uc.ProcessValue = [0, 0];
            var key = e.NewValue.ToString().ToMd5();
            var result = Caches.RunTimeCacheGet<byte[]>(key);
            if (result != null)
            {
                var data = SkiaBitmapHelper.Bytes2Image(result, (int)uc.Width, (int)uc.Height);
                uc.PART_IMG.Source = data;
            }
            else
            {
                var bytes = await HttpSchedule.HttpDownload(e.NewValue.ToString());
                var data = SkiaBitmapHelper.Bytes2Image(bytes, (int)uc.Width, (int)uc.Height);
                Caches.RunTimeCacheSet(key, bytes, 5);
                uc.PART_IMG.Source = data;
            }
        }

        private static DoubleCollection CalcProgress(double progress, double radius, double thickness)
        {
            var r = radius - thickness / 2;
            var perimeter = 2 * Math.PI * r / thickness;
            var step = progress / 100 * perimeter;
            return [step, 1000];
        }
    }
}
