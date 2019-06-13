using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace WikiUpload
{
    public class UploadResponse
    {
        private readonly List<string> _warnings;
        private readonly List<ApiError> _errors;

        private readonly Dictionary<string, string> friendlyWarnings = new Dictionary<string, string>
        {
            { "exists", "Already exists" },
            { "badfilename", "Invalid file name" },
            { "filetype-unwanted-type", "Unwanted file type" },
            { "large-file", "Large file warning" },
            { "emptyfile", "File is empty" },
            { "duplicate-archive", "Duplicate of archived" },
            { "was-deleted", "Deleted file" },
        };

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

        public UploadResponse(string xml, string retryAfter)
        {
            _warnings = new List<string>();
            _errors = new List<ApiError>();

            Xml = xml;
            Duplicates = new List<string>();

            if (retryAfter != "")
            {
                Result = ResponseCodes.MaxlagThrottle;
                _ = int.TryParse(retryAfter, out int retryValue);
                RetryDelay = Math.Max(5, retryValue);
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                XmlNode upload = doc.SelectSingleNode("/api/upload");
                if (upload != null)
                {
                    Result = upload.Attributes["result"].Value;

                    if (Result == ResponseCodes.Warning)
                    {
                        XmlNode warnings = upload.SelectSingleNode("warnings");
                        foreach (XmlNode attribute in warnings.Attributes)
                            _warnings.Add(attribute.Name);

                        if (warnings.Attributes["duplicate-archive"] != null)
                            ArchiveDuplicate = warnings.Attributes["duplicate-archive"].Value;

                        XmlNodeList duplicates = warnings.SelectNodes("duplicate/duplicate");
                        foreach (XmlNode node in duplicates)
                            Duplicates.Add(node.InnerText);
                    }
                }
                else
                {
                    Result = ResponseCodes.NoResult;
                }

                XmlNodeList errors = doc.SelectNodes("/api/error");
                foreach (XmlNode node in errors)
                    _errors.Add(new ApiError(node.Attributes["code"]?.Value, node.Attributes["info"]?.Value));

            }
        }

        public string WarningsText
        {
            get
            {
                StringBuilder text = new StringBuilder();

                foreach (var warning in _warnings)
                {
                    if (friendlyWarnings.TryGetValue(warning, out string friendlyText))
                        text.Append(friendlyText);
                    else
                        text.Append(warning);
                    if (warning == "duplicate-archive")
                        text.Append($" [{ArchiveDuplicate}]");
                    text.Append(". ");
                }

                if (IsDuplicate)
                {
                    text.Append("Duplicate of");
                    foreach (string duplicate in Duplicates)
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
                foreach (ApiError error in _errors)
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
    }
}
