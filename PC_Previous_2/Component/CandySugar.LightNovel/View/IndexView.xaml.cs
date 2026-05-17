namespace CandySugar.LightNovel.View
{
    /// <summary>
    /// IndexView.xaml 的交互逻辑
    /// </summary>
    public partial class IndexView : UserControl
    {

        public IndexView()
        {
            InitializeComponent();
            Loaded += delegate
            {
                ((IndexViewModel)this.DataContext).Views = this;
            };
        }
    }
}
