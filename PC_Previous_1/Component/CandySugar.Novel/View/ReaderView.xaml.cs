namespace CandySugar.Novel.View
{
    /// <summary>
    /// ReaderView.xaml 的交互逻辑
    /// </summary>
    public partial class ReaderView : UserControl
    {
        public ReaderView()
        {
            InitializeComponent();
            GenericDelegate.InformationAction = new((width, height) =>
            {
                Canvas.SetTop(FloatBtn, height - 160);
                Canvas.SetLeft(FloatBtn, width - 100);
                this.Width = width;
                this.Height = height - 35 <= 0 ? 0 : height - 35;
            });
            this.Loaded += delegate
            {
                Canvas.SetTop(FloatBtn, this.Height - 125);
                Canvas.SetLeft(FloatBtn, this.Width - 100);
            };
        }
    }
}
