namespace CandySugar.Cosplay.ViewModels
{
    public partial class Index3ViewModel : BasicObservableObject
    {
        public Index3ViewModel()
        {
            Builder = [];
            Service = IocDependency.Resolve<IService<CosplayModel>>();
            MenuData = new() { { "下载选中", "1" }, { "删除选中", "2" }, { "无声相册", "3" }, { "音乐相册", "4" } };
            CollectResult = new(Service.QueryAll());
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
            GenericDelegate.ChangeContentAction = obj => CollectResult = new(Service.QueryAll());
        }

        #region 事件
        private void WindowStateEvent()
        {
            BorderHeight = GlobalParam.MAXHeight;
            BorderWidth = GlobalParam.MAXWidth;
            if (GlobalParam.WindowState == WindowState.Maximized)
            {
                Cols = (int)(GlobalParam.MAXWidth / 360);
                MarginThickness = new Thickness(0, 0, 60, 20);
            }
            else
            {
                Cols = (int)(GlobalParam.MAXWidth / 360);
                MarginThickness = new Thickness(0, 0, 60, 15);
                BorderWidth -= 60;
            }
        }
        #endregion

        #region 字段
        private IService<CosplayModel> Service;
        private List<CosplayModel> Builder;
        #endregion

        #region 属性
        [ObservableProperty]
        private Dictionary<string, string> _MenuData;
        [ObservableProperty]
        private ObservableCollection<CosplayModel> _CollectResult;
        #endregion

        #region 命令
        [RelayCommand]
        public void Check(CosplayModel input) => Builder.Add(input);

        [RelayCommand]
        public void UnCheck(CosplayModel input) => Builder.Remove(input);

        [RelayCommand]
        public void Active(object input)
        {
            var value = input.GetType().GetProperty("SelectValue").GetValue(input).AsString().AsInt();
            if (Builder.Count <= 0) return;
            if (value == 1)
                Download();
            if (value == 2)
                Remove();
            if (value == 3)
                BuildPicture();
            if (value == 4)
                BuildAudio();
        }
        #endregion

        #region 方法
      
        #endregion

        #region 函数
        private void Download()
        {
            if (Builder.Count > 0)
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
                                    new CandyNotifyControl(CommonHelper.DownloadFinishInformation, true, catalog).Show();
                            });
                        }
                    });
                });
            }
        }
        private void Remove()
        {
            if (Builder.Count > 0)
            {
                Builder.ForEach(item =>
                {
                  
                    item.Images.ForEach(node => SyncStatic.DeleteFile(DownUtil.FilePath(node.ToMd5(), FileTypes.Jpg, Path.Combine("Cosplay", ((PlatformEnum)item.Platform).AsString(), item.Title.ToMd5()))));
                    Service.Remove(item.PId);
                });
                CollectResult = new(Service.QueryAll());
                Builder.Clear();
            }
        }
        private void BuildPicture()
        {
            if (Builder.Count > 0)
            {
                var RealLocal = new List<string>();
                //判断本地文件是否存在
                Builder.ForEach(item =>
                {
                    item.Images.ForEach(node =>
                    {
                        var fileName = DownUtil.FilePath(node.ToMd5(), FileTypes.Jpg, Path.Combine("Cosplay", ((PlatformEnum)item.Platform).AsString(), item.Title.ToMd5()));
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
                        if (res) Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(CommonHelper.ConvertFinishInformation, true, catalog).Show());
                    });
                }
            }
        }
        private void BuildAudio()
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
                var RealLocal = new List<string>();
                //判断本地文件是否存在
                Builder.ForEach(item =>
                {
                    item.Images.ForEach(node =>
                    {
                        var fileName = DownUtil.FilePath(node.ToMd5(), FileTypes.Jpg, Path.Combine("Cosplay", ((PlatformEnum)item.Platform).AsString(), item.Title.ToMd5()));
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
                        if (res) Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(CommonHelper.ConvertFinishInformation, true, catalog).Show());
                    });
                }
            }
        }
        #endregion
    }
}
