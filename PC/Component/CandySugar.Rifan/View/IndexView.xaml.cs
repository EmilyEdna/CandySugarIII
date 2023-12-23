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
        private Storyboard BarOpen;
        private Storyboard BarClose;
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
            BarOpen = (Storyboard)FindResource("NavListBarOpenKey");
            BarClose = (Storyboard)FindResource("NavListBarCloseKey");
            AnimeX1.Completed += CompletedEvent;
            AnimeX2.Completed += CompletedEvent;
            AnimeX3.Completed += CompletedEvent;
            AnimeX4.Completed += CompletedEvent;
            AnimeX5.Completed += CompletedEvent;
            AnimeX6.Completed += CompletedEvent;
            GenericDelegate.InformationAction = new((width, height) =>
            {
                this.Width = width;
                this.Height = height - 35 <= 0 ? 0 : height - 35;
            });
            WeakReferenceMessenger.Default.Register<MessageNotify>(this, (recip, notify) =>
            {
                BarOpen.Begin();
            });
        }

        private void CompletedEvent(object sender, EventArgs e)
        {
            ViewModel.ChangeCommand(ActiveAnime);
        }


        private void PlayClickEnvent(object sender, RoutedEventArgs e)
        {
            var Info = ((sender as CandyButton).CommandParameter as PlayInfo);
            new ScreenPlayView(Tuple.Create(Info.Route, $"{Info.Name}_{Info.Clarity}")) { Width = 1200, Height = 700 }.Show();
            BarClose.Begin();
        }
    }
}
