using CandySugar.Com.Library.KeepOn;
using Stylet;
using System;
using System.Windows;

namespace CandySugar.Com.Controls.UIExtenControls
{
    /// <summary>
    /// ScreenWebPlayView.xaml 的交互逻辑
    /// </summary>
    public partial class ScreenWebPlayView : Window
    {
        public ScreenWebPlayView()
        {
            InitializeComponent();
            ScreenKeep.PreventForCurrentThread(); 
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            ScreenKeep.RestoreForCurrentThread();
            this.WebPlayer.Dispose();
            this.Close();
        }
    }

    public class ScreenWebPlayViewModel : PropertyChangedBase
    {
        private string _Route;
        public string Route
        {
            get => _Route;
            set => SetAndNotify(ref _Route, value);
        }
    }
}
