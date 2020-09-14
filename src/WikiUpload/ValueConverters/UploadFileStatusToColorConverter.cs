using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace WikiUpload
{
    internal class UploadFileStatusToColorConverter : BaseValueConverter<UploadFileStatusToColorConverter>
    {
        private readonly Brush _waitingBrush;
        private readonly Brush _uploadingBrush;
        private readonly Brush _warningBrush;
        private readonly Brush _errorBrush;

        public UploadFileStatusToColorConverter()
        {
            _waitingBrush = (Brush)Application.Current.FindResource("UploadStatusWaitingBrush");
            _uploadingBrush = (Brush)Application.Current.FindResource("UploadStatusUploadingBrush");
            _warningBrush = (Brush)Application.Current.FindResource("UploadStatusWarningBrush");
            _errorBrush = (Brush)Application.Current.FindResource("UploadStatusErrorBrush");
        }

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((UploadFileStatus)value)
            {
                case UploadFileStatus.Waiting:
                    return _waitingBrush;
                case UploadFileStatus.Uploading:
                    return _uploadingBrush;
                case UploadFileStatus.Warning:
                    return _warningBrush;
                case UploadFileStatus.Error:
                    return _errorBrush;
                default:
                    System.Diagnostics.Debugger.Break();
                    throw new ArgumentException("Invalid UploadFileStatus", nameof(value));
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
