using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.ComponentModel;

namespace PDIWT.Resources
{
    public static class PDIWT_Helper
    {
        public static  bool EnumTextBoxHasError(DependencyObject framework)
        {
            foreach (var _item in LogicalTreeHelper.GetChildren(framework))
            {
                if (_item is DependencyObject)
                    if (((_item is TextBox) && Validation.GetHasError((TextBox)_item)) || EnumTextBoxHasError((DependencyObject)_item))
                        return true;

            }
            return false;
        }

        public static Dictionary<TEnum,string> GetEnumDescriptionDictionary<TEnum>() where TEnum : struct
        {
            var _enumDescriptionDict = new Dictionary<TEnum, string>();
            var _enumValues = Enum.GetValues(typeof(TEnum));
            foreach (var _value in _enumValues)
            {
                FieldInfo _field = typeof(TEnum).GetField(_value.ToString());
                string _description = _field.GetCustomAttribute<DescriptionAttribute>().Description;
                string _resourceStr = Localization.MainModule.Resources.ResourceManager.GetString(_description);
                _enumDescriptionDict.Add((TEnum)_value, _resourceStr);
            }
            return _enumDescriptionDict;
        }

    }
}
