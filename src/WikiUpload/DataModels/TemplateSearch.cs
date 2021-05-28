using System.Threading.Tasks;

namespace WikiUpload
{
    internal class TemplateSearch : WikiSearch
    {
        private readonly IFileUploader _fileUploader;

        public TemplateSearch(IFileUploader fileUploader)
        {
            _fileUploader = fileUploader;
        }

        public override Task<SearchResponse> FetchData(string from)
        {
            return _fileUploader.FetchTemplates(from);
        }

        public override string FullItemString(string item) => "{{" + item + "}}";
    }
}
