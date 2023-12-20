using CandySugar.Com.Controls.ExtenControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            var Info = (sender as CandyButton).CommandParameter as MovieDetailElementResult;
            var Name = (this.DataContext as IndexViewModel).DetailResult.Name;
            new ScreenPlayView(Tuple.Create(Info.PlayView, $"{Name}_{Info.Address}")) { Width = 1200, Height = 700 }.Show();
            BarClose.Begin();
        }
    }
}
