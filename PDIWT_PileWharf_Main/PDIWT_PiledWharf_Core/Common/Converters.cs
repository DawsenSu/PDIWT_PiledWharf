using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using PDIWT_PiledWharf_Core.ViewModel;
using PDIWT_PiledWharf_Core_Cpp;
using PDIWT_PiledWharf_Core.Model;
using System.Windows.Controls;

namespace PDIWT_PiledWharf_Core.Common
{
    [ValueConversion(typeof(PileTypeManaged), typeof(Visibility))]
    public class PileTypeToVisibilityConverter : IValueConverter
    {
        public string Item1 { get; set; }
        public string Item2 { get; set; }
        public string Item3 { get; set; }
        public string Item4 { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            List<string> _itemlist = new List<string> { Item1, Item2, Item3, Item4 };
            string _pileTypeDisplayName = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<PileTypeManaged>()[(PileTypeManaged)value];
            if (_itemlist.Contains(_pileTypeDisplayName))
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    [ValueConversion(typeof(PileTypeManaged), typeof(bool))]
    public class PileTypeToBooleanConverter : IValueConverter
    {
        public string Item1 { get; set; }
        public string Item2 { get; set; }
        public string Item3 { get; set; }
        public string Item4 { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            List<string> _itemlist = new List<string> { Item1, Item2, Item3, Item4 };
            string _pileTypeDisplayName = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<PileTypeManaged>()[(PileTypeManaged)value];
            if (_itemlist.Contains(_pileTypeDisplayName))
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    [ValueConversion(typeof(BearingCapacityPileTypes), typeof(Visibility))]
    public class PileWorkmanshipTypeToVisibilityConverter : IValueConverter
    {
        public string Item1 { get; set; }
        public string Item2 { get; set; }
        public string Item3 { get; set; }
        public string Item4 { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            List<string> _itemlist = new List<string> { Item1, Item2, Item3, Item4 };
            string _pileTypeDisplayName = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<BearingCapacityPileTypes>()[(BearingCapacityPileTypes)value];
            if (_itemlist.Contains(_pileTypeDisplayName))
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    [ValueConversion(typeof(BearingCapacityPileTypes), typeof(string))]
    public class PileTypesToDisplayNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var _value = (BearingCapacityPileTypes)value;
            var _dict = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<BearingCapacityPileTypes>();
            return _dict[_value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var _dict = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<BearingCapacityPileTypes>();
            var _result = _dict.Where(e => e.Value == (string)value);
            if (_result.Count() == 0)
            {
                throw new ValueUnavailableException($"Can find {value.ToString()}");
            }
            else
            {
                return _result.First();
            }
        }
    }

    [ValueConversion(typeof(int),typeof(string))]
    public class RowNumberToIndexConveter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int _row = (int)value;
            return _row == 0? "*" : _row.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }

    [ValueConversion(typeof(DesignWaterLevelCondition), typeof(string))]
    public class DesignWaterLevelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var _value = (DesignWaterLevelCondition)value;
            var _dict = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<DesignWaterLevelCondition>();
            return _dict[_value];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var _value = (string)value;
            var _dict = PDIWT.Resources.PDIWT_Helper.GetEnumDescriptionDictionary<DesignWaterLevelCondition>();
            return _dict.Where(item => item.Value == _value).First().Key;
        }
    }

    [ValueConversion(typeof(double?),typeof(string))]
    public class NullDoubleToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double? _double = (double?)value;
            if (_double == null)
                return null;
            else
                return _double.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string _str = (string)value;
            if (string.IsNullOrEmpty(_str) || string.IsNullOrWhiteSpace(_str))
                return null;
            else
            {
                if (double.TryParse(_str, out double _doubleValue))
                    return _doubleValue;
                else
                    return null;
            }
        }
    }
}
