using System.IO;
using System;
using CandyControls;
using CandySugar.Com.Style;
using System.Drawing;
using CandySugar.Com.Options;
using CandySugar.Com.Options.ComponentGeneric;

namespace CandySugar.MainUI.Views
{
    /// <summary>
    /// IndexView.xaml 的交互逻辑
    /// </summary>
    public partial class IndexView : CandyWindow
    {
        public IndexView()
        {
            InitializeComponent();
            Tray.Icon = new Icon(new MemoryStream(Convert.FromBase64String(ICO.ICOBase64)));
            this.StateChanged += delegate {
                GenericDelegate.WindowStateAction(this.WindowState);
            };
        }
    }
}
