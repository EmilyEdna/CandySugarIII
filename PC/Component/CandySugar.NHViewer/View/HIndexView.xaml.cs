namespace CandySugar.NHViewer.View
{
    /// <summary>
    /// HiView.xaml 的交互逻辑
    /// </summary>
    public partial class HIndexView : UserControl
    {
        public int ActiveAnime = 1;
        public Storyboard AnimeX1;
        public Storyboard AnimeX2;
        private HIndexViewModel ViewModel;
        public HIndexView()
        {
            InitializeComponent();
            AnimeX1 = (Storyboard)FindResource("X1Key");
            AnimeX2 = (Storyboard)FindResource("X2Key");
            Loaded += delegate {
                ViewModel = (HIndexViewModel)this.DataContext;
                ViewModel.Views = this;
            };
            AnimeX1.Completed += CompletedEvent;
            AnimeX2.Completed += CompletedEvent;
        }

        private void CompletedEvent(object sender, EventArgs e)
        {
            ViewModel.ChangeActive(ActiveAnime);
        }
    }
}
