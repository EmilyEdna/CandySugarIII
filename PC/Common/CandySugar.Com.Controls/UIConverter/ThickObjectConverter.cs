using System;
using System.Globalization;
using System.Windows.Data;
using CandyControls;
using CandyControls.ControlsModel.Thicks;

namespace CandySugar.Com.Controls.UIConverter
{
    public class ThickObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CandyImage ci)
            {
                return new ImageThickness((int)ci.Width, (int)ci.Height);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
