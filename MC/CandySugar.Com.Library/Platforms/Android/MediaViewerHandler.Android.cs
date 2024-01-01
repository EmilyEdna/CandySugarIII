using CandySugar.Com.Library.Controls;
using LibVLCSharp.Platforms.Android;
using LibVLCSharp.Shared;
using Microsoft.Maui.Handlers;
using XExten.Advance.LinqFramework;


namespace CandySugar.Com.Library.Handlers
{

    public partial class MediaViewerHandler : ViewHandler<MediaViewer, VideoView>
    {
        VideoView _videoView;
        LibVLC _libVLC;
        MediaPlayer _mediaPlayer;

        protected override VideoView CreatePlatformView() => new VideoView(Context);

        protected override void ConnectHandler(VideoView nativeView)
        {
            base.ConnectHandler(nativeView);

            PrepareControl(nativeView);
            HandleUrl(VirtualView.VideoUrl);

            base.ConnectHandler(nativeView);
        }
        #region Event

        private void VirtualView_PlayRequested()
        {
            _mediaPlayer.Play();
            VirtualView.IsPlaying = _mediaPlayer.IsPlaying;
        }
        private void VirtualView_ReloadRequested()
        {
            _mediaPlayer.Stop();
            VirtualView.IsPlaying = _mediaPlayer.IsPlaying;
            HandleUrl(VirtualView.VideoUrl);
        }
        private void VirtualView_PauseRequested()
        {
            _mediaPlayer.Pause();
            VirtualView.IsPlaying = _mediaPlayer.IsPlaying;
        }

        private void VirtualView_TimeRequested(double obj)
        {
            _mediaPlayer.Position = (float)obj;
        }
        private void VirtualView_DisposeRequested()
        {
            _mediaPlayer.Stop();
            _mediaPlayer.Dispose();
        }
        private void VirtualView_RateRequested(float obj)
        {
            if (_mediaPlayer.IsPlaying)
                _mediaPlayer.SetRate(obj);
        }
        #endregion

        #region VLCEvent
        private void VLC_PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
        {
            VirtualView.Position = e.Position;
        }

        private void VLC_TimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            VirtualView.CurrentTime = TimeSpan.FromMilliseconds(e.Time).ToString().Substring(0, 8);
        }
        #endregion

        protected override void DisconnectHandler(VideoView nativeView)
        {
            VirtualView.PauseRequested -= VirtualView_PauseRequested;
            VirtualView.PlayRequested -= VirtualView_PlayRequested;
            VirtualView.TimeRequested -= VirtualView_TimeRequested;
            VirtualView.ReloadRequested -= VirtualView_ReloadRequested;

            _mediaPlayer.TimeChanged -= VLC_TimeChanged;
            _mediaPlayer.PositionChanged -= VLC_PositionChanged;
            nativeView.Dispose();
            base.DisconnectHandler(nativeView);
        }

        private void PrepareControl(VideoView nativeView)
        {
            _libVLC = new LibVLC("--http-referrer=https://avgle.com/");
            _mediaPlayer = new MediaPlayer(_libVLC)
            {
                EnableHardwareDecoding = true
            };

            _videoView = nativeView ?? new VideoView(Context);
            _videoView.MediaPlayer = _mediaPlayer;

            VirtualView.PauseRequested += VirtualView_PauseRequested;
            VirtualView.PlayRequested += VirtualView_PlayRequested;
            VirtualView.TimeRequested += VirtualView_TimeRequested;
            VirtualView.ReloadRequested += VirtualView_ReloadRequested;
            VirtualView.DisposeRequested += VirtualView_DisposeRequested;
            VirtualView.RateRequested += VirtualView_RateRequested; ;

            _mediaPlayer.TimeChanged += VLC_TimeChanged;
            _mediaPlayer.PositionChanged += VLC_PositionChanged;
        }

        private void HandleUrl(string url)
        {
            if (!url.IsNullOrEmpty())
            {
                var media = new Media(_libVLC, url, FromType.FromLocation);
                _mediaPlayer.NetworkCaching = 20000;
                if (_mediaPlayer.Media != null)
                {
                    _mediaPlayer.Stop();
                    _mediaPlayer.Media.Dispose();
                }
                _mediaPlayer.Media = media;
                _mediaPlayer.Volume = 100;
                _mediaPlayer.AspectRatio = "770:360";
                VirtualView.IsPlaying = _mediaPlayer.IsPlaying;
            }
        }

    }

}