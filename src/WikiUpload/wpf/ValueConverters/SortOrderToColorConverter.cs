using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace WikiUpload
{
    internal class SortOrderToColorConverter : BaseValueConverter<SortOrderToColorConverter>
    {
        private readonly Brush _defaultBrush;
        private readonly Brush _highlightBrush;

        public SortOrderToColorConverter()
        {
            _defaultBrush = (Brush)Application.Current.FindResource("ButtonTextBrush");
            _highlightBrush = (Brush)Application.Current.FindResource("ToggleOnForegroundBrush");
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sortOptipn = (SortingOptions)value;
            var highlightWhen = (SortingOptions)parameter;
            return sortOptipn == highlightWhen ? _highlightBrush : _defaultBrush;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
