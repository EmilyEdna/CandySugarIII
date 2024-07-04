using CandyControls;
using CandyControls.ControlsModel.Setting;
using CandySugar.Com.Data;
using CandySugar.Com.Data.Entity.HistoryEntity;
using CandySugar.Com.Library;
using CandySugar.Com.Library.Enums;
using CandySugar.Com.Library.KeepOn;
using CandySugar.Com.Options;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LibVLCSharp.Shared;
using Microsoft.Win32;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using XExten.Advance.IocFramework;
using XExten.Advance.LinqFramework;
using VLCPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace CandySugar.Com.Controls.UIExtenControls
{
    /// <summary>
    /// CandyVlcPlayView.xaml 的交互逻辑
    /// </summary>
    public partial class CandyVlcPlayView : CandyWindow
    {

        #region 字段
        private CandyVlcPlayViewModel ViewModel;
        //倍数
        private float _Rate = 1;
        private LibVLC _LibVLC;
        private VLCPlayer _VLCPlayer;

        private string _FileName;
        private string _Route;

        //是否正在播放
        private EPlay _Playing;
        private bool _IsVolOpen;
        //1循环 2顺序
        public int PlayModel;
        #endregion

        public CandyVlcPlayView() : base()
        {
            InitializeComponent();
            InitAll();
        }
        public CandyVlcPlayView(string FileName, string Route) : base()
        {
            this._FileName = FileName;
            this._Route = Route;
            InitializeComponent();
            InitAll();
            ViewModel.Save(FileName, Route);
        }
        private void InitAll()
        {
            ScreenKeep.PreventForCurrentThread();
            this.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/CandySugar.Com.Style;component/Resources/MusicBackgroud.jpg"))
            };
            ViewModel = new CandyVlcPlayViewModel();
            this.DataContext = ViewModel;
            this._Playing = EPlay.Stop;
            this._IsVolOpen = false;
            this.Rates.Text = $"X{_Rate}";

            //初始化VLC相关
            Core.Initialize(Path.Combine(CommonHelper.AppPath, "vlclib"));
            _LibVLC = new LibVLC();
            _VLCPlayer = new VLCPlayer(_LibVLC);
            _VLCPlayer.TimeChanged += TimeChanged;
            _VLCPlayer.PositionChanged += PositionChanged;
            _VLCPlayer.Stopped += Stopped;
            _VLCPlayer.AspectRatio = this.Width + ":" + this.Height;
            this.VideoPlayer.MediaPlayer = _VLCPlayer;
            this.PlayBar.Width = this.Width - 450;
            this.Nav.Visibility = Visibility.Collapsed;

            //初始化窗口事件
            this.Closed += delegate
            {
                this.VideoPlayer.MediaPlayer.Dispose();
                ScreenKeep.RestoreForCurrentThread();
            };
            this.StateChanged += delegate
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    this.Width = SystemParameters.FullPrimaryScreenWidth;
                    this.Height = SystemParameters.FullPrimaryScreenHeight - 80;
                    this.Nav.Height = this.Nav.MinHeight * 2.5;
                }
                else
                {
                    this.Width = this.MinWidth;
                    this.Height = this.MinHeight;
                    this.Nav.Height = 350;
                }
                _VLCPlayer.AspectRatio = this.Width + ":" + this.Height;
                PlayBar.Width = this.Width - 450;
            };
        }

        #region VLC事件
        private void Stopped(object sender, EventArgs e)
        {
            if (this._Route.IsNullOrEmpty()) return;
            if (PlayModel == 1) PlayHandle();
            if (PlayModel == 2)
            {
                var LastIndex = ViewModel.History.Count - 1;
                var CurrentIndex = ViewModel.History.ToList().FindIndex(t => t.Name == this._FileName && t.Route == this._Route);
                //判断是否为最后一个
                if (CurrentIndex + 1 > LastIndex) return;
                else
                {
                    var His = ViewModel.History[CurrentIndex + 1];
                    this._Route = His.Route;
                    this._FileName = this.Name;
                    PlayHandle();
                }
            }
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
        #endregion

        #region 公用
        private void PlayHandle()
        {
            this.Dispatcher.Invoke(async () =>
            {
                using Media media = new Media(_LibVLC, new Uri(_Route));
                VedioTitle.Text = _FileName;
                await Task.Delay(500);
                VideoPlayer.MediaPlayer.Play(media);
            });
        }
        #endregion

        #region UI控件事件
        private void PositionChanged(object sender, MouseButtonEventArgs e)
        {
            if (VideoPlayer.MediaPlayer.IsPlaying)
                VideoPlayer.MediaPlayer.Position = (float)PlayBar.Value;
        }

        private void PlayHandlerEvent(object sender, RoutedEventArgs e)
        {
            if (_Route.IsNullOrEmpty()) return;
            var Param = ((Button)sender).CommandParameter.ToString().AsInt();

            if (Param == 1) //放慢播放速度
            {
                _Rate /= 2;
                _Rate = _Rate <= 1f ? 1f : _Rate;
                VideoPlayer.MediaPlayer.SetRate(_Rate);
                this.Rates.Text = $"X{_Rate}";
            }
            else if (Param == 2)
            {
                if (_Playing == EPlay.Stop)
                    PlayHandle();
                if (_Playing == EPlay.Pause)
                    VideoPlayer.MediaPlayer.Play();
                _Playing = EPlay.Play;
                this.Play.Visibility = Visibility.Collapsed;
                this.Pause.Visibility = Visibility.Visible;
            }
            else if (Param == 3)
            {
                _Playing = EPlay.Pause;
                VideoPlayer.MediaPlayer.Pause();
                this.Play.Visibility = Visibility.Visible;
                this.Pause.Visibility = Visibility.Collapsed;
            }
            else //加快播放速度
            {
                _Rate *= 2f;
                _Rate = _Rate >= 8f ? 8f : _Rate;
                VideoPlayer.MediaPlayer.SetRate(_Rate);
                this.Rates.Text = $"X{_Rate}";
            }
        }

        private void FuncHandlerEvent(object sender, RoutedEventArgs e)
        {
            var Param = ((Button)sender).CommandParameter.ToString().AsInt();
            if (Param == 1)
            {
                if (!_IsVolOpen)
                {
                    ((Storyboard)FindResource("VolOpenKey")).Begin();
                    _IsVolOpen = true;
                }
                else
                {
                    ((Storyboard)FindResource("VolCloseKey")).Begin();
                    _IsVolOpen = false;
                }
            }
            else if (Param == 2)
            {
                OpenFileDialog dialog = new OpenFileDialog()
                {
                    Filter = "视频|*.avi;*.mp4;*.flv;*.mkv;",
                };
                if (dialog.ShowDialog() == true)
                {
                    var FileName = Path.GetFileName(dialog.FileName);
                    this._Route = dialog.FileName;
                    this._FileName = FileName;
                    ViewModel.Save(_FileName, _Route);
                    PlayHandlerEvent(sender, e);
                }
            }
            else
                Nav.Visibility = Visibility.Visible;
        }
        private void CloseNavEvent(object sender, RoutedEventArgs e)
        {
            Nav.Visibility = Visibility.Collapsed;
        }

        private void VolChangeEvent(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = (sender as Slider);
            VolumeShow.Content = (int)slider.Value + "%";
            if (VideoPlayer.MediaPlayer != null)
                VideoPlayer.MediaPlayer.Volume = (int)slider.Value;
        }

        private void HistoryPlayEvent(object sender, RoutedEventArgs e)
        {
            var His = (sender as Button).CommandParameter as HistoryModel;
            using Media media = new Media(_LibVLC, new Uri(His.Route));
            VideoPlayer.MediaPlayer.Play(media);
            VedioTitle.Text = His.Name;
        }
        #endregion
    }

    public partial class CandyVlcPlayViewModel : BasicObservableObject
    {
        private IService<HistoryModel> Service;
        public CandyVlcPlayViewModel()
        {
            Service = IocDependency.Resolve<IService<HistoryModel>>();
            Setting = [new() { Width = 40, UseUnderLine = Visibility.Collapsed, Content = FontIcon.Repeat1 }, new() { Width = 40, UseUnderLine = Visibility.Collapsed, Content = FontIcon.ArrowRightArrowLeft }];
        }

        #region 属性
        [ObservableProperty]
        private ObservableCollection<CandyToggleItemSetting> _Setting;
        [ObservableProperty]
        private ObservableCollection<HistoryModel> _History;
        #endregion

        #region  方法
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="Route"></param>
        public void Save(string Name, string Route)
        {
            Service.Insert(new HistoryModel
            {
                AddDate = DateTime.Now,
                Name = Name,
                Route = Route
            });
        }
        #endregion

        #region 命令
        [RelayCommand]
        public void Trash(HistoryModel item)
        {
            Service.Remove(item.PId);
            History = new(Service.QueryAll());
        }

        [RelayCommand]
        public void Change(object item)
        {
            var Target = ((CandyToggleItem)item);
            var Index = Target.Tag.ToString().AsInt();
            if (Target.FindParent<Window>() is CandyVlcPlayView Vlc)
            {
                if (Index == 0) Vlc.PlayModel = 1;
                else Vlc.PlayModel = 2;
            }
        }
        #endregion
    }
}
