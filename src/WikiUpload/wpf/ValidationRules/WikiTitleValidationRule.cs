using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WikiUpload
{
    internal class WikiTitleValidationRule : ValidationRule
    {
        // Reference: https://www.mediawiki.org/wiki/Manual:Page_title

        private static char[] InvalidCharacters = {'#', '<', '>', '[', ']', '|', '{', '}', ':' };
        private static Regex urlEascapeSequence = new Regex(@"%[a-fA-F0-9]{2}");

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var title = (string)value;

            if (title.IndexOfAny(InvalidCharacters) != -1)
                return new ValidationResult(false, "Cannot contain # < > [ ] | { } :");

            if (title == "." || title == "..")
                return new ValidationResult(false, "Cannot be \".\" or \"..\"");

            if (title.StartsWith("./") || title.StartsWith("../"))
                return new ValidationResult(false, "Cannot start with \"./'\" or \"../'\"");

            if (title.EndsWith("/,") || title.EndsWith("/.."))
                return new ValidationResult(false, "Cannot end with \"/.'\" or \"/'..'\"");

            if (title.StartsWith(" ") || title.StartsWith("_"))
                return new ValidationResult(false, "Cannot start with a space or _");

            if (title.EndsWith(" ") || title.EndsWith("_"))
                return new ValidationResult(false, "Cannot end with a space or _");

            if (title.IndexOf("~~~") != -1)
                return new ValidationResult(false, "Cannot contain 3 or more consecutive tildes (~).");

            if (title.Replace('_', ' ').IndexOf("  ") != -1)
                return new ValidationResult(false, "Cannot contain 2 or more consecutive spaces/underscores.");

            if (urlEascapeSequence.IsMatch(title))
                return new ValidationResult(false, "Cannot contain url escape sequence (%nn).");

            return ValidationResult.ValidResult;
        }
    }
}
