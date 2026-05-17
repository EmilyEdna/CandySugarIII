using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CandySugar.Com.Controls.UIConverter;

namespace CandySugar.Com.Controls.StructCtonrols
{
    [TypeConverter(typeof(ImageThickConverter))]
    public struct ImageThickness
    {
        public ImageThickness(int length)
        {
            Width = length;
            Height = length;
        }

        public ImageThickness(int w, int h)
        {
            Width = w; Height = h;
        }

        public int Width { get; set; }
        public int Height { get; set; }
    }
}
