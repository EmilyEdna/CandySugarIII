using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using XExten.Advance.LinqFramework;

namespace CandySugar.Com.Controls.UIConverter
{
    public class NullVisibilityConvterter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
                return value.AsString().IsNullOrEmpty() ? Visibility.Visible : Visibility.Collapsed;
            else
                return value != null ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
