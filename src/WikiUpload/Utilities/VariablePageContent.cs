using System.Collections.Generic;
using System.IO;

namespace WikiUpload
{
    internal class VariablePageContent
    {
        public readonly Dictionary<string, VariableContent> _wikiText = new Dictionary<string, VariableContent>();
        private readonly VariableContent _defaultContent;
        private readonly IHelpers _helpers;
        private readonly string _fileExtension;
        private readonly string _fileName;

        public VariablePageContent(string fileExtension, string defaultContent, IHelpers helpers)
        {
            _defaultContent = new VariableContent(defaultContent);
            _helpers = helpers;
            _fileExtension = "." + fileExtension;
            _fileName = "\\" + fileExtension + _fileExtension;
        }

        public string ExpandedContent(UploadFile file)
        {
            var wikiTextFilename = file.FullPath + _fileExtension;
            VariableContent content;
            if (_helpers.FileExists(wikiTextFilename))
            {
                content = new VariableContent(_helpers.ReadAllText(wikiTextFilename));
            }
            else
            {
                if (!_wikiText.TryGetValue(file.Folder, out content))
                {
                    var folderWikiText = file.Folder + _fileName;
                    if (_helpers.FileExists(folderWikiText))
                    {
                        content = new VariableContent(_helpers.ReadAllText(folderWikiText));
                        _wikiText.Add(file.Folder, content);
                    }   
                    else
                    {
                        _wikiText.Add(file.Folder, _defaultContent);
                        content = _defaultContent;
                    }
                }
            }
            return content.ExpandedContent(file);
        }
    }
}
