using System.Windows.Media.Animation;

namespace CandySugar.Manga.View
{
    /// <summary>
    /// IndexView.xaml 的交互逻辑
    /// </summary>
    public partial class IndexView : UserControl
    {
        private IndexViewModel ViewModel;
        public IndexView()
        {
            InitializeComponent();
            Icon.Content = FontIcon.AnglesLeft;
            Loaded += delegate { ViewModel = (IndexViewModel)this.DataContext; };
            GenericDelegate.InformationAction = new((width, height) =>
            {
                Canvas.SetTop(FloatBtn, height - 160);
                Canvas.SetLeft(FloatBtn, width - 100);
                this.Width = width;
                this.Height = height - 35 <= 0 ? 0 : height - 35;
                this.RightSider.Width = this.Width / 3;
                this.RightSider.Height = this.Height;
            });
            WeakReferenceMessenger.Default.Register<MessageNotify>(this, (recip, notify) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    if (notify.NotifyType == NotifyType.Notify)
                    {
                        if (notify.SliderStatus == 1)
                            CreateOpenDyamicAmime();
                        if (notify.SliderStatus == 2)
                            CreateCloseDyamicAmime();
                    }
                });
            });
        }

        private void PopMenuEvent(object sender, RoutedEventArgs e)
        {
            PopMenu.Opened += delegate { ((Storyboard)FindResource("Overly")).Begin(); };
            PopMenu.IsOpen = true;
        }

        private void SilderEvent(object sender, RoutedEventArgs e)
        {
            Icon.IsEnabled = false;
            if (ViewModel.SliderStatus == 2) CreateOpenDyamicAmime();
            else CreateCloseDyamicAmime();
        }

        #region DyamicAmime

        private void CreateOpenDyamicAmime()
        {
            Storyboard storyboard = new Storyboard();
            DoubleAnimationUsingKeyFrames doubleAnimations = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetName(doubleAnimations, "RightSider");
            Storyboard.SetTargetProperty(doubleAnimations, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.X)"));
            doubleAnimations.KeyFrames.Add(new EasingDoubleKeyFrame(-80, TimeSpan.Zero));
            doubleAnimations.KeyFrames.Add(new EasingDoubleKeyFrame(this.Width / (-3), TimeSpan.FromSeconds(1)));
            storyboard.Children.Add(doubleAnimations);
            storyboard.Completed += delegate
            {
                Icon.IsEnabled = true;
                ViewModel.SliderStatus = 1;
                Icon.Content = FontIcon.AnglesRight;
            };
            storyboard.Begin(this.RightSider);
        }

        private void CreateCloseDyamicAmime()
        {
            Storyboard storyboard = new Storyboard();
            DoubleAnimationUsingKeyFrames doubleAnimations = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetName(doubleAnimations, "RightSider");
            Storyboard.SetTargetProperty(doubleAnimations, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(TranslateTransform.X)"));
            doubleAnimations.KeyFrames.Add(new EasingDoubleKeyFrame(this.Width / (-3), TimeSpan.Zero));
            doubleAnimations.KeyFrames.Add(new EasingDoubleKeyFrame(-80, TimeSpan.FromSeconds(1)));
            storyboard.Children.Add(doubleAnimations);
            storyboard.Completed += delegate
            {
                Icon.IsEnabled = true;
                ViewModel.SliderStatus = 2;
                Icon.Content = FontIcon.AnglesLeft;
            };
            storyboard.Begin(this.RightSider);
        }
        #endregion
    }
}
