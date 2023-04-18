using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CandySugar.Com.Controls.ExtenControls
{
    public class CandyListboxItem : ListBoxItem
    {
        public bool UnderLine
        {
            get { return (bool)GetValue(UnderLineProperty); }
            set { SetValue(UnderLineProperty, value); }
        }

        public static readonly DependencyProperty UnderLineProperty =
            DependencyProperty.Register("UnderLine", typeof(bool), typeof(CandyListboxItem), new PropertyMetadata(true));
    }
}
