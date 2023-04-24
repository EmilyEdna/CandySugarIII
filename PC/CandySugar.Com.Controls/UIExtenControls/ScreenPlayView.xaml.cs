using CandySugar.Com.Options.ComponentGeneric;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CandySugar.Com.Controls.UIExtenControls
{
    /// <summary>
    /// ScreenPlayView.xaml 的交互逻辑
    /// </summary>
    public partial class ScreenPlayView : Window
    {
        public ScreenPlayView()
        {
            InitializeComponent();
            StateChanged += Window_Stated;
            RelyLocation();
        }

        private void Window_Stated(object sender, EventArgs e)
        {
            RelyLocation();
        }
        public void RelyLocation()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.Height = SystemParameters.PrimaryScreenHeight;
                this.Width = SystemParameters.PrimaryScreenWidth;
            }
            if (this.WindowState == WindowState.Normal)
            {
                this.Height = 700;
                this.Width = 1200;
            }
            PlayBar.Width = this.Width;
        }
    }
}
