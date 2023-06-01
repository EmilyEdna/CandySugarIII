using CandySugar.MainUI.Resources.Styles;
using CandySugar.MainUI.ViewModels;

namespace CandySugar.MainUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Current.Resources.MergedDictionaries.Add(new LightTheme());
        }
        protected override Window CreateWindow(IActivationState activationState)
        {
            if (Windows.Count >= 1)
                return base.CreateWindow(activationState);
            else
            {
               activationState.Context.Services.GetService<INavigationService>().CreateBuilder().AddSegment<IndexViewModel>().Navigate();
                Thread.Sleep(2000);
                return base.CreateWindow(activationState);
            }
        }
    }
}