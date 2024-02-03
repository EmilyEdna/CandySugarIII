using CandySugar.Com.Controls.ExtenControls;
using CandySugar.Com.Library.KeepOn;

namespace CandySugar.Com.Controls.UIExtenControls
{
    /// <summary>
    /// ScreenAudioPlayView.xaml 的交互逻辑
    /// </summary>
    public partial class ScreenAudioPlayView : CandyWindow
    {
        public ScreenAudioPlayView()
        {
            InitializeComponent();
            ScreenKeep.PreventForCurrentThread();
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            ScreenKeep.RestoreForCurrentThread();
            this.Close();
        }
    }
}
