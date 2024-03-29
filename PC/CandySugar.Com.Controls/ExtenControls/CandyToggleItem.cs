﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;

namespace CandySugar.Com.Controls.ExtenControls
{
    public class CandyToggleItem : ListBoxItem
    {

        public override void OnApplyTemplate()
        {
            this.MouseUp += ToggleItemClicked;
        }

        public Visibility UnderLine
        {
            get { return (Visibility)GetValue(UnderLineProperty); }
            set { SetValue(UnderLineProperty, value); }
        }

        public static readonly DependencyProperty UnderLineProperty =
            DependencyProperty.Register("UnderLine", typeof(Visibility), typeof(CandyToggleItem), new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// 命令参数
        /// </summary>
        public object ParentElement
        {
            get { return (object)GetValue(ParentElementProperty); }
            set { SetValue(ParentElementProperty, value); }
        }
        public static readonly DependencyProperty ParentElementProperty =
            DependencyProperty.Register("ParentElement", typeof(object), typeof(CandyToggleItem), new PropertyMetadata(default));

        private void ToggleItemClicked(object sender, MouseButtonEventArgs e)
        {
            ((CandyToggle)ParentElement)?.Command?.Execute(this);
        }
    }

    public class CandyToggleItemSetting
    {
        public double Width { get; set; }
        public Visibility UseUnderLine { get; set; }
        public object Content { get; set; }
    }
}
