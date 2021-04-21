using System;
using System.Globalization;
using System.Windows;

namespace WikiUpload
{
    public class BooleanToVisiblityConverter : BaseValueConverter<BooleanToVisiblityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (parameter, (bool)value) switch
            {
                (null, true) => Visibility.Hidden,
                (null, false) => Visibility.Visible,
                (not null, true) => Visibility.Visible,
                (not null, false) => Visibility.Hidden,
            };
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
