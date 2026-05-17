namespace CandySugar.Cosplay.View
{
    /// <summary>
    /// CosplayLandView.xaml 的交互逻辑
    /// </summary>
    public partial class CosplayLandView : UserControl
    {
        public int ActiveAnime = 1;
        public Storyboard AnimeY1;
        public Storyboard AnimeY2;
        private CosplayLandViewModel ViewModel;
        public CosplayLandView()
        {
            InitializeComponent();
            Loaded += delegate { ViewModel = (CosplayLandViewModel)this.DataContext; };
            AnimeY1 = (Storyboard)FindResource("Y1Key");
            AnimeY2 = (Storyboard)FindResource("Y2Key");
            AnimeY1.Completed += CompletedEvent;
            AnimeY2.Completed += CompletedEvent;
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
