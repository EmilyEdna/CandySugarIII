namespace CandySugar.Axgle.View
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
        private IndexViewModel ViewModel;
        public IndexView()
        {
            InitializeComponent();
            Loaded += delegate { ViewModel = (IndexViewModel)this.DataContext; };
            AnimeX1 = (Storyboard)FindResource("X1Key");
            AnimeX2 = (Storyboard)FindResource("X2Key");
            AnimeX3 = (Storyboard)FindResource("X3Key");
            AnimeX4 = (Storyboard)FindResource("X4Key");

            AnimeX1.Completed += CompletedEvent;
            AnimeX2.Completed += CompletedEvent;
            AnimeX3.Completed += CompletedEvent;
            AnimeX4.Completed += CompletedEvent;

            GenericDelegate.InformationAction = new((width, height) =>
            {
                Canvas.SetTop(FloatBtn, height - 160);
                Canvas.SetLeft(FloatBtn, width - 100);
                this.Width = width;
                this.Height = height - 35 <= 0 ? 0 : height - 35;
            });
        }

        private void CompletedEvent(object sender, EventArgs e)
        {
            ViewModel.ChangeCommand(ActiveAnime);
        }

        private void PopMenuEvent(object sender, RoutedEventArgs e)
        {
            PopMenu.Opened += delegate { ((Storyboard)FindResource("Overly")).Begin(); };
            PopMenu.IsOpen = true;
        }
    }
}
