using System;
using System.Globalization;
using System.Windows;

namespace WikiUpload
{
    internal class StringToVisiblityConverter : BaseValueConverter<StringToVisiblityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var shouldBeVisible = value is string str && str.Length > 0;
            return shouldBeVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
