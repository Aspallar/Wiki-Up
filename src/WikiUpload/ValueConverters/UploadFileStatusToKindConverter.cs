using MahApps.Metro.IconPacks;
using System;
using System.Globalization;

namespace WikiUpload
{
    internal class UploadFileStatusToKindConverter : BaseMultiValueConverter<UploadFileStatusToKindConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            switch ((UploadFileStatus)values[0])
            {
                case UploadFileStatus.Waiting:
                    return (bool)values[1] ? PackIconFontAwesomeKind.FilmSolid : PackIconFontAwesomeKind.AngleUpSolid;
                case UploadFileStatus.Uploading:
                case UploadFileStatus.Delaying:
                    return PackIconFontAwesomeKind.SpinnerSolid;
                case UploadFileStatus.Warning:
                    return PackIconFontAwesomeKind.ExclamationTriangleSolid;
                case UploadFileStatus.Error:
                    return PackIconFontAwesomeKind.TimesCircleRegular;
                default:
                    System.Diagnostics.Debugger.Break();
                    throw new ArgumentException("Invalid UploadFileStatus", nameof(values));
            }
        }

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
