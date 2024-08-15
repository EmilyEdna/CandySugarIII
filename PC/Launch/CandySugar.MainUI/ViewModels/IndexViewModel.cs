using CandyControls;
using CandySugar.Com.Controls.ExtenControls;
using CandySugar.Com.Controls.UIExtenControls;
using CandySugar.Com.Library;
using CandySugar.Com.Library.Audios;
using CandySugar.Com.Library.DLLoader;
using CandySugar.Com.Library.Enums;
using CandySugar.Com.Library.FFMPeg;
using CandySugar.Com.Options.ComponentGeneric;
using CandySugar.MainUI.CtrlView;
using CandySugar.MainUI.Views;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using NPOI.HPSF;
using Stylet;
using StyletIoC;
using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using XExten.Advance.LinqFramework;
using XExten.Advance.StaticFramework;
using XExten.Advance.ThreadFramework;

namespace CandySugar.MainUI.ViewModels
{
    public class IndexViewModel : Conductor<IScreen>
    {
        private IContainer Container;
        private IWindowManager WindowManager;
        private ConcurrentQueue<string> BackQueue;
        public IndexViewModel(IContainer Container, IWindowManager WindowManager)
        {
            this.Container = Container;
            this.WindowManager = WindowManager;
            this.BackQueue = new ConcurrentQueue<string>();
            this.Title = $"甜糖V{Assembly.GetExecutingAssembly().GetName().Version}";
#if RELEASE
            //检查更新
            Modify.CandySugarModify();
#endif
        }

        protected override void OnActivate()
        {
            BlurRadius = 15;
            SearchHistory = ["1", "2"];
            CandyControl = new HomeView();
            CreateMenuUI();
            GenericDelegate.ChangeContentAction = new(obj =>
            {
                var Plugin = AssemblyLoader.Dll.FirstOrDefault(t => t.Handle == 2);
                this.View.Dispatcher.Invoke(() =>
                {
                    var Ctrl = (Control)Activator.CreateInstance(Plugin.InstanceType);
                    Ctrl.DataContext = Activator.CreateInstance(Plugin.InstanceViewModel, [obj]);
                    CandyControl = Ctrl;
                });
            });
        }


        #region 属性
        private string _Title;
        public string Title
        {
            get => _Title;
            set => SetAndNotify(ref _Title, value);
        }

        private bool _ChangeBackgroud;
        public bool ChangeBackgroud
        {
            get => _ChangeBackgroud;
            set
            {
                SetAndNotify(ref _ChangeBackgroud, value);
                ChangeBackgroudTask(value);
            }
        }

        private double _BlurRadius;
        public double BlurRadius
        {
            get => _BlurRadius;
            set => SetAndNotify(ref _BlurRadius, value);
        }
        private Control _Menus;
        public Control Mnues
        {
            get => _Menus;
            set => SetAndNotify(ref _Menus, value);
        }

        private ObservableCollection<string> _SearchHistory;
        public ObservableCollection<string> SearchHistory
        {
            get => _SearchHistory;
            set => SetAndNotify(ref _SearchHistory, value);
        }

        private Control _CandyControl;
        public Control CandyControl
        {
            get => _CandyControl;
            set => SetAndNotify(ref _CandyControl, value);
        }

        #endregion

        #region UI
        private void CreateMenuUI()
        {
            var Menu = new CandyMenu();
            Menu.SetResourceReference(CandyMenu.FontFamilyProperty, "FontStyle");
            var IMainItem = new CandyMenuItem
            {
                Header = "首页",
                CommandParameter = EHandle.Index,
            };
            IMainItem.SetBinding(CandyMenuItem.CommandProperty, new Binding()
            {
                Path = new PropertyPath("ActiveCommad", EHandle.Index),
                Source = ((IndexView)View).DataContext
            });
            Menu.Items.Add(IMainItem);
            //基础插件
            if (ComponentBinding.ComponentObjectModelGroups.Normal != null)
            {
                var FMainItem = new CandyMenuItem
                {
                    Header = "基础插件",
                    CommandParameter = EHandle.None,
                };
                ComponentBinding.ComponentObjectModelGroups.Normal.ForEach(item =>
                {
                    var SubItem = new CandyMenuItem { Header = item.Description, CommandParameter = (EHandle)item.Code };
                    SubItem.SetBinding(CandyMenuItem.CommandProperty, new Binding()
                    {
                        Path = new PropertyPath("ActiveCommad", (EHandle)item.Code),
                        Source = ((IndexView)View).DataContext
                    });
                    FMainItem.Items.Add(SubItem);
                });
                Menu.Items.Add(FMainItem);
            }
            //会员插件
            if (ComponentBinding.ComponentObjectModelGroups.Vip != null)
            {
                var SMainItem = new CandyMenuItem
                {
                    Header = "会员插件",
                    CommandParameter = EHandle.None,
                };
                ComponentBinding.ComponentObjectModelGroups.Vip.ForEach(item =>
                {
                    var SubItem = new CandyMenuItem { Header = item.Description, CommandParameter = (EHandle)item.Code };
                    SubItem.SetBinding(CandyMenuItem.CommandProperty, new Binding()
                    {
                        Path = new PropertyPath("ActiveCommad", (EHandle)item.Code),
                        Source = ((IndexView)View).DataContext
                    });
                    SMainItem.Items.Add(SubItem);
                });
                Menu.Items.Add(SMainItem);
            }
            //系统功能
            if (ComponentBinding.FunctionObjectModels != null)
            {
                var TMainItem = new CandyMenuItem
                {
                    Header = "系统功能",
                    CommandParameter = EHandle.None,
                };
                ComponentBinding.FunctionObjectModels.ForEach(item =>
                {
                    var SubItem = new CandyMenuItem { Header = item.Description, CommandParameter = (EHandle)item.Code };
                    SubItem.SetBinding(CandyMenuItem.CommandProperty, new Binding()
                    {
                        Path = new PropertyPath("ActiveCommad", (EHandle)item.Code),
                        Source = ((IndexView)View).DataContext
                    });
                    TMainItem.Items.Add(SubItem);
                });

                Menu.Items.Add(TMainItem);
            }
            Mnues = Menu;
        }
        #endregion

        #region 命令
        public RelayCommand<EHandle> ActiveCommad => new(obj =>
        {
            if (obj < EHandle.Setting)
            {
                var Plugin = AssemblyLoader.Dll.FirstOrDefault(t => t.Handle == (int)obj);
                this.View.Dispatcher.Invoke(() =>
                {
                    var Ctrl = (Control)Activator.CreateInstance(Plugin.InstanceType);
                    Ctrl.DataContext = Activator.CreateInstance(Plugin.InstanceViewModel);
                    CandyControl = Ctrl;
                });
            }
            else
            {
                if (obj == EHandle.Index)
                    CandyControl = new HomeView();
                if (obj == EHandle.Video)
                    new CandyVlcPlayView().Show();
                if (obj == EHandle.Audio)
                    new CandyAudioPlayView().Show();
                if (obj == EHandle.Setting)
                    WindowManager.ShowWindow(Container.Get<OptionViewModel>());
            }

        });

        public RelayCommand<string> SearchActiveCommand => new(obj =>
        {
            GenericDelegate.SearchAction?.Invoke(obj);
        });

        public RelayCommand<EMenu> TaskBarCommand => new(input =>
        {
            if (input == EMenu.AudioToHigh) Application.Current.Dispatcher.Invoke(AudioToHighAudio);
            if (input == EMenu.ImgToVideo) Application.Current.Dispatcher.Invoke(ImageToVideo);
            if (input == EMenu.ImgToAudio) Application.Current.Dispatcher.Invoke(ImageToAudioVideo);
            if (input == EMenu.AudioAndVideo) Application.Current.Dispatcher.Invoke(AudioAndVideoMerge);
            if (input == EMenu.Exit) Environment.Exit(0);
        });

        #endregion

        #region 方法

        private void ChangeBackgroudTask(bool CanChangeBackgroud)
        {
            if (ComponentBinding.OptionObjectModels.BackgroudLocation.IsNullOrEmpty()) return;
            var files = Directory.GetFiles(ComponentBinding.OptionObjectModels.BackgroudLocation);
            if (files.Length <= 0) return;
            if (BackQueue.IsEmpty) files.ForArrayEach<string>(BackQueue.Enqueue);
            if (CanChangeBackgroud)
                ThreadFactory.Instance.StartWithRestart(() =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var Views = (IndexView)View;
                        BackQueue.TryDequeue(out string file);
                        Storyboard Board = new Storyboard();
                        var Anime = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(3));
                        Storyboard.SetTarget(Anime, Views);
                        Storyboard.SetTargetProperty(Anime, new PropertyPath(IndexView.OpacityProperty));
                        Board.Children.Add(Anime);
                        Board.Completed += delegate
                        {
                            Views.Background = new ImageBrush(new BitmapImage(new Uri(file)));
                            Views.BeginAnimation(Grid.OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromSeconds(3)));
                            BackQueue.Enqueue(file);
                        };
                        Board.Begin();
                    });
                    Thread.Sleep((int)ComponentBinding.OptionObjectModels.Interval * 1000);
                }, "BackQuery", null, false);
            else
                ThreadFactory.Instance.StopTask("BackQuery");
        }
        /// <summary>
        /// 临时窗口
        /// </summary>
        /// <returns></returns>
        private Window CreateTempWindow()
        {
            Window TempWindow = new Window
            {
                Background = Brushes.Transparent,
                WindowStyle = WindowStyle.None,
                WindowState = WindowState.Maximized,
                Visibility = Visibility.Hidden
            };
            TempWindow.Show();
            return TempWindow;
        }
        /// <summary>
        /// 音频转高音质
        /// </summary>
        private async void AudioToHighAudio()
        {
            string[] FileName = { };
            var TempWindow = CreateTempWindow();
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "音频|*.mp3;*.wav;*.flac",
                Multiselect = true
            };
            if (dialog.ShowDialog() == true) FileName = dialog.FileNames;
            TempWindow.Close();
            if (FileName.Length <= 0) return;
            var catalog = Path.GetDirectoryName(FileName[0]);
            for (int Index = 0; Index < FileName.Length; Index++)
            {
                await Path.GetFileName(FileName[Index]).Mp3ToHighMP3(catalog);
            }
            new CandyNotifyControl(CommonHelper.ConvertFinishInformation, true, catalog).Show();
        }
        /// <summary>
        /// 图片转视频
        /// </summary>
        private async void ImageToVideo()
        {
            string[] FileName = { };
            var TempWindow = CreateTempWindow();
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "图片|*.jpg;*.jpeg;*.png;*.bmp",
                Multiselect = true
            };
            if (dialog.ShowDialog() == true) FileName = dialog.FileNames;
            TempWindow.Close();
            if (FileName.Length <= 0) return;
            var catalog = Path.GetDirectoryName(FileName[0]);
            await FileName.ToList().ImageToVideo(catalog);
            new CandyNotifyControl(CommonHelper.ConvertFinishInformation, true, catalog).Show();
        }
        /// <summary>
        /// 图片转视频带音频
        /// </summary>
        private async void ImageToAudioVideo()
        {
            string[] ImgName = { };
            string AudioName = string.Empty;
            var TempWindow = CreateTempWindow();
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "图片|*.jpg;*.jpeg;*.png;*.bmp",
                Multiselect = true
            };
            if (dialog.ShowDialog() == true) ImgName = dialog.FileNames;
            if (ImgName.Length <= 0) return;
            OpenFileDialog dialog2 = new OpenFileDialog { Filter = "音频|*.mp3;*.wav;*.flac" };
            if (dialog2.ShowDialog() == true) AudioName = dialog2.FileName;
            TempWindow.Close();
            if (AudioName.IsNullOrEmpty()) return;
            var catalog = Path.GetDirectoryName(ImgName[0]);
            var Time = AudioFactory.Instance.InitAudio(AudioName).AudioReader.TotalTime.TotalSeconds.ToString("F0");
            await ImgName.ToList().ImageToVideo(AudioName, Time, catalog);
            new CandyNotifyControl(CommonHelper.ConvertFinishInformation, true, catalog).Show();
        }

        /// <summary>
        /// 音频视频合并
        /// </summary>
        private async void AudioAndVideoMerge()
        {
            string AudioFile = string.Empty;
            string VideoFile = string.Empty;
            var TempWindow = CreateTempWindow();
            OpenFileDialog Adialog = new OpenFileDialog
            {
                Filter = "音频|*.mp3;*.wav;*.flac",
                Multiselect = false
            };
            if (Adialog.ShowDialog() == true) AudioFile = Adialog.FileName;
            OpenFileDialog Vdialog = new OpenFileDialog
            {
                Filter = "视频|*.mp4;*.avi;*.mkv;*.flv",
                Multiselect = true
            };
            if (Vdialog.ShowDialog() == true) VideoFile = Vdialog.FileName;
            TempWindow.Close();
            await Path.Combine(SyncStatic.CreateDir(CommonHelper.VideoExportPath), $"{Guid.NewGuid()}.mp4")
                 .AVMerge(AudioFile, VideoFile);

            new CandyNotifyControl(CommonHelper.ConvertFinishInformation, true, CommonHelper.VideoExportPath).Show();
        }
        #endregion
    }
}
