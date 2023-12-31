using CandySugar.Com.Library.Controls;
using LibVLCSharp.Platforms.Android;
using LibVLCSharp.Shared;
using Microsoft.Maui.Handlers;


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

        private void VirtualView_PlayRequested()
        {
            PrepareControl(_videoView);
            HandleUrl(VirtualView.VideoUrl);
            _mediaPlayer.Play();  
            _mediaPlayer.TimeChanged += VirtualView_TimeChanged;
            _mediaPlayer.PositionChanged += VirtualView_PositionChanged;
        }

        private void VirtualView_PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
        {
            VirtualView.Position = e.Position;
        }

        private void VirtualView_TimeChanged(object sender, MediaPlayerTimeChangedEventArgs e)
        {
            VirtualView.CurrentTime = TimeSpan.FromMilliseconds(e.Time).ToString().Substring(0, 8);
        }

        private void VirtualView_PauseRequested()
        {
            _mediaPlayer.Pause();
        }

        protected override void DisconnectHandler(VideoView nativeView)
        {
            VirtualView.PauseRequested -= VirtualView_PauseRequested;
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
        }

        private void VirtualView_TimeRequested(double obj)
        {
            _mediaPlayer.Position = (float)obj;
        }

        private void HandleUrl(string url)
        {
            try
            {

                if (url.EndsWith("/"))
                {
                    url = url.TrimEnd('/');
                }

                if (!string.IsNullOrEmpty(url))
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
                    _mediaPlayer.Mute = true;
                    _mediaPlayer.Play();
                }
            }
            catch (Exception ex)
            {
            }
        }

    }

}