using CandyControls;
using Org.BouncyCastle.Asn1.Cms;

namespace CandySugar.WallPaper.ViewModels
{
    public partial class Index3ViewModel : BasicObservableObject
    {

        public Index3ViewModel()
        {
            Builder = [];
            MenuData = new() { { "下载选中", "1" }, { "删除选中", "2" }, { "无声相册", "3" }, { "音乐相册", "4" } };
            Service = IocDependency.Resolve<IService<WallModel>>();
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            WindowStateEvent();
            CollectResult = new(Service.QueryAll());
            GenericDelegate.ChangeContentAction = obj => CollectResult = new(Service.QueryAll());
        }

        #region 字段
        private IService<WallModel> Service;
        private List<WallModel> Builder;
        #endregion

        #region 事件
        private void WindowStateEvent()
        {
            BorderHeight = GlobalParam.MAXHeight;
            BorderWidth = GlobalParam.MAXWidth;
            if (GlobalParam.WindowState == WindowState.Maximized)
            {
                Cols = (int)(GlobalParam.MAXWidth / 360);
            }
            else
            {
                Cols = (int)(GlobalParam.MAXWidth / 360);
                BorderWidth -= 60;
            }
        }
        #endregion

        #region 属性
        [ObservableProperty]
        private Dictionary<string, string> _MenuData;
        [ObservableProperty]
        private ObservableCollection<WallModel> _CollectResult;
        #endregion

        #region 命令 
        [RelayCommand]
        public void Check(WallModel param) => Builder.Add(param);
        [RelayCommand]
        public void UnCheck(WallModel param) => Builder.Remove(param);
        [RelayCommand]
        public void Active(string input)
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

        #region 函数
        private void Download()
        {
            if (Builder.Count > 0)
            {
                Task.Run(() =>
                {
                    Builder.ForEach(async item =>
                    {
                        var fileBytes = await new HttpClient().GetByteArrayAsync(item.Original);
                        fileBytes.FileCreate(item.PId.ToString(), FileTypes.Jpg, "WallPaper", (catalog, fileName) => new CandyNotifyControl(CommonHelper.DownloadFinishInformation, true, catalog).Show());
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
                    SyncStatic.DeleteFile(DownUtil.FilePath(item.PId.ToString(), FileTypes.Jpg, "WallPaper"));
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
                Builder.ForEach(item =>
                {
                    var fileName = DownUtil.FilePath(item.PId.ToString(), FileTypes.Jpg, "WallPaper");
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
                            new CandyNotifyControl(CommonHelper.ConvertFinishInformation, true, catalog).Show();
                        });
                    });
                }
            }
        }

        private void BuildAudio()
        {
            if (Builder.Count > 0)
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

                var RealLocal = new List<string>();
                //判断本地文件是否存在
                Builder.ForEach(item =>
                {
                    var fileName = DownUtil.FilePath(item.PId.ToString(), FileTypes.Jpg, "WallPaper");
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
                            new CandyNotifyControl(CommonHelper.ConvertFinishInformation, true, catalog).Show();
                        });
                    });
                }
            }
        }
        #endregion
    }
}
