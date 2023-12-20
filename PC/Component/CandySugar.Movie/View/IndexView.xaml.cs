using CandySugar.Com.Controls.ExtenControls;
using Microsoft.Web.WebView2.Wpf;
using System.Windows.Media.Animation;

namespace CandySugar.Movie.View
{
    /// <summary>
    /// IndexView.xaml 的交互逻辑
    /// </summary>
    public partial class IndexView : UserControl
    {
        public Storyboard AnimeX1;
        public Storyboard AnimeX2;
        private Storyboard BarOpen;
        private Storyboard BarClose;
        public IndexView()
        {
            InitializeComponent();
            AnimeX1 = (Storyboard)FindResource("X1Key");
            AnimeX2 = (Storyboard)FindResource("X2Key");
            BarOpen = (Storyboard)FindResource("NavListBarOpenKey");
            BarClose = (Storyboard)FindResource("NavListBarCloseKey");
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

        private void PlayClickEnvent(object sender, RoutedEventArgs e)
        {
            new ScreenLocalWebPlayView((sender as CandyButton).CommandParameter.ToString()).Show();
            BarClose.Begin();
        }
    }
}
