using System;
using System.Globalization;
using System.Windows;

namespace WikiUpload
{
    internal class BooleanToVisiblityConverter : BaseValueConverter<BooleanToVisiblityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var shoHide = (value is bool v) ? v : false;
            if (parameter == null)
                return shoHide ? Visibility.Hidden : Visibility.Visible;
            else
                return shoHide ? Visibility.Visible : Visibility.Hidden;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
