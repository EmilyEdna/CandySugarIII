﻿using CandySugar.WallPaper.ObjectEntity;
using XExten.Advance.JsonDbFramework;

namespace CandySugar.WallPaper.ViewModels
{
    public class MainViewModel : PropertyChangedBase
    {
        private List<WallhavSearchElementResult> WallhavBuilder;
        private List<WallkonElementResult> KonachanBuilder;
        private List<string> RealLocal;
        private List<MenuInfo> Default = new List<MenuInfo> {
            new MenuInfo { Key = 3, Value = "下载选中" },
            new MenuInfo { Key = 4, Value = "删除选中" },
            new MenuInfo { Key = 5, Value = "无声相册" },
            new MenuInfo { Key = 6, Value = "音乐相册" }
        };
        public MainViewModel()
        {
            ComponentControl = Module.IocModule.Resolve<WallhavView>();
            MenuIndex = new()
            {
                new MenuInfo { Key = 1, Value = "wallhaven" },
                new MenuInfo { Key = 2, Value = "konachan" }
            };
            GenericDelegate.HandleAction = new(obj =>
            {
                //Wallhaven收藏勾选事件
                if (obj is List<WallhavSearchElementResult> wallhav)
                {
                    WallhavBuilder = wallhav;
                    if (WallhavBuilder.Count >= 1)
                    {
                        if (!MenuIndex.Any(t => t.Key == 3 || t.Key == 4 || t.Key == 5 || t.Key == 6))
                            Default.ForEach(item => MenuIndex.Add(item));
                    }
                    else
                        Default.ForEach(item => MenuIndex.Remove(item));
                }
                //Konachan收藏勾选事件
                if (obj is List<WallkonElementResult> chan)
                {
                    KonachanBuilder = chan;
                    if (KonachanBuilder.Count >= 1)
                    {
                        if (!MenuIndex.Any(t => t.Key == 3 || t.Key == 4 || t.Key == 5 || t.Key == 6))
                            Default.ForEach(item => MenuIndex.Add(item));
                    }
                    else
                        Default.ForEach(item => MenuIndex.Remove(item));
                }
            });
        }

        #region Field
        private double Width;
        private double Height;
        #endregion

        #region Property
        private Control _ComponentControl;
        public Control ComponentControl
        {
            get => _ComponentControl;
            set => SetAndNotify(ref _ComponentControl, value);
        }
        private ObservableCollection<MenuInfo> _MenuIndex;
        /// <summary>
        /// 平台菜单
        /// </summary>
        public ObservableCollection<MenuInfo> MenuIndex
        {
            get => _MenuIndex;
            set => SetAndNotify(ref _MenuIndex, value);
        }
        #endregion

        #region Command
        public void ActiveCommand(int key)
        {
            if (key == 1)
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ComponentControl = Module.IocModule.Resolve<WallhavView>();
                    NotifyScreen();
                });
            if (key == 2)
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ComponentControl = Module.IocModule.Resolve<WallchanView>();
                    NotifyScreen();
                });
            if (key == 3)
                DownSelectPicture();
            if (key == 4)
                RemoveSelectPicture();
            if (key == 5)
                BuilderVideoPicture();
            if (key == 6)
                BuilderVideoAudioPicture();
        }
        #endregion

        #region Method
        private void BuilderVideoPicture()
        {
            if (WallhavBuilder != null)
            {
                RealLocal = new List<string>();
                //判断本地文件是否存在
                WallhavBuilder.ForEach(item =>
                {
                    var fileName = DownUtil.FilePath(item.Id, FileTypes.Jpg, "WallPaper");
                    if (File.Exists(fileName)) RealLocal.Add(fileName);
                });
                //没有被删除真实存在的文件
                if (RealLocal.Count > 0)
                {
                    //异步制作MP4
                    Task.Run(async () =>
                    {
                        var catalog = Path.Combine(CommonHelper.DownloadPath, "WallPaper");
                        var res = await RealLocal.ImageToVideo(catalog);
                        if (res) Application.Current.Dispatcher.Invoke(() =>
                        {
                            new ScreenDownNofityView(CommonHelper.ConvertFinishInformation, catalog).Show();
                        });
                    });
                }
            }
            if (KonachanBuilder != null)
            {
                RealLocal = new List<string>();
                //判断本地文件是否存在
                KonachanBuilder.ForEach(item =>
                {
                    var fileName = DownUtil.FilePath(item.Id.AsString(), FileTypes.Jpg, "WallPaper");
                    if (File.Exists(fileName)) RealLocal.Add(fileName);
                });
                //没有被删除真实存在的文件
                if (RealLocal.Count > 0)
                {
                    //异步制作MP4
                    Task.Run(async () =>
                    {
                        var catalog = Path.Combine(CommonHelper.DownloadPath, "WallPaper");
                        var res = await RealLocal.ImageToVideo(catalog);
                        if (res) Application.Current.Dispatcher.Invoke(() =>
                        {
                            new ScreenDownNofityView(CommonHelper.ConvertFinishInformation, catalog).Show();
                        });
                    });
                }
            }
        }
        private void BuilderVideoAudioPicture()
        {
            string AudioName = string.Empty;
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "音频|*.mp3"
            };
            var res = dialog.ShowDialog();
            if (res == true)
                AudioName = dialog.FileName;
            if (AudioName.IsNullOrEmpty()) return;
            var Time = AudioFactory.Instance.InitAudio(AudioName).AudioReader.TotalTime.TotalSeconds.ToString("F0");
            AudioFactory.Instance.Dispose();
            if (WallhavBuilder != null)
            {
                RealLocal = new List<string>();
                //判断本地文件是否存在
                WallhavBuilder.ForEach(item =>
                {
                    var fileName = DownUtil.FilePath(item.Id, FileTypes.Jpg, "WallPaper");
                    if (File.Exists(fileName)) RealLocal.Add(fileName);
                });
                //没有被删除真实存在的文件
                if (RealLocal.Count > 0)
                {
                    //异步制作MP4
                    Task.Run(async () =>
                    {
                        var catalog = Path.Combine(CommonHelper.DownloadPath, "WallPaper");
                        var res = await RealLocal.ImageToVideo(AudioName, Time, catalog);
                        if (res) Application.Current.Dispatcher.Invoke(() =>
                        {
                            new ScreenDownNofityView(CommonHelper.ConvertFinishInformation, catalog).Show();
                        });
                    });
                }
            }
            if (KonachanBuilder != null)
            {
                RealLocal = new List<string>();
                //判断本地文件是否存在
                KonachanBuilder.ForEach(item =>
                {
                    var fileName = DownUtil.FilePath(item.Id.AsString(), FileTypes.Jpg, "WallPaper");
                    if (File.Exists(fileName)) RealLocal.Add(fileName);
                });
                //没有被删除真实存在的文件
                if (RealLocal.Count > 0)
                {
                    //异步制作MP4
                    Task.Run(async () =>
                    {
                        var catalog = Path.Combine(CommonHelper.DownloadPath, "WallPaper");
                        var res = await RealLocal.ImageToVideo(AudioName, Time, catalog);
                        if (res) Application.Current.Dispatcher.Invoke(() =>
                        {
                            new ScreenDownNofityView(CommonHelper.ConvertFinishInformation, catalog).Show();
                        });
                    });
                }
            }
        }
        private void DownSelectPicture()
        {
            if (WallhavBuilder != null && WallhavBuilder.Count > 0)
            {
                Task.Run(() =>
                {
                    WallhavBuilder.ForEach(async item =>
                    {
                        var fileBytes = await (new HttpClient().GetByteArrayAsync(item.Original));
                        fileBytes.FileCreate(item.Id, FileTypes.Jpg, "WallPaper", (catalog, fileName) =>
                        {
                            new ScreenDownNofityView(CommonHelper.DownloadFinishInformation, catalog).Show();
                        });
                    });
                });
            }
            if (KonachanBuilder != null && KonachanBuilder.Count > 0)
            {
                Task.Run(() =>
                {
                    KonachanBuilder.ForEach(async item =>
                    {
                        var fileBytes = await (new HttpClient().GetByteArrayAsync(item.OriginalPng));
                        fileBytes.FileCreate(item.Id.AsString(), FileTypes.Jpg, "WallPaper", (catalog, fileName) =>
                        {
                            new ScreenDownNofityView(CommonHelper.DownloadFinishInformation, catalog).Show();
                        });
                    });
                });
            }
        }
        private void RemoveSelectPicture()
        {
            if (WallhavBuilder != null && WallhavBuilder.Count > 0)
            {
                var JsonHandler = new JsonDbContext(Path.Combine(CommonHelper.DownloadPath, "WallPaper", $"Wallhaven.{FileTypes.Dat}"))
                    .LoadInMemory<WallhavSearchElementResult>();
                WallhavBuilder.ForEach(item =>
                {
                    SyncStatic.DeleteFile(DownUtil.FilePath(item.Id, FileTypes.Jpg, "WallPaper"));
                    if (ComponentControl.DataContext is WallhavViewModel ViewModel)
                        ViewModel.CollectResult.Remove(item);
                    JsonHandler.Delete(t => t.Id == item.Id);
                });
                var OldData=  JsonHandler.ExcuteDelete().SaveChange<WallhavSearchElementResult>();
                WeakReferenceMessenger.Default.Send(new MessageNotify());
                if (OldData.Count <= 0)
                    Default.ForEach(item => MenuIndex.Remove(item));
            }
            if (KonachanBuilder != null && KonachanBuilder.Count > 0)
            {
                var JsonHandler = new JsonDbContext(Path.Combine(CommonHelper.DownloadPath, "WallPaper", $"Konachan.{FileTypes.Dat}"))
                  .LoadInMemory<WallkonElementResult>();
                KonachanBuilder.ForEach(item =>
                {
                    SyncStatic.DeleteFile(DownUtil.FilePath(item.Id.AsString(), FileTypes.Jpg, "WallPaper"));
                    if (ComponentControl.DataContext is WallchanViewModel ViewModel)
                        ViewModel.CollectResult.Remove(item);
                    JsonHandler.Delete(t => t.Id == item.Id);
                });
                var OldData = JsonHandler.ExcuteDelete().SaveChange<WallkonElementResult>();
                WeakReferenceMessenger.Default.Send(new MessageNotify());
                if (OldData.Count <= 0)
                    Default.ForEach(item => MenuIndex.Remove(item));
            }
        }
        public void NotifyScreen(double width, double height)
        {
            this.Width = width;
            this.Height = height;
            NotifyScreen();
        }
        private void NotifyScreen()
        {
            WeakReferenceMessenger.Default.Send(new MessageNotify
            {
                ControlParam = Tuple.Create(this.Width, this.Height)
            });
        }
        #endregion
    }
}
