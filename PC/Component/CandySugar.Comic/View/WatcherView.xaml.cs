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
using System.Windows.Navigation;
using System.Windows.Shapes;

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
