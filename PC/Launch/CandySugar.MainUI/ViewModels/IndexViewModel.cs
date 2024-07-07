using CandyControls;
using CandySugar.Com.Controls.ExtenControls;
using CandySugar.Com.Controls.UIExtenControls;
using CandySugar.Com.Library;
using CandySugar.Com.Library.Audios;
using CandySugar.Com.Library.DLLoader;
using CandySugar.Com.Library.Enums;
using CandySugar.Com.Library.FFMPeg;
using CandySugar.Com.Options.ComponentGeneric;
using CandySugar.MainUI.Views;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Stylet;
using StyletIoC;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using XExten.Advance.LinqFramework;

namespace CandySugar.MainUI.ViewModels
{
    public class IndexViewModel : Conductor<IScreen>
    {
        public IContainer Container;
        public IWindowManager WindowManager;
        public IndexViewModel(IContainer Container, IWindowManager WindowManager)
        {
            this.Container = Container;
            this.WindowManager = WindowManager;
            this.Title = $"甜糖V{Assembly.GetExecutingAssembly().GetName().Version}";
#if RELEASE
            //检查更新
            Modify.CandySugarModify();
#endif
        }

        protected override void OnActivate()
        {
            SearchHistory = ["1", "2"];
            CreateMenuUI();
        }


        #region 属性
        private string _Title;
        public string Title
        {
            get => _Title;
            set => SetAndNotify(ref _Title, value);
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
            var FMainItem = new CandyMenuItem
            {
                Header = "基础插件",
                CommandParameter = EHandle.None,
            };
            var SMainItem = new CandyMenuItem
            {
                Header = "会员插件",
                CommandParameter = EHandle.None,
            };
            var TMainItem = new CandyMenuItem
            {
                Header = "系统功能",
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
            Menu.Items.Add(IMainItem);
            Menu.Items.Add(FMainItem);
            Menu.Items.Add(SMainItem);
            Menu.Items.Add(TMainItem);
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
                    CandyControl = null;
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
            if (input == EMenu.Exit) Environment.Exit(0);
        });
        #endregion

        #region 方法
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
                Filter = "音频|*.mp3",
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
            new CandyNotifyControl(CommonHelper.DownloadFinishInformation,true ,catalog).Show();
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
                Filter = "图片|*.jpg;*.jpeg",
                Multiselect = true
            };
            if (dialog.ShowDialog() == true) FileName = dialog.FileNames;
            TempWindow.Close();
            if (FileName.Length <= 0) return;
            var catalog = Path.GetDirectoryName(FileName[0]);
            await FileName.ToList().ImageToVideo(catalog);
            new CandyNotifyControl(CommonHelper.DownloadFinishInformation, true, catalog).Show();
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
                Filter = "图片|*.jpg;*.jpeg;",
                Multiselect = true
            };
            if (dialog.ShowDialog() == true) ImgName = dialog.FileNames;
            if (ImgName.Length <= 0) return;
            OpenFileDialog dialog2 = new OpenFileDialog { Filter = "音频|*.mp3" };
            if (dialog2.ShowDialog() == true) AudioName = dialog2.FileName;
            TempWindow.Close();
            if (AudioName.IsNullOrEmpty()) return;
            var catalog = Path.GetDirectoryName(ImgName[0]);
            var Time = AudioFactory.Instance.InitAudio(AudioName).AudioReader.TotalTime.TotalSeconds.ToString("F0");
            await ImgName.ToList().ImageToVideo(AudioName, Time, catalog);
            new CandyNotifyControl(CommonHelper.DownloadFinishInformation, true, catalog).Show();
        }
        #endregion
    }
}
