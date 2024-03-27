using CandySugar.Bilibili.Models;
using CandySugar.Com.Library.Timers;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using XExten.Advance.LinqFramework;

namespace CandySugar.Bilibili.ViewModels
{
    public class IndexViewModel : PropertyChangedBase
    {

        public IndexViewModel()
        {
            SessionCode = string.Empty;
            ConQueues = [];
            InfoResults = [];
            DataResults = [];
            CompleteEvent += CompleteActionEvent;
            Merge();
            ReadCookie();
            ReadClipboradContent();
            TimerEvent();
        }

        private double Counts = 0;
        private string[] Special = { "|", "*", "?", ">", "<", ":", "\"", "/", "\\" };
        private string Catalog = Path.Combine(CommonHelper.DownloadPath, "Bilibili");
        private string CookiePath = Path.Combine(CommonHelper.DownloadPath, "Bilibili", "Cookie.txt");
        private string SessionCode;
        private BiliVideoInfoModel InfoResult;
        private Dictionary<string, bool> Complete;
        private ConcurrentQueue<BiliVideoDataModel> ConQueues;
        private bool IsBatchVideo = false;

        #region Event
        private event Action CompleteEvent;
        /// <summary>
        /// 完成通知事件
        /// </summary>
        private void CompleteActionEvent()
        {
            if (!Complete.Values.Any(t => t==false))
            {
                Application.Current.Dispatcher.Invoke(() => new ScreenDownNofityView(CommonHelper.DownloadFinishInformation, Catalog).Show());
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


        #region Property
        private string _Route;
        public string Route
        {
            get => _Route;
            set => SetAndNotify(ref _Route, value);
        }

        private ObservableCollection<BiliVideoInfoModel> _InfoResults;
        /// <summary>
        /// 视频信息
        /// </summary>
        public ObservableCollection<BiliVideoInfoModel> InfoResults
        {
            get => _InfoResults;
            set => SetAndNotify(ref _InfoResults, value);
        }

        private ObservableCollection<BiliVideoDataModel> _DataResults;
        /// <summary>
        /// 视频数据
        /// </summary>
        public ObservableCollection<BiliVideoDataModel> DataResults
        {
            get => _DataResults;
            set => SetAndNotify(ref _DataResults, value);
        }

        private ObservableCollection<BiliFavCollectResult> _Collect;
        /// <summary>
        /// 收藏
        /// </summary>
        public ObservableCollection<BiliFavCollectResult> Collect
        {
            get => _Collect;
            set => SetAndNotify(ref _Collect, value);
        }
        #endregion

        #region Command
        /// <summary>
        /// 批量合成音频
        /// </summary>
        public void BatchAudioCommand(string key)
        {
            var param = key.AsInt();
            if (param == 1)
                BatchSaveCover();
            else if (param == 2)
                BatchSaveAudio();
            else
                BatchSaveVideo();
        }
        /// <summary>
        /// 获取收藏
        /// </summary>
        public void FavCommand()
        {
            if (SessionCode.IsNullOrEmpty())
            {
                new ScreenNotifyView(CommonHelper.CookieError).Show();
                return;
            }
            var userId = Regex.Match(Regex.Match(SessionCode, "UserID=\\d+").Value, "\\d+").Value;
            OnInitFavCollect(userId);
        }
        /// <summary>
        /// 从列表中移除
        /// </summary>
        /// <param name="input"></param>
        public void TranshCammnd(BiliVideoInfoModel input)
        {
            InfoResults.Remove(input);
            if (DataResults.Count > 0)
            {
                var Model = DataResults.FirstOrDefault(t => t.BVID == input.BVID && t.CID == input.CID);
                if (Model != null)
                    DataResults.Remove(Model);
            }
        }
        /// <summary>
        /// 设置Cookie
        /// </summary>
        public void CookieCommand()
        {
            SyncStatic.CreateDir(Path.Combine(CommonHelper.DownloadPath, "Bilibili"));
            var pro = Process.Start("notepad.exe", SyncStatic.CreateFile(CookiePath));
            pro.WaitForExit();
            ReadCookie();
        }
        /// <summary>
        /// 查询
        /// </summary>
        public void QeuryCommand()
        {
            if (Route.IsNullOrEmpty()) return;
            OnInitVideo();
        }
        /// <summary>
        /// 功能
        /// </summary>
        /// <param name="input"></param>
        public void HandleCommand(Dictionary<string, object> input)
        {
            var condition = input.Values.First().AsString().AsInt();
            InfoResult = (BiliVideoInfoModel)input.Values.Last();
            if (condition == 1) SaveCover();
            if (condition == 2) SaveVideo();
            if (condition == 3) SaveAudio();
            if (condition == 4) SaveMerge();
        }

        /// <summary>
        /// 获取收藏数据
        /// </summary>
        /// <param name="input"></param>
        public void CollectCommand(BiliFavCollectResult input)
        {
            for (int index = 1; index <= input.Count; index++)
            {
                OnInitFavData(input.FId, index);
            }
        }
        #endregion

        #region BatchMethod
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
                new ScreenNotifyView(CommonHelper.DownloadWait).Show();
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

        #region Method
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
                    Application.Current.Dispatcher.Invoke(() => new ScreenDownNofityView(CommonHelper.DownloadFinishInformation, Catalog).Show());
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
                    Application.Current.Dispatcher.Invoke(() => new ScreenDownNofityView(CommonHelper.DownloadFinishInformation, Catalog).Show());
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
                    Application.Current.Dispatcher.Invoke(() => new ScreenDownNofityView(CommonHelper.DownloadFinishInformation, catalog).Show());
                });
            });
        }

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
                if (InfoResults.FirstOrDefault(t => t.CID == result.CID && t.BVID == result.BVID) == null)
                    InfoResults.Add(result.ToMapest<BiliVideoInfoModel>());
                OnInitData(result.ToMapest<BiliVideoInfoModel>());
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
                Collect = new ObservableCollection<BiliFavCollectResult>(result);
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
                                Counts = 0;
                                if (IsBatchVideo)
                                {
                                    if(Complete.ContainsKey(InfoResult.BVID))
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
    }
}
