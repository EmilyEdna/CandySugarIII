namespace CandySugar.WallPaper.View
{
    /// <summary>
    /// IndexView.xaml 的交互逻辑
    /// </summary>
    public partial class Index1View : UserControl
    {
        public Storyboard AnimeX1;
        public Storyboard AnimeX2;
        public Storyboard AnimeX3;
        private Index1ViewModel ViewModel;
        public Index1View()
        {
            InitializeComponent();
            Loaded += delegate { ViewModel = (Index1ViewModel)this.DataContext; };
            AnimeX1 = (Storyboard)FindResource("X1Key");
            AnimeX2 = (Storyboard)FindResource("X2Key");
            AnimeX3 = (Storyboard)FindResource("X3Key");
            AnimeX1.Completed += CompletedEvent;
            AnimeX2.Completed += CompletedEvent;
            AnimeX3.Completed += CompletedEvent;
        }
        private void CompletedEvent(object sender, EventArgs e)
        {
            ViewModel.ChangeActive();
        }
    }
}
