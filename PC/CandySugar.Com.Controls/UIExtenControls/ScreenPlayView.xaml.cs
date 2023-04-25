using CandySugar.Com.Library;
using LibVLCSharp.Shared;
using LibVLCSharp.WPF;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using XExten.Advance.LinqFramework;
using VLCPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace CandySugar.Com.Controls.UIExtenControls
{
    /// <summary>
    /// ScreenPlayView.xaml 的交互逻辑
    /// </summary>
    public partial class ScreenPlayView : Window
    {
        private LibVLC VlcLibVLC;
        private string MediaUri;
        private VLCPlayer VlcPlayer;
        private float Rate = 1;
        private int Playing = 0;
        public ScreenPlayView()
        {
            InitializeComponent();
            InitVideoPlayerEvent();
            StateChanged += Window_Stated;
            this.Rates.Text = $"X{Rate}";
            InitVLC();
            RelyLocation();
        }

        public ScreenPlayView(string uri)
        {
            InitializeComponent();
            InitVideoPlayerEvent();
            MediaUri = uri;
            StateChanged += Window_Stated;
            this.Rates.Text = $"X{Rate}";
            InitVLC();
            RelyLocation();
        }

        private void Window_Stated(object sender, EventArgs e)
        {
            RelyLocation();
        }
        public void RelyLocation()
        {

            if (this.WindowState == WindowState.Maximized)
            {
                this.Height = SystemParameters.PrimaryScreenHeight;
                this.Width = SystemParameters.PrimaryScreenWidth;
            }
            if (this.WindowState == WindowState.Normal)
            {
                this.Height = 700;
                this.Width = 1200;
            }
            PlayBar.Width = this.Width;
            VlcPlayer.AspectRatio = this.Width + ":" + this.Height;
        }

        #region VLC
        private void InitVLC()
        {
            Core.Initialize(Path.Combine(CommonHelper.AppPath, "vlclib"));
            VlcLibVLC = new LibVLC();
            VlcPlayer = new VLCPlayer(VlcLibVLC);
            VlcPlayer.TimeChanged += TimeChanged;
            VlcPlayer.PositionChanged += PositionChanged;
            VideoPlayer.MediaPlayer = VlcPlayer;
            VlcPlayer.AspectRatio = this.Width + ":" + this.Height;
        }

        private void PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                PlayBar.Value = e.Position;
            });

        }

        private void TimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                VideoTime.Text = TimeSpan.FromMilliseconds(e.Time).ToString().Substring(0, 8);
            });
        }

        private void PlayHandlerEvent(object sender, RoutedEventArgs e)
        {
            if (MediaUri.IsNullOrEmpty()) return;
            var Param = ((Button)sender).CommandParameter.ToString().AsInt();
            if (Param == 1)
            {
                Rate /= 2;
                var Rates = Rate <= 1f ? 1f : Rate;
                VideoPlayer.MediaPlayer.SetRate(Rates);
                this.Rates.Text = $"X{Rates}";
            }
            if (Param == 2)
            {
                if (Playing == 0)
                {
                    using Media media = new Media(VlcLibVLC, new Uri(MediaUri));
                    VideoPlayer.MediaPlayer.Play(media);
                }
                if (Playing == 1)
                {
                    Playing = 0;
                    VideoPlayer.MediaPlayer.Play();
                }
                this.Play.Visibility = Visibility.Collapsed;
                this.Pause.Visibility = Visibility.Visible;
            }
            if (Param == 3)
            {
                Playing = 1;
                VideoPlayer.MediaPlayer.Pause();
                this.Play.Visibility = Visibility.Visible;
                this.Pause.Visibility = Visibility.Collapsed;
            }
            if (Param == 4)
            {
                Rate *= 2f;
                var Rates = Rate >= 16f ? 16f : Rate;
                VideoPlayer.MediaPlayer.SetRate(Rates);
                this.Rates.Text = $"X{Rates}";
            }
        }
        #endregion



        private void InitVideoPlayerEvent()
        {
            this.VideoPlayer.MouseLeftButtonDown += (sender, @event) =>
            {
                if (VideoPlayer.MediaPlayer == null || MediaUri.IsNullOrEmpty()) return;
                if (VideoPlayer.MediaPlayer.IsPlaying)
                    VideoPlayer.MediaPlayer.Pause();
                else
                    VideoPlayer.MediaPlayer.Play();
            };
        }
    }
}
