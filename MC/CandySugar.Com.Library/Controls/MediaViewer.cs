using System;

namespace CandySugar.Com.Library.Controls
{
    public class MediaViewer : View
    {
        public event Action ReloadRequested;
        public event Action DisposeRequested;
        public event Action PauseRequested;
        public event Action PlayRequested;
        public event Action<double> TimeRequested;
        public event Action<float> RateRequested;

        public static BindableProperty VideoUrlProperty =
            BindableProperty.Create(nameof(VideoUrl), typeof(string), typeof(MediaViewer), "", defaultBindingMode: BindingMode.TwoWay);

        public static BindableProperty CurrentTimeProperty =
          BindableProperty.Create(nameof(CurrentTime), typeof(string), typeof(MediaViewer), "00:00:00", defaultBindingMode: BindingMode.TwoWay);

        public static BindableProperty PositionProperty =
            BindableProperty.Create(nameof(Position), typeof(double), typeof(MediaViewer), 0d, defaultBindingMode: BindingMode.TwoWay);

        /// <summary>
        /// Disables or enables scanning
        /// </summary>
        public string VideoUrl
        {
            get => (string)GetValue(VideoUrlProperty);
            set => SetValue(VideoUrlProperty, value);
        }
        public string CurrentTime
        {
            get => (string)GetValue(CurrentTimeProperty);
            set => SetValue(CurrentTimeProperty, value);
        }
        public double Position
        {
            get => (double)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public bool IsPlaying { get; set; }

        public void Pause()
        {
            PauseRequested?.Invoke();
        }

        public void Play()
        {
            PlayRequested?.Invoke();
        }

        public void CurrentChange(double value)
        {
            TimeRequested?.Invoke(value);
        }

        public void Reload()
        {
            ReloadRequested?.Invoke();
        }

        public void Dispose()
        {
            DisposeRequested?.Invoke();
        }

        public void SetRate(float value)
        {
            RateRequested?.Invoke(value);
        }
    }
}

