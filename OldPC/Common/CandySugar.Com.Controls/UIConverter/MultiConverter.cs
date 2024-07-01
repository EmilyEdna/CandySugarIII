using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace CandySugar.Com.Controls.UIConverter
{
    public class MultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Dictionary<string,object> keyValuePairs = new Dictionary<string,object>();
            for (int i = 0; i < values.Length; i++)
            {
                keyValuePairs.Add($"Key{i+1}", values[i]);
            }
            return keyValuePairs;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
