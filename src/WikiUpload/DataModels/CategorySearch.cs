using System.Threading.Tasks;

namespace WikiUpload
{
    public class CategorySearch : WikiSearch
    {
        private IFileUploader _fileUploader;

        public CategorySearch(IFileUploader fileUploader)
        {
            _fileUploader = fileUploader;
        }

        public override Task<SearchResponse> FetchData(string from)
        {
            return _fileUploader.FetchCategories(from);
        }

        public override string FullItemString(string item) => $"[[Category:{item}]]";
    }
}
