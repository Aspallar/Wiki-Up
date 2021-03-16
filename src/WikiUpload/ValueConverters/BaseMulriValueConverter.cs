using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace WikiUpload
{
    /// <summary>
    /// A base value converter that allows direct XAML usage
    /// </summary>
    /// <typeparam name="T">The type of this value converter</typeparam>
    public abstract class BaseMultiValueConverter<T> : MarkupExtension, IMultiValueConverter
        where T : class, new()
    {

        private static T convertorInstance = null;

        public abstract object Convert(object[] values, Type targetType, object parameter, CultureInfo culture);
        public abstract object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture);
        public override object ProvideValue(IServiceProvider serviceProvider)
            => convertorInstance ?? (convertorInstance = new T());

    }
}
