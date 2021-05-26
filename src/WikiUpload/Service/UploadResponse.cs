using System;
using System.Xml;

namespace WikiUpload
{
    public class UploadResponse : IUploadResponse
    {
        private readonly ResponseWarnings _warnings;
        private readonly ResponseErrors _errors;

        public UploadResponse(string xml, string retryAfter)
        {
            _warnings = new ResponseWarnings();
            _errors = new ResponseErrors();

            if (retryAfter == "")
                ParseResponse(xml);
            else
                MaglagResponse(retryAfter);
        }

        private void MaglagResponse(string retryAfter)
        {
            Result = ResponseCodes.MaxlagThrottle;
            _ = int.TryParse(retryAfter, out var retryValue);
            RetryDelay = Math.Max(5, retryValue);
        }

        private void ParseResponse(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var upload = doc.SelectSingleNode("/api/upload");
            if (upload != null)
            {
                Result = upload.Attributes["result"].Value;
                if (Result == ResponseCodes.Warning)
                    ParseWarnings(upload);
            }
            else
            {
                Result = ResponseCodes.NoResult;
            }

            ParseErrors(doc);
        }

        private void ParseErrors(XmlDocument doc)
        {
            var errors = doc.SelectNodes("/api/errors/error");
            if (errors.Count > 0)
            {
                foreach (XmlNode node in errors)
                {
                    var code = node.Attributes["code"].Value;
                    var info = node.SelectSingleNode("text")?.InnerText;
                    _errors.Add(new ApiError(code, info));
                }
            }
            else
            {
                // Legacy error format
                errors = doc.SelectNodes("/api/error");
                foreach (XmlNode node in errors)
                    _errors.Add(new ApiError(node.Attributes["code"].Value, node.Attributes["info"]?.Value));
            }
        }

        private void ParseWarnings(XmlNode uploadNode)
        {
            var warnings = uploadNode.SelectSingleNode("warnings");
            foreach (XmlNode attribute in warnings.Attributes)
                _warnings.Add(new ApiError(attribute.Name, attribute.Value));

            var duplicates = warnings.SelectNodes("duplicate/duplicate");
            foreach (XmlNode node in duplicates)
                _warnings.AddDuplicate(node.InnerText);
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
                _errors.Add(new ApiError("video-upload", response.Status));
            }
        }

        public IReadOnlyResponseErrors Errors => _errors;

        public IReadOnlyResponseWarnings Warnings => _warnings;

        public string Result { get; private set; }

        public int RetryDelay { get; private set; }

    }
}
