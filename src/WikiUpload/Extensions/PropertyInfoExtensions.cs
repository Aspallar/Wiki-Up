using System;
using System.Configuration;
using System.Reflection;

namespace WikiUpload
{
    internal static class PropertyInfoExtensions
    {
        public static object GetDefaultValue(this PropertyInfo property)
        {
            var attribute = (DefaultSettingValueAttribute)property.GetCustomAttributes(typeof(DefaultSettingValueAttribute), false)[0];
            return Convert.ChangeType(attribute.Value, property.PropertyType);
        }
    }
}
