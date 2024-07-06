namespace CandySugar.Music.View
{
    /// <summary>
    /// IndexView.xaml 的交互逻辑
    /// </summary>
    public partial class IndexView : UserControl
    {
        public int ActiveAnime = 1;
        private IndexViewModel ViewModel;
        public Storyboard AnimeX1;
        public Storyboard AnimeX2;
        public Storyboard AnimeX3;
        private int Vol = 0;
        public IndexView()
        {
            InitializeComponent();
            AnimeX1 = (Storyboard)FindResource("X1Key");
            AnimeX2 = (Storyboard)FindResource("X2Key");
            AnimeX3 = (Storyboard)FindResource("X3Key");
            AnimeX1.Completed += CompletedEvent;
            AnimeX2.Completed += CompletedEvent;
            AnimeX3.Completed += CompletedEvent;
            Loaded += delegate { ViewModel = (IndexViewModel)this.DataContext; };
        }

        private void CompletedEvent(object sender, EventArgs e)
        {
            ViewModel.ChangeActive(ActiveAnime);
        }

        private void VolumeEvent(object sender, RoutedEventArgs e)
        {
            if (Vol == 0)
            {
                ((Storyboard)FindResource("VolOpenKey")).Begin();
                Vol = 1;
            }
            else
            {
                ((Storyboard)FindResource("VolCloseKey")).Begin();
                Vol = 0;
            }
        }
        private void VolChangeEvent(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var slider = (sender as Slider);
            VolumeShow.Content = (int)slider.Value + "%";
            if (ViewModel != null)
            {
                var Audio = ViewModel.AudioFactory;
                if (Audio != null && Audio.WaveOutReadOnly != null)
                {
                    Audio.ChangeVolume((float)(slider.Value / 100f));
                }
            }
        }
    }
}
