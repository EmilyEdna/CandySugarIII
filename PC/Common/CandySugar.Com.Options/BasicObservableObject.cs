using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace CandySugar.Com.Options
{
    public partial class BasicObservableObject: ObservableObject
    {

        [ObservableProperty]
        private Visibility _NavVisible;
        [ObservableProperty]
        private double _NavHeight;
        [ObservableProperty]
        private double _NavWidth;
        [ObservableProperty]
        private double _BorderHeight;
        [ObservableProperty]
        private double _BorderWidth;
        [ObservableProperty]
        private int _Cols;
        [ObservableProperty]
        private Thickness _MarginThickness;
    }
}
