﻿namespace CandySugar.Rifan.View
{
    /// <summary>
    /// IndexView.xaml 的交互逻辑
    /// </summary>
    public partial class IndexView : UserControl
    {

        private int ActiveAnime = 1;
        private Storyboard AnimeX1;
        private Storyboard AnimeX2;
        private Storyboard AnimeX3;
        private Storyboard AnimeX4;
        private Storyboard AnimeX5;
        private Storyboard AnimeX6;
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
            if (CK == 5 && CK != ActiveAnime)
            {
                ActiveAnime = CK;
                AnimeX5.Begin();
            }
            if (CK == 6 && CK != ActiveAnime)
            {
                ActiveAnime = CK;
                AnimeX6.Begin();
            }
        }

        private void PlayClickEnvent(object sender, RoutedEventArgs e)
        {
            var Info = ((sender as CandyButton).CommandParameter as PlayInfo);
            new ScreenPlayView(Tuple.Create(Info.Route, $"{Info.Name}_{Info.Clarity}")) { Width = 1200, Height = 700 }.Show();
            BarClose.Begin();
        }
    }
}