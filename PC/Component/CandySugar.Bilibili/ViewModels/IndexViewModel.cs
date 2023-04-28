namespace CandySugar.Bilibili.ViewModels
{
    public class IndexViewModel : PropertyChangedBase
    {

        public IndexViewModel()
        {
            Progress = SessionCode = string.Empty;
            Merge();
            ReadCookie();
        }
        private double Counts = 0;
        private string[] Special = { "|", "*", "?", ">", "<", ":", "\"", "/", "\\" };
        private string Catalog = Path.Combine(CommonHelper.DownloadPath, "Bilibili");
        private string CookiePath = Path.Combine(CommonHelper.DownloadPath, "Bilibili", "Cookie.txt");
        private string SessionCode;

        #region Property
        private string _Progress;
        public string Progress
        {
            get => _Progress;
            set => SetAndNotify(ref _Progress, value);
        }
        private string _Route;
        public string Route
        {
            get => _Route;
            set => SetAndNotify(ref _Route, value);
        }

        private BiliVideoInfoResult _InfoResult;
        /// <summary>
        /// 视频信息
        /// </summary>
        public BiliVideoInfoResult InfoResult
        {
            get => _InfoResult;
            set => SetAndNotify(ref _InfoResult, value);
        }

        private BiliVideoDataResult _DataResult;
        /// <summary>
        /// 视频数据
        /// </summary>
        public BiliVideoDataResult DataResult
        {
            get => _DataResult;
            set => SetAndNotify(ref _DataResult, value);
        }
        #endregion

        #region Command
        public void CookieCommand()
        {
            SyncStatic.CreateDir(Path.Combine(CommonHelper.DownloadPath, "Bilibili"));
            var pro = Process.Start("notepad.exe", SyncStatic.CreateFile(CookiePath));
            pro.WaitForExit();
            ReadCookie();
        }
        public void QeuryCommand()
        {
            if (Route.IsNullOrEmpty()) return;
            OnInitVideo();
        }
        public void HandleCommand(string key)
        {
            var condition = key.AsInt();
            if (condition == 1) OnDownload();
            if (condition == 2) SaveVideo();
            if (condition == 3) SaveAudio();
            if (condition == 4) SaveAMerge();
        }
        #endregion

        #region Method
        private void SaveVideo()
        {
            if (DataResult == null) return;
            Task.Run(async () =>
            {
                var res = await DataResult.VideoDash.M4Video(Path.Combine(Catalog, $"{InfoResult.BVID}.mp4"));
                if (res)
                {
                    File.Move(Path.Combine(Catalog, $"{InfoResult.BVID}.mp4"), Path.Combine(Catalog, $"{InfoResult.Title}.mp4"));
                    Application.Current.Dispatcher.Invoke(() => new ScreenDownNofityView(CommonHelper.DownloadFinishInformation, Catalog).Show());
                }
            });
        }
        private void SaveAudio()
        {
            if (DataResult == null) return;
            Task.Run(async () =>
            {
                var res = await DataResult.AudioDash.M4Audio(Path.Combine(Catalog, $"{InfoResult.BVID}.mp3"));
                if (res)
                {
                    File.Move(Path.Combine(Catalog, $"{InfoResult.BVID}.mp3"), Path.Combine(Catalog, $"{InfoResult.Title}.mp3"));
                    Application.Current.Dispatcher.Invoke(() => new ScreenDownNofityView(CommonHelper.DownloadFinishInformation, Catalog).Show());
                }
            });
        }
        private void SaveAMerge()
        {
            if (DataResult == null) return;
            Task.Run(async () =>
            {
                SyncStatic.CreateDir(Catalog);
                var data = new Dictionary<string, string> {
                        { Path.Combine(Catalog, $"mp4_{InfoResult.BVID}.m4s"),DataResult.VideoDash },
                        { Path.Combine(Catalog, $"mp3_{InfoResult.BVID}.m4s"),DataResult.AudioDash }
                    };
                await HttpSchedule.HttpDownload(data, header =>
                {
                    header.Add(ConstDefault.Referer, "https://www.bilibili.com/");
                });
            });
        }
        private void OnDownload()
        {
            Task.Run(async () =>
            {
                var bytes = await new HttpClient().GetByteArrayAsync(InfoResult.Cover);
                bytes.FileCreate(InfoResult.Title, FileTypes.Jpg, "Bilibili", (catalog, fileName) =>
                {
                    Application.Current.Dispatcher.Invoke(() => new ScreenDownNofityView(CommonHelper.DownloadFinishInformation, catalog).Show());
                });
            });
        }
        private async void OnInitData()
        {
            try
            {
                var result = (await BilibiliFactory.Bili(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        BiliType = BiliEnum.VideoData,
                        CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                        VideoData = new BiliVideoData
                        {
                            Session = SessionCode,
                            BVID = InfoResult.BVID,
                            CID = InfoResult.CID,
                        }
                    };
                }).RunsAsync()).DataResult;
                DataResult = result;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "");
                ErrorNotify();
            }
        }
        private async void OnInitVideo()
        {
            try
            {
                var result = (await BilibiliFactory.Bili(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        BiliType = BiliEnum.VideoInfo,
                        CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                        VideoInfo = new BiliVideoInfo
                        {
                            Route = Route,
                        }
                    };
                }).RunsAsync()).InfoResult;
                InfoResult = result;
                OnInitData();
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "");
                ErrorNotify();
            }
        }
        private void ReadCookie()
        {
            if (File.Exists(CookiePath))
            {
                SessionCode = File.ReadAllText(CookiePath);
            }
        }
        private void ErrorNotify()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new ScreenNotifyView(CommonHelper.ComponentErrorInformation).Show();
            });
        }
        private void Merge()
        {
            HttpSchedule.ReceiveAction = new((item, num) =>
            {
                if (item == double.Parse((100 / num).ToString("F2")))
                    Counts += item;
                this.Progress = $"{Counts+ item}%";
                if (Math.Ceiling(Counts) >= 100)
                {
                    Task.Run(async () =>
                    {
                        this.Progress = "Please Wait . . .";
                        var res = await Path.Combine(Catalog, $"{InfoResult.BVID}.mp4").M4VAMerge(Path.Combine(Catalog, $"mp3_{InfoResult.BVID}.m4s"), Path.Combine(Catalog, $"mp4_{InfoResult.BVID}.m4s"));
                        if (res)
                        {
                            this.Progress = string.Empty;
                            Application.Current.Dispatcher.Invoke(() => new ScreenDownNofityView(CommonHelper.DownloadFinishInformation, Catalog).Show());
                            await Task.Delay(1500);
                            try
                            {
                                string SaleName = string.Empty;
                                var Pattern = "[\\|/|*/|?/|</|>/|/|:/|\\\\/|//|\"]";
                                if (InfoResult.Title.Any(t => Special.Contains(t.ToString())))
                                    SaleName = Regex.Replace(InfoResult.Title, Pattern, "_");
                                else
                                    SaleName = InfoResult.Title;
                                File.Move(Path.Combine(Catalog, $"{InfoResult.BVID}.mp4"), Path.Combine(Catalog, $"{SaleName}.mp4"));
                                SyncStatic.DeleteFile(Path.Combine(Catalog, $"mp3_{InfoResult.BVID}.m4s"));
                                SyncStatic.DeleteFile(Path.Combine(Catalog, $"mp4_{InfoResult.BVID}.m4s"));
                            }
                            catch (Exception ex)
                            {
                                Log.Logger.Error(ex, "");
                            }
                        }
                    });
                }
            });
        }
        #endregion
    }
}
