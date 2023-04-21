namespace CandySugar.Bilibili.ViewModels
{
    public class IndexViewModel : PropertyChangedBase
    {

        public IndexViewModel()
        {
            SessionCode = string.Empty;
            ReadCookie();
        }

        private string CookiePath = Path.Combine(CommonHelper.DownloadPath, "Bilibili", "Cookie.txt");
        private string SessionCode;

        #region Property
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
                var catalog = Path.Combine(CommonHelper.DownloadPath, "Bilibili");
                var res = await DataResult.VideoDash.M4Video(Path.Combine(catalog, $"{InfoResult.BVID}.mp4"));
                if (res)
                {
                    File.Move(Path.Combine(catalog, $"{InfoResult.BVID}.mp4"), Path.Combine(catalog, $"{InfoResult.Title}.mp4"));
                    Application.Current.Dispatcher.Invoke(() => new ScreenDownNofityView(CommonHelper.DownloadFinishInformation, catalog).Show());
                }
            });
        }
        private void SaveAudio()
        {
            if (DataResult == null) return;
            Task.Run(async () =>
            {
                var catalog = Path.Combine(CommonHelper.DownloadPath, "Bilibili");
                var res = await DataResult.AudioDash.M4Audio(Path.Combine(catalog, $"{InfoResult.BVID}.mp3"));
                if (res)
                {
                    File.Move(Path.Combine(catalog, $"{InfoResult.BVID}.mp3"), Path.Combine(catalog, $"{InfoResult.Title}.mp3"));
                    Application.Current.Dispatcher.Invoke(() => new ScreenDownNofityView(CommonHelper.DownloadFinishInformation, catalog).Show());
                }
            });
        }
        private void SaveAMerge()
        {
            if (DataResult == null) return;
            Task.Run(async () =>
            {
                var catalog = Path.Combine(CommonHelper.DownloadPath, "Bilibili");
                var res = await Path.Combine(catalog, $"{InfoResult.BVID}.mp4").M4VAMerge(DataResult.AudioDash, DataResult.VideoDash);
                if (res)
                {
                    File.Move(Path.Combine(catalog, $"{InfoResult.BVID}.mp4"), Path.Combine(catalog, $"{InfoResult.Title}.mp4"));
                    Application.Current.Dispatcher.Invoke(() => new ScreenDownNofityView(CommonHelper.DownloadFinishInformation, catalog).Show());
                }
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
                SessionCode= File.ReadAllText(CookiePath);
            }
        }
        private void ErrorNotify()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new ScreenNotifyView(CommonHelper.ComponentErrorInformation).Show();
            });
        }
        #endregion
    }
}
