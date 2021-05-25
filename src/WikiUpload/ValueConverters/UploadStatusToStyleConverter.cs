using System;
using System.Globalization;
using System.Windows;

namespace WikiUpload
{
    internal class UploadStatusToStyleConverter : BaseValueConverter<UploadStatusToStyleConverter>
    {
        private readonly Style _spin;
        private readonly Style _fontsize;

        public UploadStatusToStyleConverter()
        {
            _spin = (Style)Application.Current.FindResource("SpinningIcon");
            _fontsize = (Style)Application.Current.FindResource("FontSizeIcon");
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = (UploadFileStatus)value;
            return  status == UploadFileStatus.Uploading || status == UploadFileStatus.Delaying ? _spin : _fontsize;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
