using MahApps.Metro.IconPacks;
using System;
using System.Globalization;

namespace WikiUpload
{
    using static UploadFileStatus;

    internal class UploadFileStatusToKindConverter : BaseMultiValueConverter<UploadFileStatusToKindConverter>
    {
        public override object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            const int statusIndex = 0;
            const int isVideoIndex = 1;

            var status = (UploadFileStatus)values[statusIndex];

            return status switch
            {
                Waiting => WaitingIcon((bool)values[isVideoIndex]),
                Uploading => PackIconFontAwesomeKind.SpinnerSolid,
                Warning => PackIconFontAwesomeKind.ExclamationTriangleSolid,
                Error => PackIconFontAwesomeKind.TimesCircleRegular,
                _ => throw new ArgumentException("Invalid UploadFileStatus", nameof(values)),
            };
        }

        private static PackIconFontAwesomeKind WaitingIcon(bool isVideo)
            => isVideo ? PackIconFontAwesomeKind.FilmSolid : PackIconFontAwesomeKind.AngleUpSolid;

        public override object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
