using CandySugar.Com.Library.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CandySugar.Com.Controls.ExtenControls
{
    public class CandyBorder:Border
    {
        public EButton BorderType
        {
            get { return (EButton)GetValue(BorderTypeProperty); }
            set { SetValue(BorderTypeProperty, value); }
        }
        /// <summary>
        /// [Primary] [Info] [Success] [Warn] [Error]
        /// </summary>
        public static readonly DependencyProperty BorderTypeProperty =
            DependencyProperty.Register("BorderType", typeof(EButton), typeof(CandyBorder), new PropertyMetadata(EButton.Primary));
    }
}
