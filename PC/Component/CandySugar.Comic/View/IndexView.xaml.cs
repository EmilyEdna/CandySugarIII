namespace CandySugar.Comic.View
{
    /// <summary>
    /// IndexView.xaml 的交互逻辑
    /// </summary>
    public partial class IndexView : UserControl
    {
        public int ActiveAnime = 1;
        public Storyboard AnimeX1;
        public Storyboard AnimeX2;
        private Storyboard BarOpen;
        private Storyboard BarClose;
        private IndexViewModel ViewModel;
        public IndexView()
        {
            InitializeComponent();
            Loaded += delegate { ViewModel = (IndexViewModel)this.DataContext; };
            AnimeX1 = (Storyboard)FindResource("X1Key");
            AnimeX2 = (Storyboard)FindResource("X2Key");
            BarOpen = (Storyboard)FindResource("NavListBarOpenKey");
            BarClose = (Storyboard)FindResource("NavListBarCloseKey");

            AnimeX1.Completed += CompletedEvent;
            AnimeX2.Completed += CompletedEvent;

            GenericDelegate.InformationAction = new((width, height) =>
            {
                this.Width = width;
                this.Height = height - 35 <= 0 ? 0 : height - 35;
            });
            WeakReferenceMessenger.Default.Register<MessageNotify>(this, (recip, notify) =>
            {
                if (notify.NotifyType == NotifyType.Notify)
                    BarOpen.Begin();
            });
        }

        private void CompletedEvent(object sender, EventArgs e)
        {
            ViewModel.ChangeCommand(ActiveAnime);
        }
        private void ColseEvent(object sender, RoutedEventArgs e)
        {
            BarClose.Begin();
        }
    }
}
