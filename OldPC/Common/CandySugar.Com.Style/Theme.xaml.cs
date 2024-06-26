﻿using CandySugar.Com.Controls.UIExtenControls;
using CandySugar.Com.Library;
using CandySugar.Com.Library.VisualTree;
using CandySugar.Com.Options.NotifyObject;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Style
{
    /// <summary>
    /// Theme.xaml 的交互逻辑
    /// </summary>
    public partial class Theme : ResourceDictionary
    {
        private Stopwatch Watch;
        /// <summary>
        /// 背景轮询队列
        /// </summary>
        private ConcurrentQueue<string> BackQueue;
        public Theme()
        {
            Watch = new();
            BackQueue = new();
            CompositionTarget.Rendering += AnimetionEvent;
            Watch.Start();
        }

        /// <summary>
        /// 利用关键帧动态切换背景图(呼吸效果动效)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AnimetionEvent(object sender, EventArgs args)
        {
            if (Watch.Elapsed.Subtract(TimeSpan.Zero).TotalSeconds <= ComponentBinding.OptionObjectModels.Interval) return;
            if (ComponentBinding.OptionObjectModels.BackgroudLocation.IsNullOrEmpty()) return;
            var files = Directory.GetFiles(ComponentBinding.OptionObjectModels.BackgroudLocation);
            if (files.Length <= 0) return;
            if (BackQueue.IsEmpty) files.ForArrayEach<string>(BackQueue.Enqueue);
            ((Dispatcher)sender).Invoke(() =>
            {
                var style = this["CandyDefaultWindowStyle"] as System.Windows.Style;
                var template = style.Setters.Where(t => ((Setter)t).Value.GetType() == typeof(ControlTemplate))
                .Select(t => (Setter)t).FirstOrDefault().Value as ControlTemplate;
                var win = Application.Current.MainWindow;
                if (win.Name.Equals("CandyWindow"))
                {
                    if (template.FindName("ImageBackgroud", win) is Grid grid)
                        if (grid != null)
                        {
                            BackQueue.TryDequeue(out string file);
                            Storyboard Board = new Storyboard();
                            var Anime = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(3));
                            Storyboard.SetTarget(Anime, grid);
                            Storyboard.SetTargetProperty(Anime, new PropertyPath(Grid.OpacityProperty));
                            Board.Children.Add(Anime);
                            Board.Completed += delegate
                            {
                                grid.Background = new ImageBrush(new BitmapImage(new Uri(file)));
                                grid.BeginAnimation(Grid.OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromSeconds(3)));
                            };
                            Board.Begin();
                            Watch.Restart();
                        }
                }
            });
        }

        /// <summary>
        /// 窗体拖拽事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void MoveEvent(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                ((Window)((Border)sender).TemplatedParent).DragMove();
        }
        /// <summary>
        /// 最小化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void MinEvent(object sender, RoutedEventArgs e)
        {
            Window win = (Window)((Button)sender).TemplatedParent;
            win.WindowState = WindowState.Minimized;
        }
        /// <summary>
        /// 最大化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void MaxEvent(object sender, RoutedEventArgs e)
        {
            var Button = (Button)sender;
            Window win = (Window)Button.TemplatedParent;
            var GridContent = Button.FindParent<Grid>("GridContent");
            if (win.WindowState == WindowState.Maximized)
                win.WindowState = WindowState.Normal;
            else
            {
                ((Storyboard)GridContent.FindResource("CloseMenuBarAnimeFrame")).Begin();
                win.WindowState = WindowState.Maximized;
            }
        }
        /// <summary>
        /// 最大化事件显示导航栏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BarMenuEvent(object sender, MouseButtonEventArgs e)
        {
            var GridContent =  ((Border)sender).FindParent<Grid>("GridContent");
            var win = (Window)GridContent.TemplatedParent;
            if (win.WindowState == WindowState.Maximized)
            {
                var CloseAnime = (Storyboard)GridContent.FindResource("CloseMenuBarAnimeFrame");
                var OpenAnime = (Storyboard)GridContent.FindResource("OpenMenuBarAnimeFrame");
                OpenAnime.Begin();
                OpenAnime.Completed += delegate
                {
                    Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(_ =>
                    {
                        win.Dispatcher.BeginInvoke(() =>
                        {
                            if (win.WindowState == WindowState.Maximized)
                                CloseAnime.Begin();
                        });
                    });
                };
            }

        }
        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void CloseEvent(object sender, RoutedEventArgs e)
        {
            Window win = (Window)((Button)sender).TemplatedParent;
            if (win is ScreenPlayView || win is ScreenWebPlayView || win is ScreenLocalWebPlayView || win is ScreenAudioPlayView) win.Close();
            else
            {
                WindowCloseNofityView CloseWin = new WindowCloseNofityView();
                if (CloseWin.ShowDialog().Value)
                    Environment.Exit(0);
                else
                    win.Visibility = Visibility.Collapsed;
            }

        }
        /// <summary>
        /// 搜索事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void SearchEvent(object sender, RoutedEventArgs e)
        {
            WeakReferenceMessenger.Default.Send(new DefaultNotify { Module = EDefaultNotify.SearchNotify });
        }


    }
}
