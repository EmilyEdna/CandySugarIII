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
using System.Windows.Shapes;

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
