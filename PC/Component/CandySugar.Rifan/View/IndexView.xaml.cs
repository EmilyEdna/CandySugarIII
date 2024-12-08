namespace CandySugar.Rifan.View
{
    /// <summary>
    /// IndexView.xaml 的交互逻辑
    /// </summary>
    public partial class IndexView : UserControl
    {
        public int ActiveAnime = 1;
        public Storyboard AnimeX1;
        public Storyboard AnimeX2;
        public Storyboard AnimeX3;
        public Storyboard AnimeX4;
        public Storyboard AnimeX5;
        public Storyboard AnimeX6;
        private IndexViewModel ViewModel;

        public IndexView()
        {
            InitializeComponent();
            Loaded += delegate { ViewModel = (IndexViewModel)this.DataContext; };
            AnimeX1 = (Storyboard)FindResource("X1Key");
            AnimeX2 = (Storyboard)FindResource("X2Key");
            AnimeX3 = (Storyboard)FindResource("X3Key");
            AnimeX4 = (Storyboard)FindResource("X4Key");
            AnimeX5 = (Storyboard)FindResource("X5Key");
            AnimeX6 = (Storyboard)FindResource("X6Key");

            AnimeX1.Completed += CompletedEvent;
            AnimeX2.Completed += CompletedEvent;
            AnimeX3.Completed += CompletedEvent;
            AnimeX4.Completed += CompletedEvent;
            AnimeX5.Completed += CompletedEvent;
            AnimeX6.Completed += CompletedEvent;
        }

        private void CompletedEvent(object sender, EventArgs e)
        {
            ViewModel.ChangeActive(ActiveAnime);
        }
    }
}
