using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using WikiUpload.Properties;

namespace WikiUpload
{
    internal class SelectedIndexToTooltipConverter : BaseValueConverter<SelectedIndexToTooltipConverter>
    {
        private readonly string _removeAllFilesText;
        private readonly string _removeSelectedFilesText;

        public SelectedIndexToTooltipConverter()
        {
            _removeAllFilesText = Resources.RemoveAllFilesTooltip;
            _removeSelectedFilesText = Resources.RemoveSelectedFilesTooltip;
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int selectedIndex = (int)value;
            return selectedIndex == -1 ? _removeAllFilesText : _removeSelectedFilesText;
        }
        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
