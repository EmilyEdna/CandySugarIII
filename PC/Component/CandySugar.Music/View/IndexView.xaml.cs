using CandySugar.Com.Options.ComponentGeneric;
using NStandard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CandySugar.Music.View
{
    /// <summary>
    /// IndexView.xaml 的交互逻辑
    /// </summary>
    public partial class IndexView : UserControl
    {
        private int ActiveAnime = 1;
        private IndexViewModel ViewModel;
        private Storyboard AnimeX1;
        private Storyboard AnimeX2;
        private Storyboard AnimeX3;
        private int Vol = 0;
        public IndexView()
        {
            InitializeComponent();
            Icon.Content = FontIcon.AnglesLeft;
            AnimeX1 = (Storyboard)FindResource("SingleSongAnimeKey");
            AnimeX2 = (Storyboard)FindResource("SongListAnimeKey");
            AnimeX3 = (Storyboard)FindResource("CollectListAnimeKey");
            AnimeX1.Completed += AnimeEvent;
            AnimeX2.Completed += AnimeEvent;
            AnimeX3.Completed += AnimeEvent;
            Loaded += delegate { ViewModel = (IndexViewModel)this.DataContext; };
            GenericDelegate.InformationAction = new((width, height) =>
            {
                Canvas.SetTop(FloatBtn, height - 160);
                Canvas.SetLeft(FloatBtn, width - 100);
                Canvas.SetTop(InformationBar, height - 158);
                Canvas.SetTop(VolSetting, height - 305);
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

        private void AnimeEvent(object sender, EventArgs e)
        {
            ViewModel.ChangeCommand(ActiveAnime);
        }

        private void PopMenuEvent(object sender, RoutedEventArgs e)
        {
            PopMenu.Opened += delegate { ((Storyboard)FindResource("Overly")).Begin(); };
            PopMenu.IsOpen = true;
        }

        private void MouseUpChanged(object sender, MouseButtonEventArgs e)
        {
            var ListItem = (sender as ListBoxItem);
            var CK = ListItem.Tag.ToString().AsInt();
            if (CK == 1 && CK != ActiveAnime) Animetion(CK).Begin();
            if (CK == 2 && CK != ActiveAnime) Animetion(CK).Begin();
            if (CK == 3 && CK != ActiveAnime) Animetion(CK).Begin();
        }

        private Storyboard Animetion(int active)
        {
            ActiveAnime = active;
            if (active == 1) return AnimeX1;
            else if (active == 2) return AnimeX2;
            else return AnimeX3;
        }

        private void SilderEvent(object sender, RoutedEventArgs e)
        {
            Icon.IsEnabled = false;
            if (ViewModel.SliderStatus == 2) CreateOpenDyamicAmime();
            else CreateCloseDyamicAmime();
        }

        private void VolumeEvent(object sender, RoutedEventArgs e)
        {
            if (Vol == 0)
            {
                ((Storyboard)FindResource("VolOpenKey")).Begin();
                Vol = 1;
            }
            else
            {
                ((Storyboard)FindResource("VolCloseKey")).Begin();
                Vol = 0;
            }
        }

        private void VolChangeEvent(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = (sender as Slider);
            VolumeShow.Content = (int)slider.Value + "%";
            if (ViewModel != null)
            {
                var Audio = ViewModel.AudioFactory;
                if (Audio != null && Audio.WaveOutReadOnly != null)
                {
                    Audio.ChangeVolume((float)(slider.Value / 100f));
                }
            }
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
