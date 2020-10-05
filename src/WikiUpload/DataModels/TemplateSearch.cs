using System.Threading.Tasks;

namespace WikiUpload
{
    public class TemplateSearch : WikiSearch
    {
        private IFileUploader _fileUploader;

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
