using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using WikiUpload.Properties;

namespace WikiUpload
{
    public class UploadResponse : IUploadResponse
    {
        private readonly List<string> _warnings;
        private readonly List<ApiError> _errors;

        private static Dictionary<string, string> friendlyWarnings;

        public UploadResponse() { }

        private const string duplicateArchiveCode = "duplicate-archive";
        public static void Initialize()
        {
            friendlyWarnings = new Dictionary<string, string>
            {
                { "exists", Resources.UploadErrorAlreadyExists },
                { "badfilename", Resources.UploadErrorBadFilename },
                { "filetype-unwanted-type", Resources.UploadErrorUnwantedType },
                { "large-file", Resources.UploadErrorLargeFile },
                { "emptyfile", Resources.UploadErrorEmptyFile },
                { duplicateArchiveCode, Resources.UploadErrorDuplicateArchive },
                { "was-deleted", Resources.UploadErrorDeletedFile },
            };
        }

        public UploadResponse(string xml, string retryAfter)
        {
            _warnings = new List<string>();
            _errors = new List<ApiError>();

            Xml = xml;
            Duplicates = new List<string>();

            if (retryAfter != "")
            {
                Result = ResponseCodes.MaxlagThrottle;
                _ = int.TryParse(retryAfter, out var retryValue);
                RetryDelay = Math.Max(5, retryValue);
            }
            else
            {
                var doc = new XmlDocument();
                doc.LoadXml(xml);

                var upload = doc.SelectSingleNode("/api/upload");
                if (upload != null)
                {
                    Result = upload.Attributes["result"].Value;

                    if (Result == ResponseCodes.Warning)
                    {
                        var warnings = upload.SelectSingleNode("warnings");
                        foreach (XmlNode attribute in warnings.Attributes)
                            _warnings.Add(attribute.Name);

                        if (warnings.Attributes[duplicateArchiveCode] != null)
                            ArchiveDuplicate = warnings.Attributes[duplicateArchiveCode].Value;

                        var duplicates = warnings.SelectNodes("duplicate/duplicate");
                        foreach (XmlNode node in duplicates)
                            Duplicates.Add(node.InnerText);
                    }
                }
                else
                {
                    Result = ResponseCodes.NoResult;
                }

                var errors = doc.SelectNodes("/api/error");
                foreach (XmlNode node in errors)
                    _errors.Add(new ApiError(node.Attributes["code"]?.Value, node.Attributes["info"]?.Value));

            }
        }

        public UploadResponse(IngestionControllerResponse response)
        {
            if (response.Success)
            {
                Result = ResponseCodes.Success;
            }
            else
            {
                Result = ResponseCodes.NoResult;
                _errors.Add(new ApiError("videeo-upload", response.Status));
            }
        }

        public IReadOnlyList<string> Warnings => _warnings;

        public IReadOnlyList<ApiError> Errors => _errors;

        public string Xml { get; private set; }

        public string Result { get; private set; }

        public string ArchiveDuplicate { get; private set; }

        public List<string> Duplicates { get; private set; }

        public bool IsDuplicate => Duplicates.Count > 0;

        public bool IsDuplicateOfArchive => !string.IsNullOrEmpty(ArchiveDuplicate);

        public bool IsError => _errors.Count > 0;

        public int RetryDelay { get; private set; }

        public string WarningsText
        {
            get
            {
                const string separator = ". ";
                var text = new StringBuilder();

                foreach (var warning in _warnings)
                {
                    if (friendlyWarnings.TryGetValue(warning, out var friendlyText))
                    {
                        if (warning == duplicateArchiveCode)
                        {
                            text.Append(friendlyText);
                            text.Append($" [{ArchiveDuplicate}]");
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

                if (IsDuplicate)
                {
                    text.Append(Resources.UploadResponseDuplicateOf);
                    foreach (var duplicate in Duplicates)
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

        public string ErrorsText
        {
            get
            {
                var text = new StringBuilder();
                foreach (var error in _errors)
                {
                    text.Append('[');
                    text.Append(error.Code);
                    text.Append("] ");
                    text.Append(error.Info);
                    text.Append(' ');
                }
                if (text.Length > 0)
                    text.Length -= 1;
                return text.ToString();
            }
        }

        public bool IsTokenError => _errors.Count == 1 && _errors[0].Code == "badtoken";
        
        public bool IsMutsBeLoggedInError => _errors.Count == 1 && _errors[0].Code == "mustbeloggedin";

    }
}
