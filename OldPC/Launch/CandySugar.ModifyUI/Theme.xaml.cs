using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CandySugar.ModifyUI
{
    /// <summary>
    /// Theme.xaml 的交互逻辑
    /// </summary>
    public partial class Theme : ResourceDictionary
    {
        public Theme()
        {
            InitializeComponent();
        }

        private void MoveEvent(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                ((Window)((Border)sender).TemplatedParent).DragMove();
        }
    }
}
