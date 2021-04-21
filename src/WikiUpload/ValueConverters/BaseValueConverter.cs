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
    public abstract class BaseValueConverter<T> : MarkupExtension, IValueConverter
        where T : class, new()
    {

        private static T convertorInstance = null;

        public override object ProvideValue(IServiceProvider serviceProvider)
            => convertorInstance ??= new T();

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
    }
}
