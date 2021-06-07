using System.Collections.Generic;
using System.Text;
using WikiUpload.Properties;

namespace WikiUpload
{
    internal class ResponseWarnings : IReadOnlyResponseWarnings
    {
        private const string duplicateArchiveCode = "duplicate-archive";
        private const string separator = ". ";

        private static readonly Dictionary<string, string> friendlyWarnings = new Dictionary<string, string>
        {
            { "exists", Resources.UploadErrorAlreadyExists },
            { "badfilename", Resources.UploadErrorBadFilename },
            { "filetype-unwanted-type", Resources.UploadErrorUnwantedType },
            { "large-file", Resources.UploadErrorLargeFile },
            { "emptyfile", Resources.UploadErrorEmptyFile },
            { duplicateArchiveCode, Resources.UploadErrorDuplicateArchive },
            { "was-deleted", Resources.UploadErrorDeletedFile },
        };

        private readonly List<ApiError> _warnings = new List<ApiError>();
        private readonly List<string> _duplicates = new List<string>();

        public void Add(ApiError item) => _warnings.Add(item);

        public void AddDuplicate(string duplicate) => _duplicates.Add(duplicate);

        public override string ToString()
        {
            var text = new StringBuilder();

            AppendWarnings(text);

            if (_duplicates.Count > 0)
                AppendDuplicates(text);
            else
                text.RemoveLastCharacter();

            return text.ToString();
        }

        private void AppendDuplicates(StringBuilder text)
        {
            text.Append(Resources.UploadResponseDuplicateOf);
            foreach (var duplicate in _duplicates)
                text.Append(' ').AppendEnclosed(duplicate);
            text.Append('.');
        }

        private void AppendWarnings(StringBuilder text)
        {
            foreach (var warning in _warnings)
            {
                if (friendlyWarnings.TryGetValue(warning.Code, out var friendlyText))
                    AddFriendlyText(text, warning, friendlyText);
                else
                    text.Append(warning).Append(separator);
            }
        }

        private static void AddFriendlyText(StringBuilder text, ApiError warning, string friendlyText)
        {
            if (warning.Code == duplicateArchiveCode)
                text.Append(friendlyText).Append(' ').AppendEnclosed(warning.Info).Append(separator);
            else
                text.Insert(0, separator).Insert(0, friendlyText);
        }
    }
}
