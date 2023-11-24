using CandySugar.Com.Options.ComponentGeneric;
using CommunityToolkit.Mvvm.Messaging;
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
using XExten.Advance.LinqFramework;

namespace CandySugar.WallPaper.View
{
    /// <summary>
    /// WallhavView.xaml 的交互逻辑
    /// </summary>
    public partial class WallhavView : UserControl
    {
        private int ActiveAnime = 1;
        private Storyboard AnimeX1;
        private Storyboard AnimeX2;
        private Storyboard AnimeX3;
        private Storyboard AnimeX4;
        private WallhavViewModel ViewModel;
        public WallhavView()
        {
            InitializeComponent();
            Loaded += delegate { ViewModel = (WallhavViewModel)this.DataContext; };
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
