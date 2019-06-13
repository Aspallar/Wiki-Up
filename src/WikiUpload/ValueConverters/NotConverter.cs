using System;
using System.Globalization;
using System.Windows;

namespace WikiUpload
{
    public class NotConverter : BaseValueConverter<NotConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !(bool)value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
