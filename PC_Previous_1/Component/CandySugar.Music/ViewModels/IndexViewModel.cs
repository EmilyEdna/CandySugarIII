using CandySugar.Com.Controls.ExtenControls;
using CandySugar.Com.Data.Entity.MusicEntity;
using CandySugar.Com.Data;
using CandySugar.Com.Library.VisualTree;
using XExten.Advance.JsonDbFramework;
using XExten.Advance.LinqFramework;

namespace CandySugar.Music.ViewModels
{
    public class IndexViewModel : PropertyChangedBase
    {
        private object LockObject = new object();
        private object SimpleLocker = new object();
        private IService<MusicModel> Service;
        private string DbPath = Path.Combine(CommonHelper.DownloadPath, "Music", $"Music.{FileTypes.Dat}");
        public IndexViewModel()
        {
            Handle = false;
            MenuIndex = new Dictionary<PlatformEnum, string> {
                { PlatformEnum.QQMusic,"QQ音乐"},
                { PlatformEnum.NeteaseMusic,"网易音乐"},
                { PlatformEnum.KuGouMusic,"酷狗音乐"},
                { PlatformEnum.KuWoMusic,"酷我音乐"},
                { PlatformEnum.MiGuMusic,"咪咕音乐"},
                { PlatformEnum.DjRadioMusic,"网易电台"}
            };
            Title = ["单曲", "歌单", "收藏"];
            Setting = [new() { Width = 80, UseUnderLine = Visibility.Collapsed, Content = FontIcon.Repeat }, new() { Width = 80, UseUnderLine = Visibility.Collapsed, Content = FontIcon.Repeat1 }];
            GenericDelegate.SearchAction = new(SearchHandler);
            Service = IocDependency.Resolve<IService<MusicModel>>();
            var LocalDATA = Service.QueryAll();
            CollectResult = [];
            LocalDATA?.ForEach(CollectResult.Add);
            PlayTimer = new() { Interval = 1000 };
        }

        #region Field
        /// <summary>
        /// 播放器
        /// </summary>
        public AudioFactory AudioFactory => AudioFactory.Instance;
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
        private PlatformEnum Platform;
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
        /// 侧边栏开关状态 1 开 2关
        /// </summary>
        public int SliderStatus = 2;
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

        #region Property
        private ObservableCollection<string> _Title;
        public ObservableCollection<string> Title
        {
            get => _Title;
            set => SetAndNotify(ref _Title, value);
        }
        private ObservableCollection<CandyToggleItemSetting> _Setting;
        public ObservableCollection<CandyToggleItemSetting> Setting
        {
            get => _Setting;
            set => SetAndNotify(ref _Setting, value);
        }
        private bool _Handle;
        public bool Handle
        {
            get => _Handle;
            set => SetAndNotify(ref _Handle, value);
        }
        private Dictionary<PlatformEnum, string> _MenuIndex;
        /// <summary>
        /// 平台菜单
        /// </summary>
        public Dictionary<PlatformEnum, string> MenuIndex
        {
            get => _MenuIndex;
            set => SetAndNotify(ref _MenuIndex, value);
        }
        private MusicSongElementResult _CurrentPlay;
        /// <summary>
        /// 当前播放
        /// </summary>
        public MusicSongElementResult CurrentPlay
        {
            get => _CurrentPlay;
            set => SetAndNotify(ref _CurrentPlay, value);
        }
        private AudioModel _AudioInfo;
        /// <summary>
        /// 音频信息
        /// </summary>
        public AudioModel AudioInfo
        {
            get => _AudioInfo;
            set => SetAndNotify(ref _AudioInfo, value);
        }
        private AudioLive _Live;
        /// <summary>
        /// 音频实时数据
        /// </summary>
        public AudioLive Live
        {
            get => _Live;
            set => SetAndNotify(ref _Live, value);
        }
        private ObservableCollection<MusicModel> _CollectResult;
        /// <summary>
        /// 播放列表
        /// </summary>
        public ObservableCollection<MusicModel> CollectResult
        {
            get => _CollectResult;
            set => SetAndNotify(ref _CollectResult, value);
        }
        private ObservableCollection<MusicSongElementResult> _SingleResult;
        /// <summary>
        /// 单曲
        /// </summary>
        public ObservableCollection<MusicSongElementResult> SingleResult
        {
            get => _SingleResult;
            set => SetAndNotify(ref _SingleResult, value);
        }
        private ObservableCollection<MusicSheetElementResult> _SheetResult;
        /// <summary>
        /// 歌单
        /// </summary>
        public ObservableCollection<MusicSheetElementResult> SheetResult
        {
            get => _SheetResult;
            set => SetAndNotify(ref _SheetResult, value);
        }
        private ObservableCollection<MusicSongElementResult> _BasicResult;
        /// <summary>
        /// 专辑/歌单
        /// </summary>
        public ObservableCollection<MusicSongElementResult> BasicResult
        {
            get => _BasicResult;
            set => SetAndNotify(ref _BasicResult, value);
        }
        private string _CurrentLyric;
        /// <summary>
        /// 当前歌词
        /// </summary>
        public string CurrentLyric
        {
            get => _CurrentLyric;
            set => SetAndNotify(ref _CurrentLyric, value);
        }
        #endregion

        #region Command

        /// <summary>
        /// 切换控制
        /// </summary>
        public RelayCommand<object> PlayChangeModuleCommand => new(item =>
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
        });

        /// <summary>
        /// 切换
        /// </summary>
        public RelayCommand<object> ChangedCommand => new(item =>
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
        });

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="input"></param>
        public void TrashCommand(MusicModel input)
        {
            var FileName = $"[High]{input.SongId}";
            DownUtil.FileDelete(FileName, FileTypes.Mp3, "Music");
            CollectResult.Remove(input);
            Service.Remove(input.PId);
        }

        /// <summary>
        /// 切换列表
        /// </summary>
        /// <param name="key"></param>
        public void ChangeCommand(int key)
        {
            HandleType = key;
            if (key == 3) return;
            if (SearchKeyword.IsNullOrEmpty())
            {
                new ScreenNotifyView(CommonHelper.SearckWordErrorInfomartion).Show();
                return;
            }
            if (SingleResult == null)
                OnInitSingle();
            if (SheetResult == null)
                OnInitSheet();
        }

        /// <summary>
        /// 平台切换
        /// </summary>
        /// <param name="platform"></param>
        public void ActiveCommand(PlatformEnum platform)
        {
            Platform = platform;
            if (SearchKeyword.IsNullOrEmpty())
            {
                new ScreenNotifyView(CommonHelper.SearckWordErrorInfomartion).Show();
                return;
            }
            SearchPageIndex = SheetPageIndex = 1;
            if (HandleType == 1)
                OnInitSingle();
            if (HandleType == 2)
                OnInitSheet();
        }

        /// <summary>
        /// 专辑
        /// </summary>
        /// <param name="albumId"></param>
        public void AlbumCommand(string albumId)
        {
            OnInitAlbum(albumId);
        }

        /// <summary>
        /// 歌单
        /// </summary>
        /// <param name="sheetId"></param>
        public void SheetCommand(dynamic sheetId)
        {
            DjRadioPageIndex = 1;
            SheetId = sheetId.ToString();
            OnInitLists();
        }

        /// <summary>
        /// 下载
        /// </summary>
        /// <param name="input"></param>
        public void DownCommand(MusicSongElementResult input)
        {
            OnInitDown(input);
        }

        /// <summary>
        /// 加载更多
        /// </summary>
        public RelayCommand<ScrollChangedEventArgs> ScrollCommand => new((obj) =>
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
        });

        public RelayCommand<ScrollChangedEventArgs> DJScrollCommand => new((obj) =>
        {
            if (PlatformEnum.DjRadioMusic == Platform)
            {
                if (DjRadioPageIndex <= DjRadioTotal && obj.VerticalOffset + obj.ViewportHeight == obj.ExtentHeight && obj.VerticalChange > 0)
                {
                    DjRadioPageIndex += 1;
                    OnLoadMoreInitLists();
                }
            }
        });
        /// <summary>
        /// 播放
        /// </summary>
        public void PlayCommand()
        {
            PlayConditions();
            if (CollectResult.Count > 0)
            {
                Handle = !Handle;
            }
        }

        /// <summary>
        /// 上一首
        /// </summary>
        public void SkipPreviousCommand()
        {
            PlayIndex -= 1;
            if (PlayIndex < 0) PlayIndex = CollectResult.Count - 1;
            if (AudioFactory.WaveOutReadOnly != null)
            {
                PlayConditions();
            }
        }

        /// <summary>
        /// 下一首
        /// </summary>
        public void SkipNextCommand()
        {
            PlayIndex += 1;
            if (PlayIndex > CollectResult.Count - 1) PlayIndex = 0;
            if (AudioFactory.WaveOutReadOnly != null)
            {
                PlayConditions();
            }
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void PauseCommand()
        {
            Handle = !Handle;
            if (AudioFactory.WaveOutReadOnly != null)
                AudioFactory.WaveOutReadOnly.Pause();
        }
        #endregion

        #region Method
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
                            PlatformType = Platform,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MusicType = MusicEnum.SheetDetail,
                            SheetDetail = new MusicSheetDetail
                            {
                                Id = SheetId
                            }
                        };
                    }).RunsAsync()).SheetDetailResult;
                    BasicResult = new ObservableCollection<MusicSongElementResult>(result.ElementResults);
                    if (Platform == PlatformEnum.DjRadioMusic) DjRadioTotal = result.MusicNum;
                    // 这一句很关键，开启集合的异步访问支持
                    BindingOperations.EnableCollectionSynchronization(BasicResult, LockObject);
                    WeakReferenceMessenger.Default.Send(new MessageNotify { SliderStatus = 1 });
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
                            PlatformType = Platform,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MusicType = MusicEnum.SheetDetail,
                            SheetDetail = new MusicSheetDetail
                            {
                                Page = DjRadioPageIndex,
                                Id = SheetId
                            }
                        };
                    }).RunsAsync()).SheetDetailResult;
                    lock (LockObject)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            result.ElementResults.ForEach(item =>
                            {
                                BasicResult.Add(item);
                            });
                        });
                    }
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
                            PlatformType = Platform,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MusicType = MusicEnum.AlbumDetail,
                            AlbumDetail = new MusicAlbumDetail
                            {
                                AlbumId = albumId
                            }
                        };
                    }).RunsAsync()).AlbumResult;
                    BasicResult = new ObservableCollection<MusicSongElementResult>(result.ElementResults);
                    // 这一句很关键，开启集合的异步访问支持
                    BindingOperations.EnableCollectionSynchronization(BasicResult, LockObject);
                    WeakReferenceMessenger.Default.Send(new MessageNotify { SliderStatus = 1 });
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
                             PlatformType = Platform,
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
                    // 这一句很关键，开启集合的异步访问支持
                    BindingOperations.EnableCollectionSynchronization(SingleResult, LockObject);
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
                            PlatformType = Platform,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MusicType = MusicEnum.Song,
                            Search = new MusicSearch
                            {
                                Page = SearchPageIndex,
                                KeyWord = SearchKeyword
                            }
                        };
                    }).RunsAsync()).SongResult;
                    lock (LockObject)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            result.ElementResults.ForEach(item =>
                            {
                                SingleResult.Add(item);
                            });
                        });
                    }
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
                            PlatformType = Platform,
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
                    // 这一句很关键，开启集合的异步访问支持
                    BindingOperations.EnableCollectionSynchronization(SheetResult, LockObject);
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
                            PlatformType = Platform,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MusicType = MusicEnum.Sheet,
                            Search = new MusicSearch
                            {
                                Page = SheetPageIndex,
                                KeyWord = SearchKeyword
                            }
                        };
                    }).RunsAsync()).SheetResult;
                    lock (LockObject)
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            result.ElementResults.ForEach(item =>
                            {
                                SheetResult.Add(item);
                            });
                        });
                    }
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
                            PlatformType = Platform,
                            CacheSpan = ComponentBinding.OptionObjectModels.Cache,
                            MusicType = MusicEnum.Route,
                            Play = Platform == PlatformEnum.KuGouMusic ? new MusicPlaySearch
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
                            new ScreenDownNofityView(CommonHelper.DownloadFinishInformation, catalog).Show();
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
        private void ErrorNotify(string Info = "")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new ScreenNotifyView(Info.IsNullOrEmpty() ? CommonHelper.ComponentErrorInformation : Info).Show();
            });
        }
        private void AudioPlays()
        {
            if (AudioFactory.WaveOutReadOnly != null && AudioFactory.WaveOutReadOnly.PlaybackState == PlaybackState.Paused)
            {
                AudioFactory.WaveOutReadOnly.Play();
                return;
            }
            AudioFactory.InitAudio(DownUtil.FilePath(FileName(), FileTypes.Mp3, "Music"))
                .RunPlay(Info => AudioInfo = Info).InitLiveData(Info =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Live = Info;
                        if (LyricResult != null && LyricResult.Count > 0)
                        {
                            lock (LockObject)
                            {
                                var lyric = LyricResult.FirstOrDefault(item => item.Time.Split(".").FirstOrDefault().Equals(Info.LiveSpan));
                                CurrentLyric = lyric == null ? CurrentLyric : lyric.Lyric;
                            }
                        }
                    });
                });
        }
        private string FileName() => $"[High]{CurrentPlay.SongId}";
        #endregion

        #region ExternalCalls
        /// <summary>
        /// 检索数据
        /// </summary>
        /// <param name="keyword"></param>
        private void SearchHandler(string keyword)
        {
            SearchKeyword = keyword;
            Platform = PlatformEnum.NeteaseMusic;
            SearchPageIndex = SheetPageIndex = 1;
            if (HandleType == 1)
                OnInitSingle();
            if (HandleType == 2)
                OnInitSheet();
        }
        #endregion

        #region Event
        private void EventCommon()
        {
            CurrentPlay = CollectResult[PlayIndex].ToMapper<MusicSongElementResult>();
            OnInitLyric(CurrentPlay);
            if (!DownUtil.FileExists(FileName(), FileTypes.Mp3, "Music"))
                //播放下一首
                lock (SimpleLocker)
                {
                    OnInitDown(CurrentPlay);
                }
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

        #region AudioMethod
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
                if (!DownUtil.FileExists(FileName(), FileTypes.Mp3, "Music"))
                    OnInitDown(CurrentPlay);
                AudioPlays();
                ListRuch();
            }
            if (PlayMoudle == 2)
            {
                if (!DownUtil.FileExists(FileName(), FileTypes.Mp3, "Music"))
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
