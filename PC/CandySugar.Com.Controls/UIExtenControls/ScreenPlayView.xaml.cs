using CandySugar.Com.Controls.ExtenControls;
using CandySugar.Com.Library;
using CandySugar.Com.Library.FileWrite;
using CandySugar.Com.Library.KeepOn;
using CandySugar.Com.Library.VisualTree;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using Microsoft.Win32;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using XExten.Advance.JsonDbFramework;
using XExten.Advance.LinqFramework;
using XExten.Advance.NetFramework;
using VLCPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace CandySugar.Com.Controls.UIExtenControls
{
    /// <summary>
    /// ScreenPlayView.xaml 的交互逻辑
    /// </summary>
    public partial class ScreenPlayView : CandyWindow
    {
        private LibVLC VlcLibVLC;
        private Tuple<string, string> MediaInfo;
        private VLCPlayer VlcPlayer;
        private float Rate = 1;
        private int Playing = 0;
        private bool IsOpen = false;
        private bool IsNavOpen = false;
        public int PlayModel = 1;
        private ScreenPlayViewModel Vm;
        private bool IsAx = false;
        private string AxReferrer = "https://avgle.com/";
        public ScreenPlayView()
        {
            InitializeComponent();
            Init();
        }

        public ScreenPlayView(Tuple<string, string> MediaInfo, bool IsAx = false)
        {
            this.IsAx = IsAx;
            this.MediaInfo = MediaInfo;
            InitializeComponent();
            Init();
            Vm.SetHistory(MediaInfo.Item2, MediaInfo.Item1);
        }
        void Init()
        {
            Vm = this.DataContext as ScreenPlayViewModel;
            ScreenKeep.PreventForCurrentThread();
            this.Rates.Text = $"X{Rate}";
            InitVLC();
            RelyLocation();
        }

        void Window_Closed(object sender, EventArgs e)
        {
            this.VideoPlayer.MediaPlayer.Dispose();
            ScreenKeep.RestoreForCurrentThread();
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
                FindAnime("NavListBarCloseKey").Begin();
                IsNavOpen = false;
            }
            if (this.WindowState == WindowState.Normal)
            {
                this.Height = 700;
                this.Width = 1200;
                FindAnime("BarOpenKey").Begin();
            }
            NavListBar.Height = this.Height - 160 < 0 ? 0 : this.Height - 160;
            PlayBar.Width = this.Width - 250 <= 0 ? 0d : this.Width - 250;
            VlcPlayer.AspectRatio = this.Width + ":" + this.Height;
        }

        Storyboard FindAnime(string Key) => (Storyboard)FindResource(Key);

        #region PageEvent
        private void HistoryPlayEvent(object sender, RoutedEventArgs e)
        {
            var His = (sender as Button).CommandParameter as History;
            MediaInfo = Tuple.Create(His.Value, His.Key);
            using Media media = new Media(VlcLibVLC, new Uri(His.Value));
            VideoPlayer.MediaPlayer.Play(media);
            VedioTitle.Text = His.Key;
        }

        private void MouseUpChanged(object sender, MouseButtonEventArgs e)
        {
            var ListItem = sender as ListBoxItem;
            var CK = ListItem.Tag.ToString().AsInt();
        }

        private void PositionChanged(object sender, MouseButtonEventArgs e)
        {
            if (VideoPlayer.MediaPlayer.IsPlaying)
            {
                VideoPlayer.MediaPlayer.Position = (float)PlayBar.Value;
            }
        }
        #endregion

        #region VLC
        void InitVLC()
        {
            Core.Initialize(Path.Combine(CommonHelper.AppPath, "vlclib"));
            if (IsAx) VlcLibVLC = new LibVLC($"--http-referrer={AxReferrer}", "--file-caching=10000");
            else VlcLibVLC = new LibVLC();
            VlcPlayer = new VLCPlayer(VlcLibVLC);
            VlcPlayer.TimeChanged += TimeChanged;
            VlcPlayer.PositionChanged += PositionChanged;
            VlcPlayer.Stopped += Stopped;
            VideoPlayer.MediaPlayer = VlcPlayer;
            VlcPlayer.AspectRatio = this.Width + ":" + this.Height;
        }

        private void PlayHandle()
        {
            this.Dispatcher.Invoke(async () =>
            {
                using Media media = new Media(VlcLibVLC, new Uri(MediaInfo.Item1));
                VedioTitle.Text = MediaInfo.Item2;
                await Task.Delay(500);
                VideoPlayer.MediaPlayer.Play(media);
            });
        }

        private void Stopped(object sender, EventArgs e)
        {
            if (MediaInfo == null) return;
            if (PlayModel == 1) PlayHandle();
            if (PlayModel == 2)
            {
                var LastIndex = Vm.History.Count - 1;
                var CurrentIndex = Vm.History.ToList().FindIndex(t => t.Key == MediaInfo.Item2 && t.Value == MediaInfo.Item1);
                //判断是否为最后一个
                if (CurrentIndex + 1 > LastIndex) return;
                else
                {
                    var His = Vm.History[CurrentIndex + 1];
                    MediaInfo = Tuple.Create(His.Value, His.Key);
                    PlayHandle();
                }
            }
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
                Rate = Rate <= 1f ? 1f : Rate;
                VideoPlayer.MediaPlayer.SetRate(Rate);
                this.Rates.Text = $"X{Rate}";
            }
            if (Param == 2)
            {
                if (Playing == 0)
                    PlayHandle();
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
                Rate = Rate >= 4f ? 4f : Rate;
                VideoPlayer.MediaPlayer.SetRate(Rate);
                this.Rates.Text = $"X{Rate}";
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
                    Vm.SetHistory(FileName, dialog.FileName);
                    MediaInfo = Tuple.Create(dialog.FileName, FileName);
                    PlayHandlerEvent(sender, e);
                }
            }
            if (Param == 3)
            {
                if (!IsNavOpen)
                {
                    FindAnime("NavListBarOpenKey").Begin();
                    IsNavOpen = true;
                }
                else
                {
                    FindAnime("NavListBarCloseKey").Begin();
                    IsNavOpen = false;
                }
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

    public class History
    {
        public string Key { get; set; }

        public string Value { get; set; }

    }

    public class ScreenPlayViewModel : PropertyChangedBase
    {
        private JsonDbHandle<History> JsonHandler;
        private string DbPath = Path.Combine(CommonHelper.DownloadPath, "VlcPlay", $"Vlc.{FileTypes.His}");
        public ScreenPlayViewModel()
        {
            JsonHandler = new JsonDbContext(DbPath).LoadInMemory<History>();
            Setting = [new() { Width = 40, UseUnderLine = Visibility.Collapsed, Content = FontIcon.Repeat1 }, new() { Width = 40, UseUnderLine = Visibility.Collapsed, Content = FontIcon.ArrowRightArrowLeft }];
            History = new ObservableCollection<History>(JsonHandler.GetAll() ?? []);
        }

        private ObservableCollection<CandyToggleItemSetting> _Setting;
        public ObservableCollection<CandyToggleItemSetting> Setting
        {
            get => _Setting;
            set => SetAndNotify(ref _Setting, value);
        }

        private ObservableCollection<History> _History;
        public ObservableCollection<History> History
        {
            get => _History;
            set => SetAndNotify(ref _History, value);
        }

        public void SetHistory(string fileName, string route)
        {
            if (History.Any(t => t.Key == fileName)) return;
            History his = new History
            {
                Key = fileName,
                Value = route
            };
            History.Add(his);
            JsonHandler.Insert(his).ExuteInsert().SaveChange();
        }

        public RelayCommand<History> TrashCommand => new(item =>
        {
            History.Remove(item);
            JsonHandler.Delete(item).ExcuteDelete().SaveChange();
        });

        public RelayCommand<object> ModuleChangedCommand => new(item =>
        {
            var Target = ((CandyToggleItem)item);
            var Index = Target.Tag.ToString().AsInt();
            if (Target.FindParent<Window>() is ScreenPlayView Vlc)
            {
                if (Index == 0) Vlc.PlayModel = 1;
                else Vlc.PlayModel = 2;
            }
        });
    }
}
