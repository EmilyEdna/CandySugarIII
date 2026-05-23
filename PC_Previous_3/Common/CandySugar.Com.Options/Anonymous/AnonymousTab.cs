using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace CandySugar.Com.Options.Anonymous
{
    public partial class AnonymousTab : BasicObservableObject
    {
        public AnonymousTab()
        {
            Width = 1100d;
        }

        [ObservableProperty]
        double _Width;
        [ObservableProperty]
        string _Title;
        [ObservableProperty]
        Control _Value;
    }
}
