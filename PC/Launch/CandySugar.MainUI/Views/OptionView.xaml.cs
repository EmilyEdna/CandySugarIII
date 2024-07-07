using CandyControls;
using CandySugar.Com.Options.ComponentGeneric;
using System.Windows;

namespace CandySugar.MainUI.Views
{
    /// <summary>
    /// OptionView.xaml 的交互逻辑
    /// </summary>
    public partial class OptionView : CandyWindow
    {
        public OptionView()
        {
            InitializeComponent();
        }

        private void BlurEffectEvent(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            GenericDelegate.BlurChangedAction(e.NewValue);
        }
    }
}
