using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using PDIWT.Resources.Localization.MainModule;

namespace PDIWT.Resources.Converters
{
    [ValueConversion(typeof(string),typeof(bool))]
    public class StringOfListToBooleanConverter : IValueConverter
    {
        public string Item1 { get; set; }
        public string Item2 { get; set; }
        public string Item3 { get; set; }
        public string Item4 { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<string> _itemlist = new List<string> { Item1, Item2, Item3, Item4 };
            if (_itemlist.Contains(value.ToString()))
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    [ValueConversion(typeof(string),typeof(Visibility))]
    public class StringOfListToVisibilityConverter : IValueConverter
    {
        public string Item1 { get; set; }
        public string Item2 { get; set; }
        public string Item3 { get; set; }
        public string Item4 { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<string> _itemlist = new List<string> { Item1, Item2, Item3, Item4 };
            if (_itemlist.Contains(value.ToString()))
                return Visibility.Visible;
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}
