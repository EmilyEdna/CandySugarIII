using System.Windows;

namespace CandySugar.Com.Controls.UIExtenControls
{
    /// <summary>
    /// WindowCloseNofityView.xaml 的交互逻辑
    /// </summary>
    public partial class WindowCloseNofityView : Window
    {
        public WindowCloseNofityView()
        {
            InitializeComponent();
            
        }

        private void OpenCloseEvent(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void CloseEvent(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();

        }
    }
}
