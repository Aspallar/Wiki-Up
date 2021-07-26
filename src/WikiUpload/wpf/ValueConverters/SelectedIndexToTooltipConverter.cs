using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace WikiUpload
{
    internal class SelectedIndexToTooltipConverter : BaseValueConverter<SelectedIndexToTooltipConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int selectedIndex = (int)value;
            return selectedIndex == -1 ? "Remove ALL files from list" : "Remove selected files from list";
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
