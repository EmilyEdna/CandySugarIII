using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using CandySugar.Com.Controls.StructCtonrols;
using CandySugar.Com.Library.BitConvert;
using XExten.Advance.CacheFramework.RunTimeCache;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Controls.ExtenControls
{
    public class CandyImage : Control
    {
        static CandyImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CandyImage), new FrameworkPropertyMetadata(typeof(CandyImage)));
        }

        private Path PART_LOAD;
        private Button PART_BTN;
        private Path PART_INFO;
        private Grid PART_RECT;
        private Grid PART_RECT_INFO;
        private Trigger TRIGGER;
        public override void OnApplyTemplate()
        {
            PART_LOAD = (Path)this.Template.FindName("PART_LOAD", this);
            PART_BTN = (Button)this.Template.FindName("PART_BTN", this);
            PART_RECT = (Grid)this.Template.FindName("PART_RECT", this);
            PART_RECT_INFO = (Grid)this.Template.FindName("PART_RECT_INFO", this);
            PART_BTN.Click += ClickEvent;
            PART_LOAD.Height = LoadingThickness.Height;
            PART_LOAD.Width = LoadingThickness.Width;

            TRIGGER = (Trigger)this.Template.Triggers.Where(t => t is Trigger).First();
            if (!EnableTag)
                TRIGGER.EnterActions.Clear();
            else
            {
                if (PopupBtn == Visibility.Collapsed)
                {
                    PART_RECT_INFO.RowDefinitions.Last().Height = new GridLength(0, GridUnitType.Pixel);
                    CloseAnime();
                    TRIGGER.ExitActions.Add(new BeginStoryboard
                    {
                        Storyboard = CloseStory
                    });
                }
            }
            LoadAnime();
        }

        #region Anime
        public Storyboard LoadAnimeStory;
        public Storyboard CollapsedStory;
        public Storyboard ExpendStory;
        public Storyboard CloseStory;
        private void LoadAnime()
        {
            LoadAnimeStory = new Storyboard();
            DoubleAnimationUsingKeyFrames KF = new DoubleAnimationUsingKeyFrames
            {
                RepeatBehavior = RepeatBehavior.Forever
            };
            Storyboard.SetTarget(KF, PART_LOAD);
            Storyboard.SetTargetProperty(KF, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(RotateTransform.Angle)"));
            KF.KeyFrames.Add(new EasingDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
            KF.KeyFrames.Add(new EasingDoubleKeyFrame(180, TimeSpan.FromSeconds(1)));
            KF.KeyFrames.Add(new EasingDoubleKeyFrame(360, TimeSpan.FromSeconds(2)));
            LoadAnimeStory.Children.Add(KF);
            LoadAnimeStory.Begin();
        }
        private void ExpendAnime()
        {
            ExpendStory = new Storyboard();
            DoubleAnimationUsingKeyFrames Revolve = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(Revolve, PART_INFO);
            Storyboard.SetTargetProperty(Revolve, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(RotateTransform.Angle)"));
            Revolve.KeyFrames.Add(new EasingDoubleKeyFrame(-90, TimeSpan.FromSeconds(0)));
            Revolve.KeyFrames.Add(new EasingDoubleKeyFrame(0, TimeSpan.FromSeconds(1)));
            ExpendStory.Children.Add(Revolve);
            ExpendStory.Begin();
        }
        private void CollapsedAnime()
        {
            CollapsedStory = new Storyboard();
            DoubleAnimationUsingKeyFrames Revolve = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(Revolve, PART_INFO);
            Storyboard.SetTargetProperty(Revolve, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(RotateTransform.Angle)"));
            Revolve.KeyFrames.Add(new EasingDoubleKeyFrame(0, TimeSpan.FromSeconds(0)));
            Revolve.KeyFrames.Add(new EasingDoubleKeyFrame(-90, TimeSpan.FromSeconds(1)));
            CollapsedStory.Children.Add(Revolve);
            CollapsedStory.Begin();
        }
        private void CloseAnime()
        {
            CloseStory = new Storyboard();
            DoubleAnimationUsingKeyFrames Close = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTarget(Close, PART_RECT);
            Storyboard.SetTargetProperty(Close, new PropertyPath("Height"));
            Close.KeyFrames.Add(new EasingDoubleKeyFrame(100, TimeSpan.FromSeconds(0)));
            Close.KeyFrames.Add(new EasingDoubleKeyFrame(0, TimeSpan.FromSeconds(1)));
            CloseStory.Children.Add(Close);
        }
        #endregion

        #region Dp
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(CandyImage), new PropertyMetadata(Brushes.Transparent));
        public static readonly DependencyProperty CornerRadiusProperty =
            DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(CandyImage), new PropertyMetadata(default));
        public static readonly DependencyProperty BorderEffectProperty =
            DependencyProperty.Register("BorderEffect", typeof(Effect), typeof(CandyImage), new PropertyMetadata(default));
        public static readonly DependencyProperty IsAsyncLoadProperty =
            DependencyProperty.Register("IsAsyncLoad", typeof(bool), typeof(CandyImage), new PropertyMetadata(true));
        public static readonly DependencyProperty ImageThicknessProperty =
            DependencyProperty.Register("ImageThickness", typeof(ImageThickness), typeof(CandyImage), new PropertyMetadata(new ImageThickness(160, 240)));
        public static readonly DependencyProperty LoadingThicknessProperty =
            DependencyProperty.Register("LoadingThickness", typeof(ImageThickness), typeof(CandyImage), new PropertyMetadata(new ImageThickness(25, 25)));
        public static readonly DependencyProperty PopupThicknessProperty =
            DependencyProperty.Register("PopupThickness", typeof(ImageThickness), typeof(CandyImage), new PropertyMetadata(new ImageThickness(0, 0)));
        public static readonly DependencyProperty EnableLoadingProperty =
            DependencyProperty.Register("EnableLoading", typeof(bool), typeof(CandyImage), new PropertyMetadata(true));
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(CandyImage), new PropertyMetadata(default));
        public static readonly DependencyProperty PopupTemplateProperty =
            DependencyProperty.Register("PopupTemplate", typeof(DataTemplate), typeof(CandyImage), new PropertyMetadata(default));
        public static readonly DependencyProperty SrcProperty =
            DependencyProperty.Register("Src", typeof(string), typeof(CandyImage), new PropertyMetadata(default));
        internal static readonly DependencyProperty CompleteProperty =
            DependencyProperty.Register("Complete", typeof(bool), typeof(CandyImage), new PropertyMetadata(false));
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(CandyImage), new PropertyMetadata(default));
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(CandyImage), new PropertyMetadata(default));
        public static readonly DependencyProperty PopupBtnProperty =
            DependencyProperty.Register("PopupBtn", typeof(Visibility), typeof(CandyImage), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty EnableCacheProperty =
            DependencyProperty.Register("EnableCache", typeof(bool), typeof(CandyImage), new PropertyMetadata(true));
        public static readonly DependencyProperty CacheSpanProperty =
            DependencyProperty.Register("CacheSpan", typeof(int), typeof(CandyImage), new PropertyMetadata(5));
        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register("Item", typeof(object), typeof(CandyImage), new PropertyMetadata(default));
        public static readonly DependencyProperty EnableTagProperty =
            DependencyProperty.Register("EnableTag", typeof(bool), typeof(CandyImage), new PropertyMetadata(true));
        #endregion

        #region Property
        [Description("启用信息栏")]
        public bool EnableTag
        {
            get { return (bool)GetValue(EnableTagProperty); }
            set { SetValue(EnableTagProperty, value); }
        }
        [Description("绑定的对象")]
        public object Item
        {
            get { return GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }
        [Description("缓存时常/分钟")]
        public int CacheSpan
        {
            get { return (int)GetValue(CacheSpanProperty); }
            set { SetValue(CacheSpanProperty, value); }
        }
        [Description("是否使用缓存")]
        public bool EnableCache
        {
            get { return (bool)GetValue(EnableCacheProperty); }
            set { SetValue(EnableCacheProperty, value); }
        }
        [Description("弹出层颜色")]
        public Brush Fill
        {
            get { return (Brush)GetValue(FillProperty); }
            set { SetValue(FillProperty, value); }
        }
        [Description("半角")]
        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        [Description("材质阴影")]
        public Effect BorderEffect
        {
            get { return (Effect)GetValue(BorderEffectProperty); }
            set { SetValue(BorderEffectProperty, value); }
        }
        [Description("异步加载")]
        public bool IsAsyncLoad
        {
            get { return (bool)GetValue(IsAsyncLoadProperty); }
            set { SetValue(IsAsyncLoadProperty, value); }
        }
        [Description("图片链接")]
        public string Src
        {
            get { return (string)GetValue(SrcProperty); }
            set { SetValue(SrcProperty, value); }
        }
        [Description("重绘图片的长宽")]
        public ImageThickness ImageThickness
        {
            get { return (ImageThickness)GetValue(ImageThicknessProperty); }
            set { SetValue(ImageThicknessProperty, value); }
        }
        [Description("加载图像的长宽")]
        public ImageThickness LoadingThickness
        {
            get { return (ImageThickness)GetValue(LoadingThicknessProperty); }
            set { SetValue(LoadingThicknessProperty, value); }
        }
        [Description("第二弹出层长宽")]
        public ImageThickness PopupThickness
        {
            get { return (ImageThickness)GetValue(PopupThicknessProperty); }
            set { SetValue(PopupThicknessProperty, value); }
        }
        [Description("是否显示第二弹出层")]
        public Visibility PopupBtn
        {
            get { return (Visibility)GetValue(PopupBtnProperty); }
            set { SetValue(PopupBtnProperty, value); }
        }
        [Description("是否启用图片加载等待")]
        public bool EnableLoading
        {
            get { return (bool)GetValue(EnableLoadingProperty); }
            set { SetValue(EnableLoadingProperty, value); }
        }
        [Description("信息模板")]
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }
        [Description("弹出层模板")]
        public DataTemplate PopupTemplate
        {
            get { return (DataTemplate)GetValue(PopupTemplateProperty); }
            set { SetValue(PopupTemplateProperty, value); }
        }
        [Description("是否完成加载")]
        internal bool Complete
        {
            get { return (bool)GetValue(CompleteProperty); }
            set { SetValue(CompleteProperty, value); }
        }
        [Description("命令")]
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        [Description("命令参数")]
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }
        #endregion

        #region Method
        private void ClickEvent(object sender, RoutedEventArgs e)
        {
            PART_INFO = (Path)((Button)sender).Template.FindName("PART_INFO", PART_BTN);
            ExpendAnime();
            Command?.Execute(CommandParameter);
            Grid panal = new Grid();
            panal.Children.Add(new Rectangle
            {
                Height = PopupThickness.Height == 0 ? this.Height : PopupThickness.Height,
                Width = PopupThickness.Width == 0 ? Application.Current.MainWindow.ActualWidth : PopupThickness.Width,
                Fill = Fill,
            });
            panal.Children.Add(new ContentPresenter
            {
                ContentTemplate = PopupTemplate,
                Content = Item
            });
            Popup popup = new Popup
            {
                Placement = PlacementMode.Bottom,
                PlacementTarget = this,
                AllowsTransparency = true,
                StaysOpen = false,
                Height = PopupThickness.Height == 0 ? this.Height : PopupThickness.Height,
                Width = PopupThickness.Width == 0 ? Application.Current.MainWindow.ActualWidth : PopupThickness.Width,
                Child = panal
            };
            popup.IsOpen = true;
            popup.Closed += delegate
            {
                CollapsedAnime();
            };
        }
        #endregion
    }

    internal static class DownloadQueue
    {
        private static Queue<Tuple<string, Image, CandyImage>> Tiggers;
        private static AutoResetEvent AutoEvent;
        static DownloadQueue()
        {
            AutoEvent = new AutoResetEvent(true);
            Tiggers = new Queue<Tuple<string, Image, CandyImage>>();
            (new Thread(new ThreadStart(DownMethod))
            {
                IsBackground = true
            }).Start();
        }

        private static async void DownMethod()
        {
            while (true)
            {
                Tuple<string, Image, CandyImage> Tigger = null;
                lock (Tiggers)
                {
                    if (Tiggers.Count > 0)
                    {
                        Tigger = Tiggers.Dequeue();
                    }
                }
                if (Tigger != null)
                {
                    await Application.Current.Dispatcher.BeginInvoke(async () =>
                    {
                        Tigger.Item3.Complete = false;
                        var Bytes = Cache(await new HttpClient().GetByteArrayAsync(Tigger.Item1), Tigger.Item1, Tigger.Item3.EnableCache, Tigger.Item3.CacheSpan);
                        Tigger.Item2.Source = BitmapHelper.Bytes2Image(Bytes, Tigger.Item3.ImageThickness.Width, Tigger.Item3.ImageThickness.Height);
                        Tigger.Item3.Complete = true;
                        Tigger.Item3.LoadAnimeStory.Stop();
                    });
                }
                if (Tiggers.Count > 0) continue;
                //阻塞线程
                AutoEvent.WaitOne();
            }
        }

        internal async static void Init(string route, Image image, CandyImage candy)
        {
            if (candy.IsAsyncLoad)
            {
                lock (Tiggers)
                {
                    Tiggers.Enqueue(Tuple.Create(route, image, candy));
                    AutoEvent.Set();
                }
            }
            else
            {
                var Bytes = Cache(await new HttpClient().GetByteArrayAsync(route), route, candy.EnableCache, candy.CacheSpan);
                await candy.Dispatcher.BeginInvoke(() =>
                {
                    image.Source = BitmapHelper.Bytes2Image(Bytes, candy.ImageThickness.Width, candy.ImageThickness.Height);
                });
                candy.Complete = true;
            }
        }

        internal static byte[] Cache(byte[] bytes, string route, bool enableCache, int cacheSpan)
        {
            if (!enableCache) return bytes;
            var result = MemoryCaches.GetCache<byte[]>(route.ToMd5());
            if (result != null)
                return result;
            else
            {
                MemoryCaches.AddCache(route.ToMd5(), bytes, cacheSpan);
                return bytes;
            }
        }
    }
}
