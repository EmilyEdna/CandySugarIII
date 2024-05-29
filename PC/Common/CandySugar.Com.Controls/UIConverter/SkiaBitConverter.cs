using CandySugar.Com.Library.BitConvert;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CandySugar.Com.Controls.UIConverter
{
    public class SkiaBitConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = (new HttpClient().GetByteArrayAsync(value.ToString())).Result;
            return SkiaBitmapHelper.Bytes2Image(data, 220, 300);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
