using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CandySugar.Com.Controls.AttachControls
{
    public class ExtenAttach
    {
        public static bool GetVisibility(DependencyObject obj)
        {
            return (bool)obj.GetValue(VisibilityProperty);
        }
        public static void SetVisibility(DependencyObject obj, Visibility value)
        {
            obj.SetValue(VisibilityProperty, value);
        }

        public static readonly DependencyProperty VisibilityProperty =
            DependencyProperty.RegisterAttached("Visibility", typeof(bool), typeof(ExtenAttach), new PropertyMetadata(true, new PropertyChangedCallback(OnVisibilityChanged)));

        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var ctrl = (FrameworkElement)d;
            ctrl.Visibility = GetVisibility(ctrl) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
