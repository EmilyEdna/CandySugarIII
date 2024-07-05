namespace CandySugar.Bilibili.ViewModels
{
    public partial class IndexViewModel : BasicObservableObject
    {
        public IndexViewModel()
        {
            SessionCode = string.Empty;
            ConQueues = [];
            InfoResults = [];
            DataResults = [];
            Merge();
            ReadCookie();
            WindowStateEvent();
            ReadClipboradContent();
            TimerEvent();
            CompleteEvent += CompleteActionEvent;
            GenericDelegate.WindowStateEvent += WindowStateEvent;
        }

        #region 事件
        private event Action CompleteEvent;

        private void WindowStateEvent()
        {
            NavHeight = GlobalParam.NavHeight;
        }

        /// <summary>
        /// 完成通知事件
        /// </summary>
        private void CompleteActionEvent()
        {
            if (!Complete.Values.Any(t => t == false))
            {
                Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(CommonHelper.DownloadFinishInformation,true, Catalog).Show());
                Complete.Clear();
            }
        }
        private void TimerEvent()
        {
            TimerHelper.InitTimer(1000, (uc) =>
            {
                TimerHelper.Stop();
                if (ConQueues.Count <= 0)
                {
                    IsBatchVideo = false;
                    CompleteActionEvent();
                    return;
                }
                ConQueues.TryDequeue(out BiliVideoDataModel model);
                InfoResult = InfoResults.FirstOrDefault(t => t.BVID == model.BVID && t.CID == model.CID);
                Task.Run(async () =>
                {
                    var data = new Dictionary<string, string> {
                        { Path.Combine(Catalog, $"mp4_{InfoResult.BVID}.m4s"),model.VideoDash },
                        { Path.Combine(Catalog, $"mp3_{InfoResult.BVID}.m4s"),model.AudioDash }
                    };
                    await HttpSchedule.HttpDownload(data, header =>
                    {
                        header.Add(ConstDefault.Referer, "https://www.bilibili.com/");
                    });
                });

            });
        }
        #endregion

        #region 字段
        private double Counts = 0;
        private string[] Special = { "|", "*", "?", ">", "<", ":", "\"", "/", "\\" };
        private string Catalog = Path.Combine(CommonHelper.DownloadPath, "Bilibili");
        private string CookiePath = Path.Combine(CommonHelper.DownloadPath, "Bilibili", "Cookie.txt");
        private string SessionCode;
        private BiliVideoInfoModel InfoResult;
        private Dictionary<string, bool> Complete;
        private ConcurrentQueue<BiliVideoDataModel> ConQueues;
        private bool IsBatchVideo = false;
        #endregion

        #region 属性
        [ObservableProperty]
        private string _Route;
        [ObservableProperty]
        private ObservableCollection<BiliVideoInfoModel> _InfoResults;
        [ObservableProperty]
        private ObservableCollection<BiliVideoDataModel> _DataResults;
        [ObservableProperty]
        private ObservableCollection<BiliFavCollectResult> _Collects;
        #endregion

        #region 命令
        [RelayCommand]
        public void Cookie()
        {
            SyncStatic.CreateDir(Path.Combine(CommonHelper.DownloadPath, "Bilibili"));
            var pro = Process.Start("notepad.exe", SyncStatic.CreateFile(CookiePath));
            pro.WaitForExit();
            ReadCookie();
        }

        [RelayCommand]
        public void Clear()
        {
            InfoResults.Clear();
            DataResults.Clear();
        }

        [RelayCommand]
        public void Qeury()
        {
            if (Route.IsNullOrEmpty()) return;
            OnInitVideo();
        }

        [RelayCommand]
        public void Fav()
        {
            if (SessionCode.IsNullOrEmpty())
            {
                new CandyNotifyControl(CommonHelper.CookieError).Show();
                return;
            }
            var userId = SessionCode.WithRegex("UserID=\\d+").WithRegex("\\d+");
            OnInitFavCollect(userId);
        }

        [RelayCommand]
        public void BatchAudio(string input)
        {
            var param = input.AsInt();
            if (param == 1)
                BatchSaveCover();
            else if (param == 2)
                BatchSaveAudio();
            else
                BatchSaveVideo();
        }

        [RelayCommand]
        public void Collect(BiliFavCollectResult input)
        {
            for (int index = 1; index <= input.Count; index++)
            {
                OnInitFavData(input.FId, index);
            }
        }

        [RelayCommand]
        public void Transh(BiliVideoInfoModel input)
        {
            InfoResults.Remove(input);
            if (DataResults.Count > 0)
            {
                var Model = DataResults.FirstOrDefault(t => t.BVID == input.BVID && t.CID == input.CID);
                if (Model != null)
                    DataResults.Remove(Model);
            }
        }

        [RelayCommand]
        public void Handle(Dictionary<string, object> input)
        {
            var condition = input.Values.First().AsString().AsInt();
            InfoResult = (BiliVideoInfoModel)input.Values.Last();
            if (condition == 1) SaveCover();
            if (condition == 2) SaveVideo();
            if (condition == 3) SaveAudio();
            if (condition == 4) SaveMerge();
        }
        #endregion

        #region 方法
        private void SaveVideo()
        {
            if (DataResults.Count <= 0) return;
            var DataResult = DataResults.FirstOrDefault(t => t.CID == InfoResult.CID && t.BVID == InfoResult.BVID);
            if (DataResult == null) return;
            Task.Run(async () =>
            {
                var res = await DataResult.VideoDash.M4Video(Path.Combine(SyncStatic.CreateDir(Catalog), $"{InfoResult.BVID}.mp4"));
                if (res)
                {
                    var Pattern = "[\\|/|*/|?/|</|>/|/|:/|\\\\/|//|\"]";
                    if (InfoResult.Title.Any(t => Special.Contains(t.ToString())))
                        InfoResult.Title = Regex.Replace(InfoResult.Title, Pattern, "_");
                    else
                        InfoResult.Title = InfoResult.Title;
                    File.Move(Path.Combine(Catalog, $"{InfoResult.BVID}.mp4"), Path.Combine(Catalog, $"{InfoResult.Title}.mp4"));
                    Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(CommonHelper.DownloadFinishInformation,true, Catalog).Show());
                }
            });
        }

        private void SaveAudio()
        {
            if (DataResults.Count <= 0) return;
            var DataResult = DataResults.FirstOrDefault(t => t.CID == InfoResult.CID && t.BVID == InfoResult.BVID);
            if (DataResult == null) return;
            Task.Run(async () =>
            {
                var res = await DataResult.AudioDash.M4Audio(Path.Combine(SyncStatic.CreateDir(Catalog), $"{InfoResult.BVID}.mp3"));
                if (res)
                {
                    var Pattern = "[\\|/|*/|?/|</|>/|/|:/|\\\\/|//|\"]";
                    if (InfoResult.Title.Any(t => Special.Contains(t.ToString())))
                        InfoResult.Title = Regex.Replace(InfoResult.Title, Pattern, "_");
                    else
                        InfoResult.Title = InfoResult.Title;
                    File.Move(Path.Combine(Catalog, $"{InfoResult.BVID}.mp3"), Path.Combine(Catalog, $"{InfoResult.Title}.mp3"));
                    Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(CommonHelper.DownloadFinishInformation, true, Catalog).Show());
                }
            });
        }

        private void SaveMerge()
        {
            if (DataResults.Count <= 0) return;
            var DataResult = DataResults.FirstOrDefault(t => t.CID == InfoResult.CID && t.BVID == InfoResult.BVID);
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

        private void SaveCover()
        {
            Task.Run(async () =>
            {
                var bytes = await new HttpClient().GetByteArrayAsync(InfoResult.Cover);
                bytes.FileCreate(InfoResult.Title, FileTypes.Jpg, "Bilibili", (catalog, fileName) =>
                {
                    Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(CommonHelper.DownloadFinishInformation,true, catalog).Show());
                });
            });
        }

        private void Merge()
        {
            HttpSchedule.ReceiveAction = new((item, num) =>
            {
                if (item == double.Parse((100 / num).ToString("F2")))
                    Counts += item;
                if (InfoResult != null)
                {
                    var Model = InfoResults.First(t => t.CID == InfoResult.CID && t.BVID == InfoResult.BVID);
                    Model.Width = (Counts + item) * 2.66;
                }
                if (Math.Ceiling(Counts) >= 100)
                {
                    Task.Run(async () =>
                    {
                        var res = await Path.Combine(Catalog, $"{InfoResult.BVID}.mp4").M4VAMerge(Path.Combine(Catalog, $"mp3_{InfoResult.BVID}.m4s"), Path.Combine(Catalog, $"mp4_{InfoResult.BVID}.m4s"));
                        if (res)
                        {
                            if (IsBatchVideo == false)
                                Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(CommonHelper.DownloadFinishInformation,true, Catalog).Show());
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
                                Counts = 0;
                                if (IsBatchVideo)
                                {
                                    if (Complete.ContainsKey(InfoResult.BVID))
                                        Complete[InfoResult.BVID] = true;

                                    await Task.Delay(1500);
                                    TimerHelper.Start();
                                }
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

        private void ReadClipboradContent()
        {
            GenericDelegate.ClipboardAction = new(data =>
            {
                this.Route = data.ToString();
            });
        }
        #endregion

        #region 批处理
        private void BatchSaveCover()
        {
            if (DataResults.Count <= 0 || InfoResults.Count <= 0) return;
            Complete = DataResults.ToDictionary(t => t.BVID, t => false);
            foreach (var DataResult in DataResults)
            {
                var InfoResult = InfoResults.FirstOrDefault(t => t.CID == DataResult.CID && t.BVID == DataResult.BVID);
                if (InfoResult == null) continue;
                Task.Run(async () =>
                {
                    var bytes = await new HttpClient().GetByteArrayAsync(InfoResult.Cover);
                    bytes.FileCreate(InfoResult.Title, FileTypes.Jpg, "Bilibili", (catalog, fileName) =>
                    {
                        Complete[InfoResult.BVID] = true;
                        CompleteEvent?.Invoke();
                    });
                });
            }
        }
        private void BatchSaveAudio()
        {
            if (DataResults.Count <= 0 || InfoResults.Count <= 0) return;
            Complete = DataResults.ToDictionary(t => t.BVID, t => false);
            foreach (var DataResult in DataResults)
            {
                var InfoResult = InfoResults.FirstOrDefault(t => t.CID == DataResult.CID && t.BVID == DataResult.BVID);
                if (InfoResult == null) continue;
                Task.Run(async () =>
                {
                    var res = await DataResult.AudioDash.M4Audio(Path.Combine(SyncStatic.CreateDir(Catalog), $"{InfoResult.BVID}.mp3"));
                    if (res)
                    {
                        var Pattern = "[\\|/|*/|?/|</|>/|/|:/|\\\\/|//|\"]";
                        if (InfoResult.Title.Any(t => Special.Contains(t.ToString())))
                            InfoResult.Title = Regex.Replace(InfoResult.Title, Pattern, "_");
                        else
                            InfoResult.Title = InfoResult.Title;
                        File.Move(Path.Combine(Catalog, $"{InfoResult.BVID}.mp3"), Path.Combine(Catalog, $"{InfoResult.Title}.mp3"));
                        Complete[InfoResult.BVID] = true;
                        CompleteEvent?.Invoke();
                    }
                });
            }
        }
        private void BatchSaveVideo()
        {
            if (IsBatchVideo)
            {
                new CandyNotifyControl(CommonHelper.DownloadWait).Show();
                return;
            }
            if (DataResults.Count <= 0 || InfoResults.Count <= 0) return;
            Complete = DataResults.ToDictionary(t => t.BVID, t => false);
            DataResults.ForEnumerEach(item =>
            {
                if (ConQueues.FirstOrDefault(t => t.BVID == item.BVID && t.CID == item.CID) == null)
                    ConQueues.Enqueue(item);
            });
            SyncStatic.CreateDir(Catalog);
            IsBatchVideo = true;
            TimerHelper.Start();
        }
        #endregion

        #region 函数
        private async void OnInitData(BiliVideoInfoModel InfoResult)
        {
            try
            {
                var Proxy = Module.IocModule.Proxy;
                var result = (await BilibiliFactory.Bili(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        ProxyIP = Proxy.IP,
                        ProxyPort = Proxy.Port,
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
                DataResults.Add(new BiliVideoDataModel
                {
                    AudioDash = result.AudioDash,
                    VideoDash = result.VideoDash,
                    BVID = InfoResult.BVID,
                    CID = InfoResult.CID,
                });
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
                var Proxy = Module.IocModule.Proxy;
                var result = (await BilibiliFactory.Bili(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        ProxyIP = Proxy.IP,
                        ProxyPort = Proxy.Port,
                        BiliType = BiliEnum.VideoInfo,
                        CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                        VideoInfo = new BiliVideoInfo
                        {
                            Route = Route,
                        }
                    };
                }).RunsAsync()).InfoResult;

                result.CIDName.ForDicEach((CID, Title) =>
                {
                    if (InfoResults.FirstOrDefault(t => t.CID == CID && t.BVID == result.BVID) == null)
                    {
                        var Model = new BiliVideoInfoModel
                        {
                            BVID = result.BVID,
                            CID = CID,
                            Cover = result.Cover,
                            Title = Title,
                        };
                        InfoResults.Add(Model);
                        OnInitData(Model);
                    }
                });
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "");
                ErrorNotify();
            }
        }

        private async void OnInitFavCollect(string userId)
        {
            try
            {
                var Proxy = Module.IocModule.Proxy;
                var result = (await BilibiliFactory.Bili(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        ProxyIP = Proxy.IP,
                        ProxyPort = Proxy.Port,
                        BiliType = BiliEnum.FavCollect,
                        CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                        FavList = new BiliFavCollect
                        {
                            UserId = userId
                        }
                    };
                }).RunsAsync()).FavCollectResults;
                Collects = new ObservableCollection<BiliFavCollectResult>(result);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "");
                ErrorNotify();
            }
        }

        private async void OnInitFavData(string FId, int PageIndex)
        {
            try
            {
                var Proxy = Module.IocModule.Proxy;
                var result = (await BilibiliFactory.Bili(opt =>
                {
                    opt.RequestParam = new Input
                    {
                        ProxyIP = Proxy.IP,
                        ProxyPort = Proxy.Port,
                        BiliType = BiliEnum.FavData,
                        CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                        FavData = new BiliFavData
                        {
                            FavId = FId,
                            PageIndex = PageIndex
                        }
                    };
                }).RunsAsync()).FavDataResults;
                result.ToMapest<List<BiliVideoInfoModel>>().ForEach(item =>
                {
                    if (InfoResults.FirstOrDefault(t => t.CID == item.CID && t.BVID == item.BVID) == null)
                    {
                        InfoResults.Add(item);
                        OnInitData(item);
                    }
                });
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

        private void ErrorNotify(string input = "") =>
                    Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(input.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : input).Show());
        #endregion
    }
}
