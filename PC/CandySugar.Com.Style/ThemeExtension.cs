using System.Reflection;
using System;
using System.Windows;
using System.ComponentModel;

namespace CandySugar.Com.Style
{
    public class ThemeExtension : ResourceDictionary
    {
        public ThemeExtension()
        {
            Application.Current.Resources.MergedDictionaries.Insert(0, new ResourceDictionary { Source = GetStyleAbsoluteUri("Theme") });
        }
        public static Uri GetStyleAbsoluteUri(string path)
        {
            return new Uri($"pack://application:,,,/CandySugar.Com.Style;component/{path}.xaml");
        }
    }
}
