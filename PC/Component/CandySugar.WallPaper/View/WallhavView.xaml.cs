namespace CandySugar.WallPaper.View
{
    /// <summary>
    /// WallhavView.xaml 的交互逻辑
    /// </summary>
    public partial class WallhavView : UserControl
    {
        public int ActiveAnime = 1;
        public Storyboard AnimeX1;
        public Storyboard AnimeX2;
        public Storyboard AnimeX3;
        public Storyboard AnimeX4;
        private WallhavViewModel ViewModel;
        public WallhavView()
        {
            InitializeComponent();
            Loaded += delegate { ViewModel = (WallhavViewModel)this.DataContext; };
            AnimeX1 = (Storyboard)FindResource("X1Key");
            AnimeX2 = (Storyboard)FindResource("X2Key");
            AnimeX3 = (Storyboard)FindResource("X3Key");
            AnimeX4 = (Storyboard)FindResource("X4Key");
            AnimeX1.Completed += CompletedEvent;
            AnimeX2.Completed += CompletedEvent;
            AnimeX3.Completed += CompletedEvent;
            AnimeX4.Completed += CompletedEvent;
            WeakReferenceMessenger.Default.Register<MessageNotify>(this, (recip, notify) =>
            {
                if (notify.ControlParam is Tuple<double, double> data)
                {
                    this.Width = data.Item1;
                    this.Height = data.Item2;
                }
                else
                    ViewModel.Builder.Clear();
            });
        }

        private void CompletedEvent(object sender, EventArgs e)
        {
            ViewModel.ChangeCommand(ActiveAnime);
        }
    }
}
