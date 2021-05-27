using System.Collections.Generic;
using System.Text;
using WikiUpload.Properties;

namespace WikiUpload
{
    public class ResponseWarnings : IReadOnlyResponseWarnings
    {
        private const string duplicateArchiveCode = "duplicate-archive";

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
            const string separator = ". ";
            var text = new StringBuilder();

            foreach (var warning in _warnings)
            {
                if (friendlyWarnings.TryGetValue(warning.Code, out var friendlyText))
                {
                    if (warning.Code == duplicateArchiveCode)
                    {
                        text.Append(friendlyText);
                        text.Append($" [{warning.Info}]");
                        text.Append(separator);
                    }
                    else
                    {
                        text.Insert(0, separator);
                        text.Insert(0, friendlyText);
                    }
                }
                else
                {
                    text.Append(warning);
                    text.Append(separator);
                }
            }

            if (_duplicates.Count > 0)
            {
                text.Append(Resources.UploadResponseDuplicateOf);
                foreach (var duplicate in _duplicates)
                    text.Append($" [{duplicate}]");
                text.Append('.');
            }
            else if (text.Length > 0)
            {
                text.Length -= 1;
            }

            return text.ToString();
        }
    }
}
