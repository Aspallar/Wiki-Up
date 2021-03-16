using MahApps.Metro.IconPacks;
using System;
using System.Globalization;

namespace WikiUpload
{
    internal class UploadFileStatusToKindConverter : BaseValueConverter<UploadFileStatusToKindConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var uploadFile = (UploadFile)value;
            switch (uploadFile.Status)
            {
                case UploadFileStatus.Waiting:
                    return uploadFile.IsVideo ? PackIconFontAwesomeKind.FilmSolid : PackIconFontAwesomeKind.AngleUpSolid;
                case UploadFileStatus.Uploading:
                    return PackIconFontAwesomeKind.SpinnerSolid;
                case UploadFileStatus.Warning:
                    return PackIconFontAwesomeKind.ExclamationTriangleSolid;
                case UploadFileStatus.Error:
                    return PackIconFontAwesomeKind.TimesCircleRegular;
                default:
                    System.Diagnostics.Debugger.Break();
                    throw new ArgumentException("Invalid UploadFileStatus", nameof(value));
            }
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
