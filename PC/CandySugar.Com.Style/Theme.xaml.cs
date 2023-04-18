using CandySugar.Com.Library;
using CandySugar.Com.Options.ComponentObject;
using CandySugar.Com.Options.NotifyObject;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using XExten.Advance.LinqFramework;
using XExten.Advance.StaticFramework;

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
                var template = ((Setter)style.Setters.LastOrDefault()).Value as ControlTemplate;
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
            Window win = (Window)((Button)sender).TemplatedParent;
            if (win.WindowState == WindowState.Maximized)
                win.WindowState = WindowState.Normal;
            else
                win.WindowState = WindowState.Maximized;
        }
        /// <summary>
        /// 窗体关闭事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void CloseEvent(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
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
