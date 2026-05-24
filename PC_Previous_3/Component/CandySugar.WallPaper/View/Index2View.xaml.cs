namespace CandySugar.WallPaper.View
{
    /// <summary>
    /// IndexView.xaml 的交互逻辑
    /// </summary>
    public partial class Index2View : UserControl
    {
        public Storyboard AnimeX1;
        public Storyboard AnimeX2;
        public Storyboard AnimeX3;
        private Index2ViewModel ViewModel;
        public Index2View()
        {
            InitializeComponent();
            Loaded += delegate { ViewModel = (Index2ViewModel)this.DataContext; };
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
