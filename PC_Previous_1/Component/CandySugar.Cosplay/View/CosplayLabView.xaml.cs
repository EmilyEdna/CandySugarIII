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
    /// CosplayLabView.xaml 的交互逻辑
    /// </summary>
    public partial class CosplayLabView : UserControl
    {
        public int ActiveAnime = 1;
        public Storyboard AnimeX1;
        public Storyboard AnimeX2;
        public Storyboard AnimeX3;
        private CosplayLabViewModel ViewModel;
        public CosplayLabView()
        {
            InitializeComponent();
            Loaded += delegate { ViewModel = (CosplayLabViewModel)this.DataContext; };
            AnimeX1 = (Storyboard)FindResource("X1Key");
            AnimeX2 = (Storyboard)FindResource("X2Key");
            AnimeX3 = (Storyboard)FindResource("X3Key");
            AnimeX1.Completed += CompletedEvent;
            AnimeX2.Completed += CompletedEvent;
            AnimeX3.Completed += CompletedEvent;
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
