using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace CandySugar.Com.Options
{
    public partial class BasicObservableObject: ObservableObject
    {

        private Visibility _NavVisible;
        public Visibility NavVisible
        {
            get => _NavVisible;
            set =>SetProperty(ref _NavVisible, value);
        }
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
