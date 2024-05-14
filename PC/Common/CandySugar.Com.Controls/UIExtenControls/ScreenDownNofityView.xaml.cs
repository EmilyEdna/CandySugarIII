using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// ScreenDownNofityView.xaml 的交互逻辑
    /// </summary>
    public partial class ScreenDownNofityView : Window
    {
        public ScreenDownNofityView(string Info,string Catalog)
        {
            this.Info = Info;
            this.Catalog = Catalog;
            InitializeComponent();
            Loaded += NotifyLoad;
        }

        public string Info { get; set; }
        public string Catalog { get; set; }

        private void NotifyLoad(object sender, RoutedEventArgs e)
        {
            Left = SystemParameters.WorkArea.Right - this.Width;
            Top = SystemParameters.WorkArea.Bottom;
            var animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                To = SystemParameters.WorkArea.Bottom - this.Height,
            };
            this.BeginAnimation(TopProperty, animation);
        }

        private void CloseEvent(object sender, RoutedEventArgs e)
        {
            var animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                To = SystemParameters.WorkArea.Bottom,
            };
            animation.Completed += (ss, ee) =>
            {
                this.Close();
            };
            this.BeginAnimation(TopProperty, animation);
        }

        private void OpenCloseEvent(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", Catalog);
            CloseEvent(sender, e);
        }
    }
}
