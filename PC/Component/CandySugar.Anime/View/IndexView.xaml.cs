using System.Windows.Media.Animation;

namespace CandySugar.Anime.View
{
    /// <summary>
    /// IndexView.xaml 的交互逻辑
    /// </summary>
    public partial class IndexView : UserControl
    {
        private Storyboard BarOpen;
        private Storyboard BarClose;

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(int), typeof(IndexView), new PropertyMetadata(5));

        public IndexView()
        {
            InitializeComponent();
            BarOpen = (Storyboard)FindResource("NavListBarOpenKey");
            BarClose = (Storyboard)FindResource("NavListBarCloseKey");
            GenericDelegate.InformationAction = new((width, height) =>
            {
                this.Width = width;
                this.Height = height - 35 <= 0 ? 0 : height - 35;
                if (width >= SystemParameters.PrimaryScreenWidth) Columns = 8;
                else Columns = 5;
            });
            WeakReferenceMessenger.Default.Register<MessageNotify>(this, (recip, notify) =>
            {
                if ((bool)notify.ControlParam) BarOpen.Begin();
                else BarClose.Begin();
            });
        }
    }
}
