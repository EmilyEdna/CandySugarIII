using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CandySugar.Com.Controls.ExtenControls
{
    public class CandyWindow:Window
    {
        public Visibility ShowTitle
        {
            get { return (Visibility)GetValue(ShowTitleProperty); }
            set { SetValue(ShowTitleProperty, value); }
        }
        public static readonly DependencyProperty ShowTitleProperty =
            DependencyProperty.Register("ShowTitle", typeof(Visibility), typeof(CandyWindow), new PropertyMetadata(Visibility.Visible));
        public Visibility ShowQeuryIcon
        {
            get { return (Visibility)GetValue(ShowQeuryIconProperty); }
            set { SetValue(ShowQeuryIconProperty, value); }
        }
        public static readonly DependencyProperty ShowQeuryIconProperty =
            DependencyProperty.Register("ShowQeuryIcon", typeof(Visibility), typeof(CandyWindow), new PropertyMetadata(Visibility.Visible));
        public Visibility ShowMinIcon
        {
            get { return (Visibility)GetValue(ShowMinIconProperty); }
            set { SetValue(ShowMinIconProperty, value); }
        }
        public static readonly DependencyProperty ShowMinIconProperty =
            DependencyProperty.Register("ShowMinIcon", typeof(Visibility), typeof(CandyWindow), new PropertyMetadata(Visibility.Visible));
        public Visibility ShowMaxIcon
        {
            get { return (Visibility)GetValue(ShowMaxIconProperty); }
            set { SetValue(ShowMaxIconProperty, value); }
        }
        public static readonly DependencyProperty ShowMaxIconProperty =
            DependencyProperty.Register("ShowMaxIcon", typeof(Visibility), typeof(CandyWindow), new PropertyMetadata(Visibility.Visible));
        public Visibility ShowCloseIcon
        {
            get { return (Visibility)GetValue(ShowCloseIconProperty); }
            set { SetValue(ShowCloseIconProperty, value); }
        }
        public static readonly DependencyProperty ShowCloseIconProperty =
            DependencyProperty.Register("ShowCloseIcon", typeof(Visibility), typeof(CandyWindow), new PropertyMetadata(Visibility.Visible));
        public Visibility ShowBackgroud
        {
            get { return (Visibility)GetValue(ShowBackgroudProperty); }
            set { SetValue(ShowBackgroudProperty, value); }
        }
        public static readonly DependencyProperty ShowBackgroudProperty =
            DependencyProperty.Register("ShowBackgroud", typeof(Visibility), typeof(CandyWindow), new PropertyMetadata(Visibility.Visible));
    }
}
