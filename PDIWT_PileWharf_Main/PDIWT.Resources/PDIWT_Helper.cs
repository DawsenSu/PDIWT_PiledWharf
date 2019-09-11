using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Reflection;
using System.ComponentModel;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Media;

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

        public static T GetFristVisualChild<T>(DependencyObject dependencyObject) where T : UIElement
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                DependencyObject _dpObject = VisualTreeHelper.GetChild(dependencyObject, i);
                if (_dpObject is T)
                    return (T)_dpObject;
                else
                    return GetFristVisualChild<T>(_dpObject);
            }
            return null;
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

        public static IList<T> DeepCloneList<T>(this IList<T> tList) where T : class
        {
            IList<T> listNew = new List<T>();
            foreach (var item in tList)
            {
                T model = System.Activator.CreateInstance<T>();                     //实例化一个T类型对象
                PropertyInfo[] propertyInfos = model.GetType().GetProperties();     //获取T对象的所有公共属性
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    //判断值是否为空，如果空赋值为null见else
                    if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    {
                        //如果convertsionType为nullable类，声明一个NullableConverter类，该类提供从Nullable类到基础基元类型的转换
                        NullableConverter nullableConverter = new NullableConverter(propertyInfo.PropertyType);
                        //将convertsionType转换为nullable对的基础基元类型
                        propertyInfo.SetValue(model, Convert.ChangeType(propertyInfo.GetValue(item), nullableConverter.UnderlyingType), null);
                    }
                    else
                    {
                        propertyInfo.SetValue(model, Convert.ChangeType(propertyInfo.GetValue(item), propertyInfo.PropertyType), null);
                    }
                }
                listNew.Add(model);
            }
            return listNew;
        }

        public static T DeepCopyByXml<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(typeof(T));
                xml.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                retval = xml.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }
        public static T DeepCopyByBin<T>(T obj)
        {
            object retval;
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                //序列化成流
                bf.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                //反序列化成对象
                retval = bf.Deserialize(ms);
                ms.Close();
            }
            return (T)retval;
        }
    }
}
