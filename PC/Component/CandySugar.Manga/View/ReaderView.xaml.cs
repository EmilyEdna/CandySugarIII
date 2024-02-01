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

namespace CandySugar.Manga.View
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
                (this.DataContext as ReaderViewModel).SetImages();
            };
        }
    }
}
