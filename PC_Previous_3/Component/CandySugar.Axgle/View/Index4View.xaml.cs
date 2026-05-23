namespace CandySugar.Axgle.View
{
    /// <summary>
    /// ExpendView.xaml 的交互逻辑
    /// </summary>
    public partial class Index4View : UserControl
    {
        private Index4ViewModel ViewModel;
        public Index4View()
        {
            InitializeComponent();
            Loaded += delegate
            {
                ViewModel = (Index4ViewModel)this.DataContext;
            };
        }
    }
}
