using CandySugar.Com.Library.VisualTree;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CandySugar.Com.Controls.ExtenControls
{
    public class CandyListBox : ListBox
    {
        public override void OnApplyTemplate()
        {
            ScrollViewer scrollViewer = this.FindChildren<ScrollViewer>().FirstOrDefault();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled;
            this.ItemContainerStyle = (Style)Application.Current.Resources["CandyListBoxItemStyle"];
            this.BorderThickness = new Thickness(0);
            this.Background = Brushes.Transparent;
            FrameworkElementFactory stackFactory = new FrameworkElementFactory(typeof(StackPanel));
            stackFactory.SetValue(MarginProperty, new Thickness(5));
            stackFactory.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
            this.ItemsPanel = new ItemsPanelTemplate(stackFactory);
            base.OnApplyTemplate();
        }
    }
}
