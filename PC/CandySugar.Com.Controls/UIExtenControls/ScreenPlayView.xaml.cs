using CandySugar.Com.Library;
using LibVLCSharp.Shared;
using LibVLCSharp.WPF;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
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
        private Tuple<string, string> MediaInfo;
        private VLCPlayer VlcPlayer;
        private float Rate = 1;
        private int Playing = 0;
        private bool IsOpen = false;
        private Dictionary<string, string> Local = new Dictionary<string, string>();
        public ScreenPlayView()
        {
            InitializeComponent();
            StateChanged += Window_Stated;
            this.Rates.Text = $"X{Rate}";
            InitVLC();
            RelyLocation();
        }

        public ScreenPlayView(Tuple<string, string> MediaInfo)
        {
            InitializeComponent();
            this.MediaInfo = MediaInfo;
            StateChanged += Window_Stated;
            this.Rates.Text = $"X{Rate}";
            InitVLC();
            RelyLocation();
        }

        void Window_Stated(object sender, EventArgs e)
        {
            RelyLocation();
        }

        void RelyLocation()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.Height = SystemParameters.PrimaryScreenHeight;
                this.Width = SystemParameters.PrimaryScreenWidth;
                FindAnime("BarCloseKey").Begin();
            }
            if (this.WindowState == WindowState.Normal)
            {
                this.Height = 700;
                this.Width = 1200;
                FindAnime("BarOpenKey").Begin();
            }
            PlayBar.Width = this.Width - 250 <= 0 ? 0d : this.Width - 250;
            VlcPlayer.AspectRatio = this.Width + ":" + this.Height;
        }

        Storyboard FindAnime(string Key) => (Storyboard)FindResource(Key);

        #region VLC
        void InitVLC()
        {
            Core.Initialize(Path.Combine(CommonHelper.AppPath, "vlclib"));
            VlcLibVLC = new LibVLC();
            VlcPlayer = new VLCPlayer(VlcLibVLC);
            VlcPlayer.TimeChanged += TimeChanged;
            VlcPlayer.PositionChanged += PositionChanged;
            VideoPlayer.MediaPlayer = VlcPlayer;
            VlcPlayer.AspectRatio = this.Width + ":" + this.Height;
        }

        void PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                PlayBar.Value = e.Position;
            });

        }

        void TimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                VideoTime.Text = TimeSpan.FromMilliseconds(e.Time).ToString().Substring(0, 8);
            });
        }

        void PlayHandlerEvent(object sender, RoutedEventArgs e)
        {
            if (MediaInfo == null) return;
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
                    using Media media = new Media(VlcLibVLC, new Uri(MediaInfo.Item1));
                    VideoPlayer.MediaPlayer.Play(media);
                    VedioTitle.Text = MediaInfo.Item2;
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

        #region PageControl
        void FuncHandlerEvent(object sender, RoutedEventArgs e)
        {
            var Param = ((Button)sender).CommandParameter.ToString().AsInt();
            if (Param == 1)
            {
                if (!IsOpen)
                {
                    FindAnime("VolOpenKey").Begin();
                    IsOpen = true;
                }
                else
                {
                    FindAnime("VolCloseKey").Begin();
                    IsOpen = false;
                }
            }
            if (Param == 2)
            {
                OpenFileDialog dialog = new OpenFileDialog()
                {
                    Filter = "视频|*.avi;*.mp4;*.flv;*.mkv;",
                };
                if (dialog.ShowDialog() == true)
                {
                    var FileName = Path.GetFileName(dialog.FileName);
                    if (!Local.ContainsKey(FileName))
                    {
                        Local.Add(FileName, dialog.FileName);
                        MediaInfo = Tuple.Create(dialog.FileName, FileName);
                        PlayHandlerEvent(sender, e);
                    }
                }
            }
            if (Param == 3)
            {
                
            }
        }
        void VolChangeEvent(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = (sender as Slider);
            VolumeShow.Content = (int)slider.Value + "%";
            if (VideoPlayer.MediaPlayer != null)
                VideoPlayer.MediaPlayer.Volume = (int)slider.Value;
        }
        #endregion


    }
}
