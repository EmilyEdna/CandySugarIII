using System.Windows;

namespace CandySugar.MainUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //this.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = ThemeExtension.GetStyleAbsoluteUri("Theme.xaml") });
            base.OnStartup(e);
        }
    }
}
