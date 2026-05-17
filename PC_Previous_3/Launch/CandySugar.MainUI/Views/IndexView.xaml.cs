using CandyControls;
using CandySugar.Com.Controls.ExtenControls;
using CandySugar.Com.Options;
using CandySugar.Com.Options.ComponentGeneric;
using CandySugar.Com.Style;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;

namespace CandySugar.MainUI.Views
{
    /// <summary>
    /// IndexView.xaml 的交互逻辑
    /// </summary>
    public partial class IndexView : CandyWindow
    {
        public IndexView()
        {
            InitUI();
            InitializeComponent();
            Tray.Icon = new Icon(new MemoryStream(Convert.FromBase64String(ICO.ICOBase64)));
            this.StateChanged += delegate
            {
                GlobalParam.WindowState = this.WindowState;
                if (this.WindowState == WindowState.Maximized)
                {
                    GlobalParam.MAXWidth = SystemParameters.FullPrimaryScreenWidth * .9;
                    GlobalParam.MAXHeight = (SystemParameters.FullPrimaryScreenHeight - 60) * .9;
                    GlobalParam.NavHeight = (SystemParameters.FullPrimaryScreenHeight - 100) / 1.3;
                }
                if (this.WindowState == WindowState.Normal)
                    InitUI();
                GenericDelegate.WindowAction();
            };
            this.Closing += CloseConfirmEvent;
        }

        private void CloseConfirmEvent(object sender, CancelEventArgs e)
        {
            var Result = !(new CandyNotifyControl().ShowDialog().Value);
            if (Result) this.Visibility = Visibility.Collapsed;
            e.Cancel = Result;
        }

        private void InitUI()
        {
            GlobalParam.MAXWidth = 1160;
            GlobalParam.MAXHeight = 550;
            GlobalParam.NavHeight = 500;
            GlobalParam.NavWidth = 250;
        }

        private void IconClickEvent(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Visible;
        }
    }
}
