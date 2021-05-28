using System;
using System.Globalization;

namespace WikiUpload
{
    internal class RememberPasswordOptionsToIntConverter : BaseValueConverter<RememberPasswordOptionsToIntConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (int)value;

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => (RememberPasswordOptions)value;
    }
}
