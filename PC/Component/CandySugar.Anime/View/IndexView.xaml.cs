using System.Windows.Media.Animation;

namespace CandySugar.Anime.View
{
    /// <summary>
    /// IndexView.xaml 的交互逻辑
    /// </summary>
    public partial class IndexView : UserControl
    {
        public int ActiveAnime = 1;
        public Storyboard AnimeX1;
        public Storyboard AnimeX2;
        private IndexViewModel ViewModel;
        public IndexView()
        {
            InitializeComponent();
            AnimeX1 = (Storyboard)FindResource("X1Key");
            AnimeX2 = (Storyboard)FindResource("X2Key");
            Loaded += delegate {
                ViewModel = (IndexViewModel)this.DataContext;
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
