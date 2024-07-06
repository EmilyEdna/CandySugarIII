

namespace CandySugar.Music.ViewModels
{
    public partial class IndexViewModel : BasicObservableObject
    {

        public IndexViewModel()
        {
            Handle = false;
            PlayTimer = new() { Interval = 1000 };
            BasicResult = [];
            Title = ["单曲", "歌单", "收藏"];
            NavVisible = Visibility.Hidden;
            PlatformType = PlatformEnum.NeteaseMusic;
            MenuData = new Dictionary<string, string> {
                { "QQ音乐","1"},{ "网易音乐","2"},
                { "酷狗音乐","3"},{ "酷我音乐","4"},
                { "咪咕音乐","5"}, { "网易电台","6"}
            };
            Setting = [
                new() { Width = 80, UseUnderLine = Visibility.Collapsed, Content = FontIcon.Repeat },
                new() { Width = 80, UseUnderLine = Visibility.Collapsed, Content = FontIcon.Repeat1 }
                ];
            Service = IocDependency.Resolve<IService<MusicModel>>();
            GenericDelegate.SearchAction = new(SearchHandler);
            GenericDelegate.WindowStateEvent += WindowStateEvent;
            CollectResult = new(Service.QueryAll());
            WindowStateEvent();
        }

        #region 字段
        private IService<MusicModel> Service;

        /// <summary>
        /// 播放器
        /// </summary>
        public AudioFactory AudioFactory => AudioFactory.Instance;
        /// <summary>
        /// 播放索引
        /// </summary>
        private int PlayIndex = 0;
        /// <summary>
        /// 1列表循环 2单曲循环
        /// </summary>
        private int PlayMoudle = 1;
        /// <summary>
        /// 歌曲切换Timer
        /// </summary>
        private Timer PlayTimer;
        /// <summary>
        /// 1 单曲 2歌单 3列表
        /// </summary>
        private int HandleType = 1;
        /// <summary>
        /// 检索关键字
        /// </summary>
        private string SearchKeyword;
        /// <summary>
        /// 平台
        /// </summary>
        private PlatformEnum PlatformType;
        /// <summary>
        /// 单曲页码
        /// </summary>
        private int SearchPageIndex = 1;
        /// <summary>
        /// 单曲总数
        /// </summary>
        private int SearchTotal;
        /// <summary>
        /// 歌单页码
        /// </summary>
        private int SheetPageIndex = 1;
        /// <summary>
        /// 歌单总数
        /// </summary>
        private int SheetTotal;
        /// <summary>
        /// 歌词
        /// </summary>
        private List<MusicLyricElemetResult> LyricResult;
        /// <summary>
        /// 歌单Id
        /// </summary>
        private string SheetId;
        /// <summary>
        /// 电台页面
        /// </summary>
        private int DjRadioPageIndex = 1;
        /// <summary>
        /// 电台歌曲总数
        /// </summary>
        private int DjRadioTotal;
        #endregion

        #region 事件
        private void WindowStateEvent()
        {
            if (GlobalParam.WindowState == WindowState.Maximized)
                MarginThickness = new Thickness(0, 0, 60, 20);
            else
                MarginThickness = new Thickness(0, 0, 60, 15);
            BorderHeight = GlobalParam.MAXHeight-100;
            BorderWidth = (GlobalParam.MAXWidth / 2) - 130;
        }

        private void EventCommon()
        {
            CurrentPlay = CollectResult[PlayIndex].ToMapper<MusicSongElementResult>();
            OnInitLyric(CurrentPlay);
            if (!DownUtil.FileExists($"[High]{CurrentPlay.SongId}", FileTypes.Mp3, "Music"))
                //播放下一首
                OnInitDown(CurrentPlay);
            AudioPlays();
        }

        private void ListRuchEvent(object sender, ElapsedEventArgs e)
        {
            if (AudioFactory.WaveOutReadOnly != null && AudioFactory.WaveOutReadOnly.PlaybackState == PlaybackState.Stopped)
            {
                var PlayNum = CollectResult.Count;
                //播放完成
                if (Math.Truncate(Live.LiveSeconds) >= Math.Truncate(AudioInfo.Seconds))
                {
                    PlayIndex += 1;
                    if (PlayIndex < PlayNum)
                    {
                        EventCommon();
                    }
                    else
                    {
                        PlayIndex = 0;
                        EventCommon();
                    }
                }
            }
        }

        private void SingleEvent(object sender, ElapsedEventArgs e)
        {
            if (AudioFactory.WaveOutReadOnly != null && AudioFactory.WaveOutReadOnly.PlaybackState == PlaybackState.Stopped)
            {
                //播放完成
                if (Math.Truncate(Live.LiveSeconds * 10) / 10 >= Math.Truncate(AudioInfo.Seconds * 10) / 10)
                    EventCommon();
            }
        }
        #endregion

        #region 属性
        [ObservableProperty]
        private bool _Handle;
        [ObservableProperty]
        private ObservableCollection<string> _Title;
        [ObservableProperty]
        private Dictionary<string, string> _MenuData;
        [ObservableProperty]
        private MusicSongElementResult _CurrentPlay;
        [ObservableProperty]
        private ObservableCollection<CandyToggleItemSetting> _Setting;
        [ObservableProperty]
        private ObservableCollection<MusicSongElementResult> _BasicResult;
        [ObservableProperty]
        private ObservableCollection<MusicSongElementResult> _SingleResult;
        [ObservableProperty]
        private ObservableCollection<MusicSheetElementResult> _SheetResult;
        [ObservableProperty]
        private ObservableCollection<MusicModel> _CollectResult;
        [ObservableProperty]
        private AudioModel _AudioInfo;
        [ObservableProperty]
        private AudioLive _Live;
        [ObservableProperty]
        private string _CurrentLyric;
        #endregion

        #region 命令
        [RelayCommand]
        public void Close()
        {
            NavVisible = Visibility.Hidden;
            BasicResult = [];
        }
        [RelayCommand]
        public void Changed(object item)
        {
            var Target = ((CandyToggleItem)item);
            if (Target.FindParent<UserControl>() is IndexView View)
            {
                var Index = Target.Tag.ToString().AsInt();

                if (Index == 0)
                {
                    View.ActiveAnime = 1;
                    View.AnimeX1.Begin();
                }
                if (Index == 1)
                {
                    View.ActiveAnime = 2;
                    View.AnimeX2.Begin();
                }
                if (Index == 2)
                {
                    View.ActiveAnime = 3;
                    View.AnimeX3.Begin();
                }
            }
        }
        [RelayCommand]
        public void Scroll(ScrollChangedEventArgs obj)
        {
            if (HandleType == 1)
                if (SearchPageIndex <= SearchTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    SearchPageIndex += 1;
                    OnLoadMoreSingle();
                }
            if (HandleType == 2)
                if (SheetPageIndex <= SheetTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    SheetPageIndex += 1;
                    OnLoadMoreSheet();
                }
        }
        [RelayCommand]
        public void Album(string albumId) => OnInitAlbum(albumId);
        [RelayCommand]
        public void Down(MusicSongElementResult input) => OnInitDown(input);
        [RelayCommand]
        public void Trash(MusicModel input)
        {
            var FileName = $"[High]{input.SongId}";
            DownUtil.FileDelete(FileName, FileTypes.Mp3, "Music");
            CollectResult.Remove(input);
            Service.Remove(input.PId);
        }
        [RelayCommand]
        public void Sheet(dynamic sheetId)
        {
            DjRadioPageIndex = 1;
            SheetId = sheetId.ToString();
            OnInitLists();
        }
        [RelayCommand]
        public void LoadMore(ScrollChangedEventArgs obj)
        {
            if (PlatformEnum.DjRadioMusic == PlatformType)
            {
                if (DjRadioPageIndex <= DjRadioTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    DjRadioPageIndex += 1;
                    OnLoadMoreInitLists();
                }
            }
        }
        [RelayCommand]
        public void Active(object input) 
        {
            var Data = input.ToMapest<AnonymousWater>().SelectValue.AsString().AsInt();
            if (Data == 0) return;
            PlatformType = (PlatformEnum)Data;
            if (SearchKeyword.IsNullOrEmpty())
            {
                new CandyNotifyControl(CommonHelper.SearckWordErrorInfomartion).Show();
                return;
            }
            SearchPageIndex = SheetPageIndex = 1;
            if (HandleType == 1)
                OnInitSingle();
            if (HandleType == 2)
                OnInitSheet();
        }
        #endregion

        #region 播放命令
        [RelayCommand]
        public void PlayChangeModule(object item)
        {
            var Target = ((CandyToggleItem)item);
            var Index = Target.Tag.ToString().AsInt() + 1;
            if (this.PlayMoudle != Index)
                this.PlayMoudle = Index;
            if (AudioFactory.WaveOutReadOnly != null) //此时正在播放
            {
                if (this.PlayMoudle == 1) ListRuch();
                if (this.PlayMoudle == 2) Single();
            }
        }

        [RelayCommand]
        public void SkipPrevious()
        {
            PlayIndex -= 1;
            if (PlayIndex < 0) PlayIndex = CollectResult.Count - 1;
            if (AudioFactory.WaveOutReadOnly != null)
                PlayConditions();
        }
        [RelayCommand]
        public void Play()
        {
            PlayConditions();
            if (CollectResult.Count > 0)
                Handle = !Handle;
        }
        [RelayCommand]
        public void Pause()
        {
            Handle = !Handle;
            if (AudioFactory.WaveOutReadOnly != null)
                AudioFactory.WaveOutReadOnly.Pause();
        }
        [RelayCommand]
        public void SkipNext()
        {
            PlayIndex += 1;
            if (PlayIndex > CollectResult.Count - 1) PlayIndex = 0;
            if (AudioFactory.WaveOutReadOnly != null)
                PlayConditions();
        }
        #endregion

        #region 函数
        public void ChangeActive(int ActiveAnime)
        {
            HandleType = ActiveAnime;
            if (ActiveAnime == 3) return;
            if (SearchKeyword.IsNullOrEmpty())
            {
                new CandyNotifyControl(CommonHelper.SearckWordErrorInfomartion).Show();
                return;
            }
            if (SingleResult == null)
                OnInitSingle();
            if (SheetResult == null)
                OnInitSheet();
        }

        private void ErrorNotify(string input = "") =>
                    Application.Current.Dispatcher.Invoke(() => new CandyNotifyControl(input.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : input).Show());

        private void AudioPlays()
        {
            if (AudioFactory.WaveOutReadOnly != null && AudioFactory.WaveOutReadOnly.PlaybackState == PlaybackState.Paused)
            {
                AudioFactory.WaveOutReadOnly.Play();
                return;
            }
            AudioFactory.InitAudio(DownUtil.FilePath($"[High]{CurrentPlay.SongId}", FileTypes.Mp3, "Music"))
                .RunPlay(Info => AudioInfo = Info).InitLiveData(Info =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Live = Info;
                        if (LyricResult != null && LyricResult.Count > 0)
                        {
                            var lyric = LyricResult.FirstOrDefault(item => item.Time.Split(".").FirstOrDefault().Equals(Info.LiveSpan));
                            CurrentLyric = lyric == null ? CurrentLyric : lyric.Lyric;
                        }
                    });
                });
        }
        #endregion

        #region 方法
        /// <summary>
        /// 歌单列表
        /// </summary>
        /// <param name="sheetId"></param>
        private void OnInitLists()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await MusicFactory.Music(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            PlatformType = PlatformType,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MusicType = MusicEnum.SheetDetail,
                            SheetDetail = new MusicSheetDetail
                            {
                                Id = SheetId
                            }
                        };
                    }).RunsAsync()).SheetDetailResult;
                    BasicResult = new ObservableCollection<MusicSongElementResult>(result.ElementResults);
                    if (PlatformType == PlatformEnum.DjRadioMusic) DjRadioTotal = result.MusicNum;
                    NavVisible = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 加载更多歌单列表
        /// </summary>
        private void OnLoadMoreInitLists()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await MusicFactory.Music(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            PlatformType = PlatformType,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MusicType = MusicEnum.SheetDetail,
                            SheetDetail = new MusicSheetDetail
                            {
                                Page = DjRadioPageIndex,
                                Id = SheetId
                            }
                        };
                    }).RunsAsync()).SheetDetailResult;
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(BasicResult.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 专辑关联
        /// </summary>
        /// <param name="albumId"></param>
        private void OnInitAlbum(string albumId)
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await MusicFactory.Music(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            PlatformType = PlatformType,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MusicType = MusicEnum.AlbumDetail,
                            AlbumDetail = new MusicAlbumDetail
                            {
                                AlbumId = albumId
                            }
                        };
                    }).RunsAsync()).AlbumResult;
                    BasicResult = new ObservableCollection<MusicSongElementResult>(result.ElementResults);
                    NavVisible = Visibility.Visible;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 单曲查询
        /// </summary>
        private void OnInitSingle()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await MusicFactory.Music(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            PlatformType = PlatformType,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MusicType = MusicEnum.Song,
                            Search = new MusicSearch
                            {
                                Page = 1,
                                KeyWord = SearchKeyword
                            }
                        };
                    }).RunsAsync()).SongResult;
                    SearchTotal = result.Total ?? 0;
                    SingleResult = new ObservableCollection<MusicSongElementResult>(result.ElementResults);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 加载更多单曲
        /// </summary>
        private void OnLoadMoreSingle()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await MusicFactory.Music(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            PlatformType = PlatformType,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MusicType = MusicEnum.Song,
                            Search = new MusicSearch
                            {
                                Page = SearchPageIndex,
                                KeyWord = SearchKeyword
                            }
                        };
                    }).RunsAsync()).SongResult;
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(SingleResult.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 歌单查询
        /// </summary>
        private void OnInitSheet()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await MusicFactory.Music(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            PlatformType = PlatformType,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MusicType = MusicEnum.Sheet,
                            Search = new MusicSearch
                            {
                                Page = 1,
                                KeyWord = SearchKeyword
                            }
                        };
                    }).RunsAsync()).SheetResult;
                    SheetTotal = result.Total ?? 0;
                    SheetResult = new ObservableCollection<MusicSheetElementResult>(result.ElementResults);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 加载更多歌单
        /// </summary>
        private void OnLoadMoreSheet()
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await MusicFactory.Music(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            PlatformType = PlatformType,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MusicType = MusicEnum.Sheet,
                            Search = new MusicSearch
                            {
                                Page = SheetPageIndex,
                                KeyWord = SearchKeyword
                            }
                        };
                    }).RunsAsync()).SheetResult;
                    Application.Current.Dispatcher.Invoke(() => result.ElementResults.ForEach(SheetResult.Add));
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        /// <summary>
        /// 歌曲下载
        /// </summary>
        /// <param name="input"></param>
        private void OnInitDown(MusicSongElementResult input)
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await MusicFactory.Music(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            PlatformType = PlatformType,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MusicType = MusicEnum.Route,
                            Play = PlatformType == PlatformEnum.KuGouMusic ? new MusicPlaySearch
                            {
                                Dynamic = input.SongId,
                                KuGouAlbumId = input.SongAlbumId,
                            } : new MusicPlaySearch
                            {
                                Dynamic = input.SongId,
                            }
                        };
                    }).RunsAsync()).PlayResult;
                    if (!result.CanPlay)
                    {
                        ErrorNotify("当前歌曲已下架，请切换平台!");
                        return;
                    }
                    var fileBytes = await (new HttpClient().GetByteArrayAsync(result.SongURL));

                    fileBytes.FileCreate(input.SongId, FileTypes.Mp3, "Music", async (catalog, fileName) =>
                    {
                        if (await fileName.Mp3ToHighMP3(catalog))
                        {
                            //删除文件
                            SyncStatic.DeleteFile(Path.Combine(catalog, fileName));
                            new CandyNotifyControl(CommonHelper.DownloadFinishInformation, true, catalog).Show();
                        };
                        if (!CollectResult.Any(t => t.SongId == input.SongId))
                        {
                            var Model = input.ToMapper<MusicModel>();
                            Model.PId = Service.Insert(Model);
                            CollectResult.Add(Model);
                        }
                    });
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        private void OnInitLyric(MusicSongElementResult input)
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = (await MusicFactory.Music(opt =>
                    {
                        opt.RequestParam = new Input
                        {
                            PlatformType = input.MusicPlatformType,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MusicType = MusicEnum.Lyric,
                            Lyric = new MusicLyricSearch
                            {
                                Dynamic = input.SongId
                            }
                        };
                    }).RunsAsync()).LyricResult.Lyrics;
                    LyricResult = result;
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "");
                    ErrorNotify();
                }
            });
        }
        #endregion

        #region ExternalCalls
        /// <summary>
        /// 检索数据
        /// </summary>
        /// <param name="keyword"></param>
        private void SearchHandler(string keyword)
        {
            SearchKeyword = keyword;
            SearchPageIndex = SheetPageIndex = 1;
            if (HandleType == 1)
                OnInitSingle();
            if (HandleType == 2)
                OnInitSheet();
        }
        #endregion

        #region 音频方法
        private void PlayConditions()
        {
            if (CollectResult.Count <= 0)
            {
                ErrorNotify("收藏列表未添加歌曲!");
                return;
            }
            CurrentPlay = CollectResult[PlayIndex].ToMapper<MusicSongElementResult>();
            OnInitLyric(CurrentPlay);
            if (PlayMoudle == 1)
            {
                if (!DownUtil.FileExists($"[High]{CurrentPlay.SongId}", FileTypes.Mp3, "Music"))
                    OnInitDown(CurrentPlay);
                AudioPlays();
                ListRuch();
            }
            if (PlayMoudle == 2)
            {
                if (!DownUtil.FileExists($"[High]{CurrentPlay.SongId}", FileTypes.Mp3, "Music"))
                    OnInitDown(CurrentPlay);
                AudioPlays();
                Single();
            }
        }
        /// <summary>
        /// 列表循环
        /// </summary>
        private void ListRuch()
        {
            PlayTimer.Elapsed -= SingleEvent;
            PlayTimer.Elapsed += ListRuchEvent;
            PlayTimer.Start();
        }
        /// <summary>
        /// 单曲循环
        /// </summary>
        private void Single()
        {
            PlayTimer.Elapsed -= ListRuchEvent;
            PlayTimer.Elapsed += SingleEvent;
            PlayTimer.Start();
        }
        #endregion
    }
}
