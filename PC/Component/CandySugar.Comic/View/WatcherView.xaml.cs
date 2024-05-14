namespace CandySugar.Comic.View
{
    /// <summary>
    /// WatcherView.xaml 的交互逻辑
    /// </summary>
    public partial class WatcherView : UserControl
    {
        public WatcherView()
        {
            InitializeComponent();
            GenericDelegate.InformationAction = new((width, height) =>
            {
                this.Width = width;
                this.Height = height - 35 <= 0 ? 0 : height - 35;
            });
        }
    }
}
