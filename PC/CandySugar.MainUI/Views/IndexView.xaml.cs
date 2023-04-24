using CandySugar.Com.Controls.UIExtenControls;
using CandySugar.Com.Library.HotKey;
using CandySugar.Com.Options.ComponentGeneric;
using CandySugar.Com.Options.NotifyObject;
using CandySugar.Com.Style;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Animation;

namespace CandySugar.MainUI.Views
{
    /// <summary>
    /// IndexView.xaml 的交互逻辑
    /// </summary>
    public partial class IndexView : Window
    {
        private HotKeyAction _HotKey;
        public IndexView()
        {
            InitializeComponent();
            RelyLocation();
            _HotKey = new HotKeyAction();
            Loaded += Window_Loaded;
            StateChanged += Window_Stated;
            Tray.Icon = new System.Drawing.Icon(new MemoryStream(Convert.FromBase64String(ICO.ICOBase64)));
            new ScreenPlayView().Show();
        }

        private void Window_Stated(object sender, EventArgs e)
        {
            RelyLocation();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _HotKey.RegistHotKey();
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _HotKey.SetHwnd(this);
        }
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            _HotKey.InitHotKey();
            //自定义搜索框的位置
            PopBox.CustomPopupPlacementCallback = new((popupSize, targetSize, offset) =>
            {
                Point point = new(0, 0);
                if (WindowState == WindowState.Maximized)
                    point = new Point(targetSize.Width / 2.4, targetSize.Height / 10);
                if (this.WindowState == WindowState.Normal)
                    point = new Point(targetSize.Width / 2.5, targetSize.Height / 10);
                CustomPopupPlacement placement = new(point, PopupPrimaryAxis.None);
                return new CustomPopupPlacement[] { placement };
            });
            WeakReferenceMessenger.Default.Register<DefaultNotify>(this, (recip, notify) =>
            {
                if (notify.Module == EDefaultNotify.SearchNotify)
                {
                    PopBox.IsOpen = true;
                }
            });
        }

        /// <summary>
        /// 动态转换浮动按钮的位置
        /// </summary>
        public void RelyLocation()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.Height = SystemParameters.PrimaryScreenHeight;
                this.Width = SystemParameters.PrimaryScreenWidth;
            }
            if (this.WindowState == WindowState.Normal)
            {
                this.Height = 700;
                this.Width = 1200;
            }
            Canvas.SetTop(FloatBtn, this.Height - 100);
            Canvas.SetLeft(FloatBtn, this.Width - 100);
            GenericDelegate.InformationAction?.Invoke(this.Width, this.Height);
        }

        private void PopMenuEvent(object sender, RoutedEventArgs e)
        {
            PopMenu.Opened += delegate { ((Storyboard)FindResource("Overly")).Begin(); };
            PopMenu.IsOpen = true;
        }
    }
}
