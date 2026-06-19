namespace CandySugar.Axgle.View
{
    /// <summary>
    /// ExpendView.xaml 的交互逻辑
    /// </summary>
    public partial class Index2View : UserControl
    {
        private Index2ViewModel ViewModel;
        public Index2View()
        {
            InitializeComponent();
            Loaded += delegate
            {
                ViewModel = (Index2ViewModel)this.DataContext;
            };
        }
    }
}
