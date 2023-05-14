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

namespace CandySugar.Comic.View
{
    /// <summary>
    /// IndexView.xaml 的交互逻辑
    /// </summary>
    public partial class IndexView : UserControl
    {
        private int ActiveAnime = 1;
        private Storyboard AnimeX1;
        private Storyboard AnimeX2;
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
        }


    }
}
