namespace CandySugar.Cosplay.ViewModels
{
    public class MainViewModel : PropertyChangedBase
    {
        private List<CosplayInitElementResult> Builder;
        private List<string> RealLocal;
        private List<MenuInfo> Default = [
            new MenuInfo { Key = 3, Value = "下载选中" },
            new MenuInfo { Key = 4, Value = "删除选中" },
            new MenuInfo { Key = 5, Value = "无声相册" },
            new MenuInfo { Key = 6, Value = "音乐相册" }
        ];

        public MainViewModel()
        {
            ComponentControl = Module.IocModule.Resolve<CosplayLabView>();
            MenuIndex = new()
            {
                new MenuInfo { Key = 1, Value = "Lab" },
                new MenuInfo { Key = 2, Value = "Land" }
            };
            GenericDelegate.HandleAction = new(obj =>
            {
                if (obj is List<CosplayInitElementResult> input)
                {
                    Builder = input;
                    if (Builder.Count >= 1)
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
                    ComponentControl = Module.IocModule.Resolve<CosplayLabView>();
                    NotifyScreen();
                });
            if (key == 2)
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ComponentControl = Module.IocModule.Resolve<CosplayLandView>();
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
            if (Builder != null)
            {
                RealLocal = new List<string>();
                //判断本地文件是否存在
                Builder.ForEach(item => {
                    var Type = item.Platform == PlatformEnum.Lab ? "Lab" : "Land";
                    item.Images.ForEach(node =>
                    {
                        var fileName = DownUtil.FilePath(node.ToMd5(), FileTypes.Jpg, Path.Combine("Cosplay", Type, item.Title.ToMd5()));
                        if (File.Exists(fileName)) RealLocal.Add(fileName);
                    });
                });
                //没有被删除真实存在的文件
                if (RealLocal.Count > 0)
                {
                    //异步制作MP4
                    Task.Run(async () =>
                    {
                        var catalog = Path.GetDirectoryName(RealLocal.First());
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
            if (Builder != null)
            {
                RealLocal = new List<string>();
                //判断本地文件是否存在
                Builder.ForEach(item => {
                    var Type = item.Platform == PlatformEnum.Lab ? "Lab" : "Land";
                    item.Images.ForEach(node =>
                    {
                        var fileName = DownUtil.FilePath(node.ToMd5(), FileTypes.Jpg, Path.Combine("Cosplay", Type, item.Title.ToMd5()));
                        if (File.Exists(fileName)) RealLocal.Add(fileName);
                    });
                });
                //没有被删除真实存在的文件
                if (RealLocal.Count > 0)
                {
                    //异步制作MP4
                    Task.Run(async () =>
                    {
                        var catalog = Path.GetDirectoryName(RealLocal.First());
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
            if (Builder != null && Builder.Count > 0)
            {
                Task.Run(() =>
                {
                    Builder.ForEach(async item =>
                    {
                        for (int index = 0; index < item.Images.Count; index++)
                        {
                            var fileBytes = await new HttpClient().GetByteArrayAsync(item.Images[index]);
                            fileBytes.FileCreate(item.Images[index].ToMd5(), FileTypes.Jpg, Path.Combine("Cosplay", item.Platform.ToString(), item.Title.ToMd5()), (catalog, fileName) =>
                            {
                                if (index == item.Images.Count - 1)
                                    new ScreenDownNofityView(CommonHelper.DownloadFinishInformation, catalog).Show();
                            });
                        }
                    });
                });
            }
        }
        private void RemoveSelectPicture()
        {
            if (Builder != null && Builder.Count > 0)
            {
                IService<CosplayModel> Servcie = IocDependency.Resolve<IService<CosplayModel>>();
                Builder.ForEach(item =>
                {
                    var Type = item.Platform == PlatformEnum.Lab ? "Lab" : "Land";
                    item.Images.ForEach(node => SyncStatic.DeleteFile(DownUtil.FilePath(node.ToMd5(), FileTypes.Jpg, Path.Combine("Cosplay", Type, item.Title.ToMd5()))));
                    if (ComponentControl.DataContext is CosplayLabViewModel CosplayLab)
                    {
                        CosplayLab.CollectResult.Remove(item);
                        Servcie.Remove(Servcie.QueryAll().First(t => t.Platform == 1 && t.Title == item.Title).PId);
                    }
                    if (ComponentControl.DataContext is CosplayLandViewModel CosplayLand)
                    {
                        CosplayLand.CollectResult.Remove(item);
                        Servcie.Remove(Servcie.QueryAll().First(t => t.Platform == 2 && t.Title == item.Title).PId);
                    }
                });
                if (Servcie.QueryAll().Count <= 0)
                    Default.ForEach(item => MenuIndex.Remove(item));
                WeakReferenceMessenger.Default.Send(new MessageNotify());
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
