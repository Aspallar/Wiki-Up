using System.Collections.Generic;
using System.Threading.Tasks;

namespace WikiUpload
{
    public interface IWikiSearch
    {
        List<string> Data { get; }
        string ErrorMessage { get; }
        bool HasNext { get; }
        bool HasPrevious { get; }
        bool IsError { get; }


        Task<SearchResponse> FetchData(string from);
        string FullItemString(string item);
        Task Next();
        Task Previous();
        Task Start(string from);
    }
}