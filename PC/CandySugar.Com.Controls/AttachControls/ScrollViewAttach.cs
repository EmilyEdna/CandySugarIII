using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using CandySugar.Com.Library.Enums;
using CandySugar.Com.Library.VisualTree;

namespace CandySugar.Com.Controls.AttachControls
{
    public static class ScrollViewAttach
    {
        public static void SetPressCommand(DependencyObject target, EDirection value)
        {
            target.SetValue(PressCommandProperty, value);
        }

        public static EDirection GetPressCommand(DependencyObject target)
        {
            return (EDirection)target.GetValue(PressCommandProperty);
        }

        public static DependencyProperty PressCommandProperty =
         DependencyProperty.RegisterAttached("PressCommand", typeof(EDirection), typeof(ScrollViewAttach), new PropertyMetadata(EDirection.None, CommandChanged));

        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ListBox element = target as ListBox;
            if (element != null)
            {
                element.Focus();
                element.KeyDown += PressEvent;
            }
        }

        private static void PressEvent(object sender, KeyEventArgs e)
        {
            ListBox element = sender as ListBox;
            EDirection press = (EDirection)element.GetValue(PressCommandProperty);
            ScrollViewer scrollViewer = element.FindChildren<ScrollViewer>().FirstOrDefault();
            if (e.Key == Key.Up && (press == EDirection.Top || press == EDirection.Arrow))
                scrollViewer.ScrollToVerticalOffset(-5d);
            if (e.Key == Key.Down && (press == EDirection.Bottom || press == EDirection.Arrow))
            {
                scrollViewer.ScrollToVerticalOffset(5d);
               if(scrollViewer.VerticalOffset + scrollViewer.ViewportHeight >= scrollViewer.ExtentHeight)
                    scrollViewer.ScrollToEnd();
            }
        }
    }
}
