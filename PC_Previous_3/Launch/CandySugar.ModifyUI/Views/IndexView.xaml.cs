using System.Windows;
using System.Windows.Media.Animation;

namespace CandySugar.ModifyUI.Views
{
    /// <summary>
    /// IndexView.xaml 的交互逻辑
    /// </summary>
    public partial class IndexView : Window
    {
        public IndexView()
        {
            InitializeComponent();
            ((Storyboard)this.FindResource("AnimeKey")).Begin();
        }
    }
}
