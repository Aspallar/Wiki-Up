using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using WikiUpload.Properties;

namespace WikiUpload
{
    internal class WikiFileTitleValidationRule : ValidationRule
    {
        // Reference: https://www.mediawiki.org/wiki/Manual:Page_title
        // in addition to the above rules, file names cannot contain path characters :/\

        private static readonly char[] invalidCharacters = { '#', '<', '>', '[', ']', '|', '{', '}', ':', '/', '\\' };
        private static readonly Regex urlEascapeSequence = new Regex(@"%[a-fA-F0-9]{2}");
        private static readonly Regex consecutiveWhitespace = new Regex(@"[ _]{2}");

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var title = (string)value;

            if (title == string.Empty)
                return ValidationResult.ValidResult;

            if (title.IndexOfAny(invalidCharacters) != -1)
            {
                
                var invalidCharactersString = string.Join(" ", invalidCharacters);
                return new ValidationResult(false, string.Format(Resources.EditUploadFileNameErrorInvalidCharacters, invalidCharactersString));
            }

            if (title == "." || title == "..")
                return new ValidationResult(false, Resources.EditUploadFileNameErrorIsRelativePath);

            if (title.StartsWith(" ") || title.StartsWith("_"))
                return new ValidationResult(false, Resources.EditUploadFileNameErrorWhitespaceAtStart);

            if (title.EndsWith(" ") || title.EndsWith("_"))
                return new ValidationResult(false, Resources.EditUploadFileNameErrorWhitespaceAtEnd);

            if (title.IndexOf("~~~") != -1)
                return new ValidationResult(false, Resources.EditUploadFileNameErrorThreeTildes);

            if (consecutiveWhitespace.IsMatch(title))
                return new ValidationResult(false, Resources.EditUploadFileNameErrorConsecutiveSpaces);

            if (urlEascapeSequence.IsMatch(title))
                return new ValidationResult(false, Resources.EditUploadFileNameErrorUrlEscape);

            if (Encoding.UTF8.GetByteCount(title) > 255)
                return new ValidationResult(false, Resources.EditUploadFileNameErrorTooLong);

            var lastPeriodPosition = title.LastIndexOf('.');

            if (lastPeriodPosition == -1 || lastPeriodPosition == title.Length - 1)
                return new ValidationResult(false, Resources.EditUploadFileNameErrorMustHaveExension);

            if (lastPeriodPosition == 0)
                return new ValidationResult(false, Resources.EditUploadFileNameErrorMustHaveFileName);

            return ValidationResult.ValidResult;
        }
    }
}
