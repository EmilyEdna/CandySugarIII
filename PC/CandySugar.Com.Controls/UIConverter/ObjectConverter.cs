using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CandySugar.Com.Controls.ExtenControls;
using CandySugar.Com.Controls.StructCtonrols;

namespace CandySugar.Com.Controls.UIConverter
{
    public class ObjectConverter : IValueConverter
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
