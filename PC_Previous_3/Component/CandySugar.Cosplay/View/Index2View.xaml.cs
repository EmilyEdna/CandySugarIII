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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CandySugar.Cosplay.View
{
    /// <summary>
    /// Index2View.xaml 的交互逻辑
    /// </summary>
    public partial class Index2View : UserControl
    {
        public int ActiveAnime = 1;
        public Storyboard AnimeX1;
        public Storyboard AnimeX2;
        private Index2ViewModel ViewModel;
        public Index2View()
        {
            InitializeComponent();
            Loaded += delegate { ViewModel = (Index2ViewModel)this.DataContext; };
            AnimeX1 = (Storyboard)FindResource("X1Key");
            AnimeX2 = (Storyboard)FindResource("X2Key");
            AnimeX1.Completed += CompletedEvent;
            AnimeX2.Completed += CompletedEvent;
        }
        private void CompletedEvent(object sender, EventArgs e)
        {
            ViewModel.ChangeActive(ActiveAnime);
        }
    }
}
