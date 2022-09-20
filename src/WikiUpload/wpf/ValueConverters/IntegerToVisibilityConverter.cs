using System;
using System.Globalization;
using System.Windows;

namespace WikiUpload
{
    internal class IntegerToVisibilityConverter : BaseValueConverter<IntegerToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var intValue = value as int?;
            return intValue.HasValue && intValue.Value > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
