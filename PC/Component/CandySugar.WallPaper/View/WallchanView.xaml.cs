using CommunityToolkit.Mvvm.Messaging;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace CandySugar.WallPaper.View
{
    /// <summary>
    /// WallhavView.xaml 的交互逻辑
    /// </summary>
    public partial class WallchanView : UserControl
    {
        private int ActiveAnime = 1;
        private Storyboard AnimeX1;
        private Storyboard AnimeX2;
        private Storyboard AnimeX3;
        private Storyboard AnimeX4;
        private WallchanViewModel ViewModel;
        public WallchanView()
        {
            InitializeComponent();
            Loaded += delegate { ViewModel = (WallchanViewModel)this.DataContext; };
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
                var param = (Tuple<double, double>)notify.ControlParam;
                this.Width = param.Item1;
                this.Height = param.Item2;
            });
        }

        private void CompletedEvent(object sender, EventArgs e)
        {
            ViewModel.ChangeCommand(ActiveAnime);
        }

        private void MouseUpChanged(object sender, MouseButtonEventArgs e)
        {
            var item = (sender as ListBoxItem);
            var CK = item.Tag.ToString().AsInt();
            if (CK == 1 && CK != ActiveAnime)
            {
                ActiveAnime = CK;
                AnimeX1.Begin();
            }
            if (CK == 2 && CK != ActiveAnime)
            {
                ActiveAnime = CK;
                AnimeX2.Begin();
            }
            if (CK == 3 && CK != ActiveAnime)
            {
                ActiveAnime = CK;
                AnimeX3.Begin();
            }
            if (CK == 4 && CK != ActiveAnime)
            {
                ActiveAnime = CK;
                AnimeX4.Begin();
            }
        }
    }
}
