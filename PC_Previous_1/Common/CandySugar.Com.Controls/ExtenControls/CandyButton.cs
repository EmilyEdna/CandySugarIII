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
    public class CandyButton : Button
    {
        static CandyButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CandyButton), new FrameworkPropertyMetadata(typeof(CandyButton)));
        }

        public EButton ButtonType
        {
            get { return (EButton)GetValue(ButtonTypeProperty); }
            set { SetValue(ButtonTypeProperty, value); }
        }
        /// <summary>
        /// [Primary] [Info] [Success] [Warn] [Error]
        /// </summary>
        public static readonly DependencyProperty ButtonTypeProperty =
            DependencyProperty.Register("ButtonType", typeof(EButton), typeof(CandyButton), new PropertyMetadata(EButton.Primary));
    }
}
