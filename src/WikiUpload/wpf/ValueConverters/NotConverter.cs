using System;
using System.Globalization;

namespace WikiUpload
{
    internal class NotConverter : BaseValueConverter<NotConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value is bool v) ? !v : true;

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
