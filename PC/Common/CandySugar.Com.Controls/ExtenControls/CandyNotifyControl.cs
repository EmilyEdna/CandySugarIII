﻿using CandySugar.Com.Library;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace CandySugar.Com.Controls.ExtenControls
{
    public class CandyNotifyControl : Window
    {
        private TextBlock Infos;
        public CandyNotifyControl(string msg)
        {
            CreateStyle();
            CreateUI();
            this.Loaded += WindowLoad;
            Infos.Text = msg;
        }

        #region UI
        private void CreateStyle()
        {
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowStyle = WindowStyle.None;
            this.BorderBrush = Brushes.Transparent;
            this.BorderThickness = new Thickness(0);
            this.AllowsTransparency = true;
            this.SnapsToDevicePixels = true;
            this.Width = 300;
            this.Height = 80;

            // 创建一个新的 ControlTemplate
            ControlTemplate customTemplate = new ControlTemplate(GetType());

            // 创建一个 Border 作为 ControlTemplate 的根元素
            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Border));
            border.SetValue(Border.BorderBrushProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#e63995")));
            border.SetValue(Border.BorderThicknessProperty, new Thickness(1.5));

            // 创建一个 Grid 作为 Border 的子元素
            FrameworkElementFactory grid = new FrameworkElementFactory(typeof(Grid));

            // 创建一个 Rectangle 作为 Grid 的子元素
            FrameworkElementFactory rectangle = new FrameworkElementFactory(typeof(Rectangle));
            rectangle.SetValue(Rectangle.WidthProperty, new TemplateBindingExtension(Window.WidthProperty));
            rectangle.SetValue(Rectangle.HeightProperty, new TemplateBindingExtension(Window.HeightProperty));
            rectangle.SetValue(Rectangle.FillProperty, new SolidColorBrush((Color)ColorConverter.ConvertFromString("#faf0e6")));
            rectangle.SetValue(Rectangle.OpacityProperty, 0.3);

            // 将 Rectangle 添加到 Grid 中
            grid.AppendChild(rectangle);

            // 创建一个 ContentPresenter 作为 Grid 的子元素
            FrameworkElementFactory contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetBinding(ContentPresenter.ContentProperty, new Binding("Content") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) });

            // 将 ContentPresenter 添加到 Grid 中
            grid.AppendChild(contentPresenter);

            // 将 Grid 添加到 Border 中
            border.AppendChild(grid);

            // 设置 ControlTemplate 的根元素
            customTemplate.VisualTree = border;

            this.Template = customTemplate;
        }
        private void CreateUI()
        {

            Grid GridContent = new Grid
            {
                Margin = new Thickness(3),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#40e6cfe6"))
            };

            Button Close = new Button
            {
                Margin = new Thickness(5),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                FontSize = 16,
                Foreground = Brushes.DeepSkyBlue,
                FontWeight = FontWeights.Bold,
                UseLayoutRounding = true,
                Content=FontIcon.Xmark,
            };
            Close.SetResourceReference(Button.FontFamilyProperty, "Thin");
            TextOptions.SetTextFormattingMode(Close, TextFormattingMode.Display);
            Close.Click += CloseEvent;

            Infos = new TextBlock
            {
                Margin = new Thickness(0, 15, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 15,
                FontWeight= FontWeights.Bold,
                UseLayoutRounding = true,
                Foreground = Brushes.DeepSkyBlue,
            };
            Infos.SetResourceReference(TextBlock.FontFamilyProperty, "FontStyle");
            TextOptions.SetTextFormattingMode(Infos, TextFormattingMode.Display);
            GridContent.Children.Add(Close);
            GridContent.Children.Add(Infos);
            this.Content = GridContent;
        }
        #endregion

        private void WindowLoad(object sender, RoutedEventArgs e)
        {
            Left = SystemParameters.WorkArea.Right - this.Width;
            Top = SystemParameters.WorkArea.Bottom;
            var animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.5)),
                To = SystemParameters.WorkArea.Bottom - this.Height,
            };
            this.BeginAnimation(TopProperty, animation);
        }
        private void CloseEvent(object sender, RoutedEventArgs e)
        {
            var animation = new DoubleAnimation
            {
                Duration = new Duration(TimeSpan.FromSeconds(0.3)),
                To = SystemParameters.WorkArea.Bottom,
            };
            animation.Completed += (ss, ee) =>
            {
                this.Close();
            };
            this.BeginAnimation(TopProperty, animation);
        }
    }
}