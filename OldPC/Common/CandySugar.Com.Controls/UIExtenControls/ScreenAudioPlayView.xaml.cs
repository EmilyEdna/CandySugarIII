using CandySugar.Com.Controls.ExtenControls;
using CandySugar.Com.Library;
using CandySugar.Com.Library.Audios;
using CandySugar.Com.Library.FileWrite;
using CandySugar.Com.Library.KeepOn;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using NAudio.Wave;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using XExten.Advance.JsonDbFramework;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Controls.UIExtenControls
{
    /// <summary>
    /// ScreenAudioPlayView.xaml 的交互逻辑
    /// </summary>
    public partial class ScreenAudioPlayView : CandyWindow
    {
        private ScreenAudioPlayViewModel VM;
        private int Vol = 0;
        public ScreenAudioPlayView()
        {
            InitializeComponent();
            VM = new ScreenAudioPlayViewModel();
            this.DataContext = VM;
            ScreenKeep.PreventForCurrentThread();
            Unloaded += delegate
            {
                VM.AudioFactory.Dispose();
            };
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            ScreenKeep.RestoreForCurrentThread();
            this.Close();
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
            if (VM != null)
            {
                var Audio = VM.AudioFactory;
                if (Audio != null && Audio.WaveOutReadOnly != null)
                {
                    Audio.ChangeVolume((float)(slider.Value / 100f));
                }
            }
        }

        private void FuncHandlerEvent(object sender, System.Windows.RoutedEventArgs e)
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
                VM.SaveMp3(files);

            }
        }
    }

    public class ScreenAudioPlayViewModel : PropertyChangedBase
    {
        private JsonDbHandle<string> JsonHandler;
        private string DbPath = Path.Combine(CommonHelper.DownloadPath, "Audio", $"Audio.{FileTypes.Dat}");
        public ScreenAudioPlayViewModel()
        {
            Handle = false;
            PlayTimer = new() { Interval = 1000 };
            Setting = [new() { Width = 80, UseUnderLine = Visibility.Collapsed, Content = FontIcon.Repeat }, new() { Width = 80, UseUnderLine = Visibility.Collapsed, Content = FontIcon.Repeat1 }];
            JsonHandler = new JsonDbContext(DbPath).LoadInMemory<string>();
            CollectResult = new(JsonHandler.GetAll());
        }

        #region Field
        private ObservableCollection<string> _CollectResult;
        /// <summary>
        /// 播放列表
        /// </summary>
        public ObservableCollection<string> CollectResult
        {
            get => _CollectResult;
            set => SetAndNotify(ref _CollectResult, value);
        }
        /// <summary>
        /// 播放器
        /// </summary>
        public AudioFactory AudioFactory => AudioFactory.Instance;
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

        #region Property
        private AudioModel _AudioInfo;
        /// <summary>
        /// 音频信息
        /// </summary>
        public AudioModel AudioInfo
        {
            get => _AudioInfo;
            set => SetAndNotify(ref _AudioInfo, value);
        }
        private AudioLive _Live;
        /// <summary>
        /// 音频实时数据
        /// </summary>
        public AudioLive Live
        {
            get => _Live;
            set => SetAndNotify(ref _Live, value);
        }
        private ObservableCollection<string> _Title;
        public ObservableCollection<string> Title
        {
            get => _Title;
            set => SetAndNotify(ref _Title, value);
        }
        private ObservableCollection<CandyToggleItemSetting> _Setting;
        public ObservableCollection<CandyToggleItemSetting> Setting
        {
            get => _Setting;
            set => SetAndNotify(ref _Setting, value);
        }
        private bool _Handle;
        public bool Handle
        {
            get => _Handle;
            set => SetAndNotify(ref _Handle, value);
        }
        private string _CurrentPlay;

        public string CurrentPlay
        {
            get => _CurrentPlay;
            set => SetAndNotify(ref _CurrentPlay, value);
        }

        #endregion

        #region Command
        /// <summary>
        /// 播放选中的歌曲
        /// </summary>
        /// <param name="input"></param>
        public void CurrentPlayCommand(string input)
        {
            CurrentPlay = input;
            PlayIndex = CollectResult.ToList().IndexOf(input);
            PlayCommand();
        }
        /// <summary>
        /// 删除歌曲
        /// </summary>
        /// <param name="input"></param>
        public void TrashCommand(string input)
        {
            if (CurrentPlay == input)
            {
                new ScreenNotifyView("当前歌曲正在播放").Show();
                return;
            }
            JsonHandler.Delete(input).ExcuteDelete().SaveChange();
            CollectResult.Remove(input);
        }

        /// <summary>
        /// 切换控制
        /// </summary>
        public RelayCommand<object> PlayChangeModuleCommand => new(item =>
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
        });
        /// <summary>
        /// 播放
        /// </summary>
        public void PlayCommand()
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
        public void SkipPreviousCommand()
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
        public void SkipNextCommand()
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
        public void PauseCommand()
        {
            Handle = !Handle;
            if (AudioFactory.WaveOutReadOnly != null)
                AudioFactory.WaveOutReadOnly.Pause();
        }
        #endregion

        #region AudioMethod
        private void PlayConditions()
        {
            if (CollectResult.Count <= 0) return;
            CurrentPlay = CollectResult[PlayIndex];
            if (!File.Exists(CurrentPlay)) return;
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
            AudioFactory.InitAudio(CurrentPlay)
                .RunPlay(Info => AudioInfo = Info).InitLiveData(Info =>
                {
                    Application.Current.Dispatcher.Invoke(() => Live = Info);
                }, 64);
        }

        public void SaveMp3(List<string> input)
        {
            JsonHandler.Insert(input).ExuteInsert().SaveChange();
            CollectResult = new ObservableCollection<string>(JsonHandler.GetAll());
        }
        #endregion

        #region Event
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
