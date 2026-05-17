using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml.Linq;
using CandySugar.Com.Library.Enums;
using CandySugar.Com.Library.VisualTree;
using XExten.Advance.ThreadFramework;

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
                ThreadFactory.Instance.StartWithRestart(() =>
                {
                    element.Dispatcher.Invoke(() =>
                    {
                        if (element.Items.Count > 0)
                        {
                            element.Focus();
                            element.KeyDown += PressEvent;
                            ThreadFactory.Instance.StopTask("绑定事件");
                        }
                    });
                }, "绑定事件", null, false);
            }
        }

        private static void PressEvent(object sender, KeyEventArgs e)
        {
            ListBox element = sender as ListBox;
            EDirection press = (EDirection)element.GetValue(PressCommandProperty);

            ScrollViewer scrollViewer = element.FindChildren<ScrollViewer>().FirstOrDefault();
            if (e.Key == Key.Up && press == EDirection.UpDown)
                scrollViewer.ScrollToVerticalOffset(-5d);
            if (e.Key == Key.Down && press == EDirection.UpDown)
            {
                scrollViewer.ScrollToVerticalOffset(5d);
                if (scrollViewer.VerticalOffset + scrollViewer.ViewportHeight >= scrollViewer.ExtentHeight)
                    scrollViewer.ScrollToEnd();
            }
            if (e.Key == Key.Left) Around(element, -1);
            if (e.Key == Key.Right) Around(element, 1);
            if (e.Key == Key.Escape) Around(element, 0);
        }

        private static void Around(ListBox element,int param)
        {
            var uc = element.FindParent<UserControl>();
            if (uc.DataContext != null)
            {
                var method = uc.DataContext.GetType().GetMethod("KeyHandler");
                method?.Invoke(element.DataContext, new object[] { param });
            }
        }
    }
}
