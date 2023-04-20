using CandySugar.Com.Controls.UIExtenControls;
using CandySugar.Com.Library;
using CandySugar.Com.Library.Audios;
using CandySugar.Com.Library.DLLoader;
using CandySugar.Com.Library.Enums;
using CandySugar.Com.Library.FFMPeg;
using CandySugar.Com.Library.Internet;
using CandySugar.Com.Library.Threads;
using CandySugar.Com.Library.Transfers;
using CandySugar.Com.Options.ComponentGeneric;
using CandySugar.MainUI.Views;
using Microsoft.Win32;
using Stylet;
using StyletIoC;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using XExten.Advance.LinqFramework;
using XExten.Advance.ThreadFramework;

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
            Modify.CandySugarModify();
            #endif
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            var dlls = AssemblyLoader.Dll.Select(item => new ComponentMenu
            {
                InstanceType = item.InstanceType,
                Name = item.Description,
                IsEnable = item.IsEnable,
                ViewModel = item.InstanceViewModel
            });
            MenuObj = new ObservableCollection<ComponentMenu>(dlls);
            ThreadFactory.Instance.StartWithRestart(() =>
            {
                if (!InternetWork.GetNetworkState)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        new ScreenNotifyView(CommonHelper.InternetErrorInformation).Show();
                    });
                    Thread.Sleep(10000);
                }
            }, "InternetWorkCheck", true);
        }

        #region Property
        private string _Title;
        public string Title
        {
            get => _Title;
            set => SetAndNotify(ref _Title, value);
        }

        private Control _CandyControl;
        public Control CandyControl
        {
            get => _CandyControl;
            set => SetAndNotify(ref _CandyControl, value);
        }

        private ObservableCollection<ComponentMenu> _MenuObj;
        /// <summary>
        /// 组件菜单
        /// </summary>
        public ObservableCollection<ComponentMenu> MenuObj
        {
            get => _MenuObj;
            set => SetAndNotify(ref _MenuObj, value);
        }
        #endregion

        #region Command
        /// <summary>
        /// 激活组件内容
        /// </summary>
        /// <param name="InstanceType"></param>
        public void ActiveCommand(Type InstanceType)
        {
            this.View.Dispatcher.Invoke(() =>
            {
                var Ctrl = (Control)Activator.CreateInstance(InstanceType);
                var ViewModel = MenuObj.FirstOrDefault(t => t.InstanceType == InstanceType).ViewModel;
                Ctrl.DataContext = Activator.CreateInstance(ViewModel);
                CandyControl = Ctrl;
                var MainView = (IndexView)View;
                //将主窗体的长宽变动通知给子控件
                GenericDelegate.InformationAction?.Invoke(MainView.Width, MainView.Height);
            });
        }
        /// <summary>
        /// 组件检索
        /// </summary>
        /// <param name="keyword"></param>
        public void SearchCommand(string keyword)
        {
            if (this.CandyControl != null)
            {
                GenericDelegate.SearchAction?.Invoke(keyword);
            }
        }
        /// <summary>
        /// 托盘功能
        /// </summary>
        /// <param name="input"></param>
        public void SettingCommand(EMenu input)
        {
            if (input == EMenu.About) WindowManager.ShowWindow(Container.Get<AboutViewModel>());
            if (input == EMenu.AudioToHigh) Application.Current.Dispatcher.Invoke(AudioToHighAudio);
            if (input == EMenu.ImgToVideo) Application.Current.Dispatcher.Invoke(ImageToVideo);
            if (input == EMenu.ImgToAudio) Application.Current.Dispatcher.Invoke(ImageToAudioVideo);
            if (input == EMenu.SysOption) WindowManager.ShowWindow(Container.Get<OptionViewModel>());
        }
        #endregion

        #region Method
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
            new ScreenDownNofityView(CommonHelper.DownloadFinishInformation, catalog).Show();
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
            new ScreenDownNofityView(CommonHelper.DownloadFinishInformation, catalog).Show();
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
            new ScreenDownNofityView(CommonHelper.DownloadFinishInformation, catalog).Show();
        }
        #endregion
    }
}
