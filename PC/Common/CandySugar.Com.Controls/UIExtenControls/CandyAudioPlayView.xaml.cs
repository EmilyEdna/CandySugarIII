using CandyControls;
using CandyControls.ControlsModel.Setting;
using CandySugar.Com.Controls.ExtenControls;
using CandySugar.Com.Data;
using CandySugar.Com.Data.Entity.MusicEntity;
using CandySugar.Com.Library;
using CandySugar.Com.Library.Audios;
using CandySugar.Com.Library.FileWrite;
using CandySugar.Com.Library.KeepOn;
using CandySugar.Com.Options;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using XExten.Advance.IocFramework;
using XExten.Advance.JsonDbFramework;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Controls.UIExtenControls
{
    /// <summary>
    /// CandyAudioPlayView.xaml 的交互逻辑
    /// </summary>
    public partial class CandyAudioPlayView : CandyWindow
    {
        private CandyAudioPlayViewModel ViewModel;
        public CandyAudioPlayView()
        {
            InitializeComponent();
            this.ViewModel = new CandyAudioPlayViewModel(this);
            this.DataContext = ViewModel;
            this.Closed += Window_Closed;
            ScreenKeep.PreventForCurrentThread();
            this.Background = new ImageBrush
            {
                ImageSource = new BitmapImage(new Uri("pack://application:,,,/CandySugar.Com.Style;component/Resources/MusicBackgroud.jpg"))
            };
            Unloaded += delegate
            {
                ViewModel.AudioFactory.Dispose();
            };
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            ScreenKeep.RestoreForCurrentThread();
            this.Close();
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
    }

    internal partial class CurrentPlayModel : ObservableObject
    {
        [ObservableProperty]
        private string _SongName;
        [ObservableProperty]
        private string _Address;
    }

    internal partial class CandyAudioPlayViewModel : BasicObservableObject
    {
        private JsonDbHandle<CurrentPlayModel> JsonHandler;
        private CandyAudioPlayView Views;
        private string DbPath = Path.Combine(CommonHelper.DownloadPath, "Audio", $"Audio.{FileTypes.Dat}");
        private IService<MusicModel> Service;
        public CandyAudioPlayViewModel(CandyAudioPlayView View)
        {
            Views = View;
            Handle = false;
            PlayTimer = new() { Interval = 1000 };
            Service = IocDependency.Resolve<IService<MusicModel>>();
            Setting = [new() { Width = 80, UseUnderLine = Visibility.Collapsed, Content = FontIcon.Repeat }, new() { Width = 80, UseUnderLine = Visibility.Collapsed, Content = FontIcon.Repeat1 }];
            JsonHandler = new JsonDbContext(DbPath).LoadInMemory<CurrentPlayModel>();
            var Data = Service.QueryAll().Select(t => new CurrentPlayModel
            {
                SongName = t.SongName,
                Address = DownUtil.FilePath($"[High]{t.SongId}", FileTypes.Mp3, "Music")
            }).ToList();
            Data.AddRange(JsonHandler.GetAll());
            CollectResult = new(Data);
        }

        #region 字段
        /// <summary>
        /// 播放器
        /// </summary>
        public AudioFactory AudioFactory => AudioFactory.Instance;
        /// <summary>
        /// 音量调整开关
        /// </summary>
        private int Vol = 0;
        /// <summary>
        /// 播放索引
        /// </summary>
        private int PlayIndex = 0;
        /// <summary>
        /// 1列表循环 2单曲循环
        /// </summary>
        private int PlayMoudle = 1;
        /// <summary>
        /// 歌曲切换Timer
        /// </summary>
        private Timer PlayTimer;
        #endregion

        #region 属性
        [ObservableProperty]
        private ObservableCollection<CurrentPlayModel> _CollectResult;
        [ObservableProperty]
        private ObservableCollection<CandyToggleItemSetting> _Setting;
        [ObservableProperty]
        private CurrentPlayModel _CurrentPlay;
        [ObservableProperty]
        private bool _Handle;
        [ObservableProperty]
        private AudioLive _Live;
        [ObservableProperty]
        private AudioModel _AudioInfo;
        #endregion

        #region 命令
        /// <summary>
        /// 打开音量
        /// </summary>
        [RelayCommand]
        public void Volume()
        {
            if (Vol == 0)
            {
                ((Storyboard)Views.FindResource("VolOpenKey")).Begin();
                Vol = 1;
            }
            else
            {
                ((Storyboard)Views.FindResource("VolCloseKey")).Begin();
                Vol = 0;
            }
        }
        /// <summary>
        /// 打开文件路径
        /// </summary>
        [RelayCommand]
        public void OpenFile()
        {
            OpenFolderDialog dialog = new OpenFolderDialog()
            {
                Multiselect = false
            };

            if (dialog.ShowDialog() == true)
            {
                List<string> files = new List<string>();
                Directory.GetFiles(dialog.FolderName).ForArrayEach<string>(node =>
                {
                    if (Path.GetExtension(node).ToLower().Contains("mp3"))
                        files.Add(node);
                });
                SaveMp3(files);

            }
        }
        /// <summary>
        /// 模式切换
        /// </summary>
        /// <param name="item"></param>
        [RelayCommand]
        public void PlayChangeModule(object item)
        {
            var Target = ((CandyToggleItem)item);
            var Index = Target.Tag.ToString().AsInt() + 1;
            if (this.PlayMoudle != Index)
                this.PlayMoudle = Index;
            if (AudioFactory.WaveOutReadOnly != null) //此时正在播放
            {
                if (this.PlayMoudle == 1) ListRuch();
                if (this.PlayMoudle == 2) Single();
            }
        }
        /// <summary>
        /// 播放当前
        /// </summary>
        /// <param name="input"></param>
        [RelayCommand]
        public void PlayCurrent(CurrentPlayModel input)
        {
            CurrentPlay = input;
            PlayIndex = CollectResult.ToList().IndexOf(input);
            Play();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="input"></param>
        [RelayCommand]
        public void Trash(CurrentPlayModel input)
        {
            if (CurrentPlay != null)
            {
                if (CurrentPlay.Address == input.Address)
                {
                    new CandyNotifyControl("当前歌曲正在播放").Show();
                    return;
                }
            }
            JsonHandler.Delete(input).ExcuteDelete().SaveChange();
            CollectResult.Remove(input);
        }
        /// <summary>
        /// 播放
        /// </summary>
        [RelayCommand]
        public void Play()
        {
            PlayConditions();
            if (CollectResult.Count > 0)
            {
                Handle = !Handle;
            }
        }

        /// <summary>
        /// 上一首
        /// </summary>
        [RelayCommand]
        public void SkipPrevious()
        {
            PlayIndex -= 1;
            if (PlayIndex < 0) PlayIndex = CollectResult.Count - 1;
            if (AudioFactory.WaveOutReadOnly != null)
            {
                PlayConditions();
            }
        }

        /// <summary>
        /// 下一首
        /// </summary>
        [RelayCommand]
        public void SkipNext()
        {
            PlayIndex += 1;
            if (PlayIndex > CollectResult.Count - 1) PlayIndex = 0;
            if (AudioFactory.WaveOutReadOnly != null)
            {
                PlayConditions();
            }
        }

        /// <summary>
        /// 暂停
        /// </summary>
        [RelayCommand]
        public void Pause()
        {
            Handle = !Handle;
            if (AudioFactory.WaveOutReadOnly != null)
                AudioFactory.WaveOutReadOnly.Pause();
        }
        #endregion


        #region 方法
        private void PlayConditions()
        {
            if (CollectResult.Count <= 0) return;
            CurrentPlay = CollectResult[PlayIndex];
            if (!File.Exists(CurrentPlay.Address)) return;
            if (PlayMoudle == 1)
            {
                AudioPlays();
                ListRuch();
            }
            if (PlayMoudle == 2)
            {
                AudioPlays();
                Single();
            }
        }
        /// <summary>
        /// 列表循环
        /// </summary>
        private void ListRuch()
        {
            PlayTimer.Elapsed -= SingleEvent;
            PlayTimer.Elapsed += ListRuchEvent;
            PlayTimer.Start();
        }
        /// <summary>
        /// 单曲循环
        /// </summary>
        private void Single()
        {
            PlayTimer.Elapsed -= ListRuchEvent;
            PlayTimer.Elapsed += SingleEvent;
            PlayTimer.Start();
        }
        private void AudioPlays()
        {
            if (AudioFactory.WaveOutReadOnly != null && AudioFactory.WaveOutReadOnly.PlaybackState == PlaybackState.Paused)
            {
                AudioFactory.WaveOutReadOnly.Play();
                return;
            }
            AudioFactory.InitAudio(CurrentPlay.Address)
                .RunPlay(Info => AudioInfo = Info).InitLiveData(Info =>
                {
                    Application.Current.Dispatcher.Invoke(() => Live = Info);
                }, 64);
        }

        public void SaveMp3(List<string> input)
        {
            var data = input.Select(t => new CurrentPlayModel
            {
                SongName = Path.GetFileName(t),
                Address = t
            }).ToList();

            JsonHandler.Insert(data).ExuteInsert().SaveChange();
            CollectResult = new ObservableCollection<CurrentPlayModel>(JsonHandler.GetAll());
        }
        #endregion

        #region 事件
        private void EventCommon()
        {
            CurrentPlay = CollectResult[PlayIndex];
            AudioPlays();
        }

        private void ListRuchEvent(object sender, ElapsedEventArgs e)
        {
            if (AudioFactory.WaveOutReadOnly != null && AudioFactory.WaveOutReadOnly.PlaybackState == PlaybackState.Stopped)
            {
                var PlayNum = CollectResult.Count;
                //播放完成
                if (Math.Truncate(Live.LiveSeconds) >= Math.Truncate(AudioInfo.Seconds))
                {
                    PlayIndex += 1;
                    if (PlayIndex < PlayNum) EventCommon();
                    else
                    {
                        PlayIndex = 0;
                        EventCommon();
                    }
                }
            }
        }

        private void SingleEvent(object sender, ElapsedEventArgs e)
        {
            if (AudioFactory.WaveOutReadOnly != null && AudioFactory.WaveOutReadOnly.PlaybackState == PlaybackState.Stopped)
            {
                //播放完成
                if (Math.Truncate(Live.LiveSeconds * 10) / 10 >= Math.Truncate(AudioInfo.Seconds * 10) / 10)
                    EventCommon();
            }
        }
        #endregion
    }
}
