using CandySugar.Com.Pages;
using CandySugar.MainUI.Resources.Styles;

namespace CandySugar.MainUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Current.Resources.MergedDictionaries.Add(new DarkTheme());
            Current.Resources.MergedDictionaries.Add(new ControlStyle());
            MainPage = new IndexView();
        }
    }
}