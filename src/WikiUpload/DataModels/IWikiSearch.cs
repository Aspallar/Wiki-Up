using System.Collections.Generic;
using System.ComponentModel;
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

        event PropertyChangedEventHandler PropertyChanged;

        Task<SearchResponse> FetchData(string from);
        string FullItemString(string item);
        Task Next();
        void OnPropertyChanged(string name);
        Task Previous();
        Task Start(string from);
    }
}