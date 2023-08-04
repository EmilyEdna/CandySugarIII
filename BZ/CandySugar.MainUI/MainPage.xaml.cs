using CandySugar.MainUI.Views;

namespace CandySugar.MainUI
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            SizeChanged += WindowSizeChanged;
        }

        private void WindowSizeChanged(object sender, EventArgs e)
        {
           
        }
    }
}