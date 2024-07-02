using CandyControls;
using CandySugar.Com.Options.ComponentGeneric;
using CandySugar.Com.Style;
using System;
using System.Drawing;
using System.IO;

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
