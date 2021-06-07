using System;
using System.Globalization;
using System.Windows;

namespace WikiUpload
{
    internal class UploadFileStatusToVisiblityConverter : BaseValueConverter<UploadFileStatusToVisiblityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
            => ((UploadFileStatus)value) == UploadFileStatus.Uploading ? Visibility.Visible : Visibility.Hidden;

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
