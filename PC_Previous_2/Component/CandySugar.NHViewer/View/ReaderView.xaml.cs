namespace CandySugar.NHViewer.View
{
    /// <summary>
    /// ReaderView.xaml 的交互逻辑
    /// </summary>
    public partial class ReaderView : UserControl
    {
        public ReaderView()
        {
            InitializeComponent();
            Loaded += delegate
            {
                ((ReaderViewModel)this.DataContext).Views = this;
            };
        }
    }
}
