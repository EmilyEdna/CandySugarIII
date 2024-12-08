using CandyControls;
using CandyControls.ControlsModel.Thicks;
using System;
using System.Globalization;
using System.Windows.Data;

namespace CandySugar.Com.Controls.UIConverter
{
    public class ThickObjectConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CandyImage ci)
            {
                return new WidthHeightStruct((int)ci.Width, (int)ci.Height);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
